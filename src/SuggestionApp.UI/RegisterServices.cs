using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using SuggestionApp.Library.DI;

namespace SuggestionApp.UI;

public static class RegisterServices
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddMemoryCache();

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
        });
    }
}
