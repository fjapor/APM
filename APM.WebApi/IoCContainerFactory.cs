using APM.WebApi.Repositories;
using APM.WebApi.Services;
using APM.WebAPI.Repositories;
using Autofac;
using Autofac.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace APM.WebApi
{
    public class IoCContainerFactory
    {
        public IContainer Create()
        {
            var builder = new ContainerBuilder();

            // Register WebApi controllers
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            ConfigureIocForRepositories(builder);
            ConfigureIocForServices(builder);

            // Build and return the Container
            var container = builder.Build();
            return container;
        }

        private void ConfigureIocForRepositories(ContainerBuilder builder)
        {
            builder.RegisterType<ProductRepository>().As<IProductRepository>();
            builder.RegisterType<CurrencyRepository>().As<ICurrencyRepository>();
        }

        private void ConfigureIocForServices(ContainerBuilder builder)
        {
            builder.RegisterType<CurrencyConversionService>().As<ICurrencyConversionService>();
        }
    }
}