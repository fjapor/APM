using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace APM.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            //config.SuppressDefaultHostAuthentication();
            //config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            config.EnableCors();

            config.Routes.MapHttpRoute(
                  name: "DefaultApiLocalized",
                  routeTemplate: "api/{lang}/{controller}/{id}",
                  constraints: new { lang = @"(\w{2})|(\w{2}-\w{2})" }, //en or en-US, pt or pt-BR
                  defaults: new { id = RouteParameter.Optional  }
              );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

        }
    }
}
