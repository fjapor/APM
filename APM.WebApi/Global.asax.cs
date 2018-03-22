using Autofac;
using Autofac.Integration.WebApi;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace APM.WebApi
{
    public class WebApiApplication : HttpApplication
    {
        private static IContainer Container;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterHttpFilters(GlobalConfiguration.Configuration.Filters);

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            CreateIocContainer();
        }
        public  void RegisterHttpFilters(System.Web.Http.Filters.HttpFilterCollection filters)
        {
            filters.Add(new MultiCulture.LocalizationAttribute("en"));
        }
        private void CreateIocContainer()
        {
            var factory = new IoCContainerFactory();
            Container = factory.Create();

            // Create the depenedcy resolver and configure Web API with it
            var resolver = new AutofacWebApiDependencyResolver(Container);
            GlobalConfiguration.Configuration.DependencyResolver = resolver;
        }
    }
}
