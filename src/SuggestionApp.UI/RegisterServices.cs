using Autofac;
using Autofac.Extensions.DependencyInjection;
using SuggestionApp.Library.DI;

namespace SuggestionApp.UI;

public static class RegisterServices
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddMemoryCache();

        //builder.Services.AddSingleton<IDbConnection, DbConnection>();
        //builder.Services.AddSingleton<ICategoryData, MongoCategoryData>();
        //builder.Services.AddSingleton<IStatusData, MongoStatusData>();
        //builder.Services.AddSingleton<ISuggestionData, MongoSuggestionData>();
        //builder.Services.AddSingleton<IUserData, MongoUserData>();

        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
        {
            builder.RegisterModule(new LibraryModule());
        });

    }
}
