using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using SuggestionApp.Library.DI;
using SuggestionApp.UI.Logging;

namespace SuggestionApp.UI;

public static class RegisterServices
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor().AddMicrosoftIdentityConsentHandler(); ;
        builder.Services.AddMemoryCache();
        builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();

        builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"));

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy =>
            {
                policy.RequireClaim("jobTitle", "Admin");
            });
        });

        //builder.Services.AddSingleton<IDbConnection, DbConnection>();
        //builder.Services.AddSingleton<ICategoryData, MongoCategoryData>();
        //builder.Services.AddSingleton<IStatusData, MongoStatusData>();
        //builder.Services.AddSingleton<ISuggestionData, MongoSuggestionData>();
        //builder.Services.AddSingleton<IUserData, MongoUserData>();

        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
        {
            builder.RegisterModule(new LibraryModule());
            builder.RegisterType<SimpleCustomLogger>().As<ILogger>().SingleInstance();
            //builder.RegisterType<MSCustomLogger>().As<ILogger>().SingleInstance();
        });
    }
}
