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
            DependencyConfig.ResolveDependencies(container);

            //Enable Authentication an Registration
            AuthConfig.ConfigureAuth(Plugins,container);

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
        

        public static void Start()
        {
            new AppHost().Init();
        }
    }
}