using Autofac;
using SuggestionApp.Library.DataAccess;

namespace SuggestionApp.Library.DI;

public class LibraryModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<DbConnection>().As<IDbConnection>().SingleInstance();
        builder.RegisterType<MongoCategoryData>().As<ICategoryData>().SingleInstance();
        builder.RegisterType<MongoStatusData>().As<IStatusData>().SingleInstance();
        builder.RegisterType<MongoSuggestionData>().As<ISuggestionData>().SingleInstance();
        builder.RegisterType<MongoUserData>().As<IUserData>().SingleInstance();
    }
}
