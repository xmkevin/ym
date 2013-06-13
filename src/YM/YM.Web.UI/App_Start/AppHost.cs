using ServiceStack.Api.Swagger;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.Common.Web;
using ServiceStack.Configuration;
using ServiceStack.MiniProfiler;
using ServiceStack.MiniProfiler.Data;
using ServiceStack.Mvc;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.WebHost.Endpoints;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(YM.Web.UI.AppHost), "Start")]

namespace YM.Web.UI
{
    public class AppHost : AppHostBase
    {
        public AppHost() //Tell ServiceStack the name and where to find your web services  
            : base("YMService", typeof(YM.Service.SlideService).Assembly) { }


        public override void Configure(Funq.Container container)
        {
            //Set JSON web services to return idiomatic JSON camelCase properties
            ServiceStack.Text.JsConfig.EmitCamelCaseNames = true;

            Plugins.Add(new SwaggerFeature());

            //Register all your dependencies: 

            //Register a external dependency-free 
            container.Register<ICacheClient>(new MemoryCacheClient());
            //Configure an alt. distributed peristed cache that survives AppDomain restarts. e.g Redis
            //container.Register<IRedisClientsManager>(c => new PooledRedisClientManager("localhost:6379"));

            //Enable Authentication an Registration
            ConfigureAuth(container);

            //Configure Custom User Defined REST Paths for your services
            RouteConfig.ConfigureServiceRoutes(Routes);

            //Change the default ServiceStack configuration
            //const Feature disableFeatures = Feature.Jsv | Feature.Soap;
            SetConfig(new EndpointHostConfig
            {
                //EnableFeatures = Feature.All.Remove(disableFeatures),
                AppendUtf8CharsetOnContentTypes = new HashSet<string> { ContentType.Html },
                DebugMode = true, //Show StackTraces in service responses during development
            });

            //Set MVC to use the same Funq IOC as ServiceStack
            ControllerBuilder.Current.SetControllerFactory(new FunqControllerFactory(container));
            ServiceStackController.CatchAllController = reqCtx => container.TryResolve<HomeController>();

        }


        private void ConfigureAuth(Funq.Container container)
        {
            //Enable and register existing services you want this host to make use of.
            //Look in Web.config for examples on how to configure your oauth proviers, e.g. oauth.facebook.AppId, etc.
            var appSettings = new AppSettings();

            //Register all Authentication methods you want to enable for this web app.            
            Plugins.Add(new AuthFeature(
                () => new AuthUserSession(), //Use your own typed Custom UserSession type
                new IAuthProvider[] {
                    new BasicAuthProvider()
                }));

            //Provide service for new users to register so they can login with supplied credentials.
            Plugins.Add(new RegistrationFeature());

            //Create a DB Factory configured to access the UserAuth SQL Server DB
            var connStr = appSettings.Get("MYSQL_CONNECTION_STRING", //AppHarbor or Local connection string
                ConfigUtils.GetConnectionString("UserAuth"));
            container.Register<IDbConnectionFactory>(
                new OrmLiteConnectionFactory(connStr, //ConnectionString in Web.Config
                    ServiceStack.OrmLite.MySql.MySqlDialectProvider.Instance)
                    {
                        ConnectionFilter = x => new ProfiledDbConnection(x, Profiler.Current)
                    });

            //Store User Data into the referenced SqlServer database
            container.Register<IUserAuthRepository>(c =>
                new OrmLiteAuthRepository(c.Resolve<IDbConnectionFactory>())); //Use OrmLite DB Connection to persist the UserAuth and AuthProvider info

            var authRepo = (OrmLiteAuthRepository)container.Resolve<IUserAuthRepository>(); //If using and RDBMS to persist UserAuth, we must create required tables
            if (appSettings.Get("RecreateAuthTables", false))
                authRepo.DropAndReCreateTables(); //Drop and re-create all Auth and registration tables
            else
                authRepo.CreateMissingTables();   //Create only the missing tables
            
        }

        public static void Start()
        {
            new AppHost().Init();
        }
    }
}