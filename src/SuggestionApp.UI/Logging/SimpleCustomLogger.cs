using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;

namespace SuggestionApp.UI.Logging;

public class SimpleCustomLogger : ILogger
{
    public IDisposable BeginScope<TState>(TState state) => default!;


    public bool IsEnabled(LogLevel logLevel)
    {
        switch (logLevel)
        {
            case LogLevel.Error:
                return true;
            default:
                return false;
        }
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"My logger is logging exception {exception?.StackTrace}.");
    }
}

public sealed class SimpleCustomLoggerProvider : ILoggerProvider
{

    public ILogger CreateLogger(string categoryName)
    {
        return new SimpleCustomLogger();
    }

    public void Dispose()
    {
    }
}

public static class SimpleCustomLoggerExtensions
{
    public static ILoggingBuilder AddSimpleCustomLogger(
        this ILoggingBuilder builder)
    {
        builder.AddConfiguration();

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, SimpleCustomLoggerProvider>());

        //LoggerProviderOptions.RegisterProviderOptions
        //    <CustomLoggerConfiguration, ExceptionLoggerProvider>(builder.Services);

        return builder;
    }
}
