using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace SuggestionApp.UI.Logging;

public class MSCustomLoggerConfiguration
{
    public int EventId { get; set; }

    public Dictionary<LogLevel, LogFormat> LogLevels { get; set; } =
        new()
        {
            [LogLevel.Information] = LogFormat.Short,
            [LogLevel.Warning] = LogFormat.Short,
            [LogLevel.Error] = LogFormat.Long
        };

    public enum LogFormat
    {
        Short,
        Long
    }
}

public class MSCustomLogger : ILogger
{
    private readonly string name;
    private readonly Func<MSCustomLoggerConfiguration> getCurrentConfig;

    public MSCustomLogger(
        string name,
        Func<MSCustomLoggerConfiguration> getCurrentConfig) =>
        (this.name, this.getCurrentConfig) = (name, getCurrentConfig);

    public IDisposable BeginScope<TState>(TState state) => default!;

    public bool IsEnabled(LogLevel logLevel) =>
        getCurrentConfig().LogLevels.ContainsKey(logLevel);

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        MSCustomLoggerConfiguration config = getCurrentConfig();

        if (config.EventId == 0 || config.EventId == eventId.Id)
        {
            switch (config.LogLevels[logLevel])
            {
                case MSCustomLoggerConfiguration.LogFormat.Short:
                    Console.WriteLine($"{name}: {formatter(state, exception)}");
                    break;
                case MSCustomLoggerConfiguration.LogFormat.Long:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{eventId.Id,2}: {logLevel,-12}] {name} - {formatter(state, exception)}");
                    break;
                default:
                    // No-op
                    break;
            }
        }
    }
}

[ProviderAlias("MSCustomLog")]
public sealed class MSCustomLoggerProvider : ILoggerProvider
{
    private readonly IDisposable onChangeToken;
    private MSCustomLoggerConfiguration config;
    private readonly ConcurrentDictionary<string, MSCustomLogger> loggers =
        new(StringComparer.OrdinalIgnoreCase);

    public MSCustomLoggerProvider(
        IOptionsMonitor<MSCustomLoggerConfiguration> config)
    {
        this.config = config.CurrentValue;
        onChangeToken = config.OnChange(updatedConfig => this.config = updatedConfig);
    }

    public ILogger CreateLogger(string categoryName) =>
        loggers.GetOrAdd(categoryName, name => new MSCustomLogger(name, GetCurrentConfig));

    private MSCustomLoggerConfiguration GetCurrentConfig() => config;

    public void Dispose()
    {
        loggers.Clear();
        onChangeToken.Dispose();
    }
}

public static class MSCustomLoggerExtensions
{
    public static ILoggingBuilder AddMSCustomLogger(
        this ILoggingBuilder builder)
    {
        builder.AddConfiguration();

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, MSCustomLoggerProvider>());

        LoggerProviderOptions.RegisterProviderOptions
            <MSCustomLoggerConfiguration, MSCustomLoggerProvider>(builder.Services);

        return builder;
    }
}
