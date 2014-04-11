using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dependencies;
using Autofac;
using Autofac.Integration.WebApi;
using IssueTrackerApi.Infrastructure;
using IssueTrackerApi.Models;
using WebApiContrib.Formatting.CollectionJson.Client;

namespace IssueTrackerApi
{
    public static  class WebApiConfiguration
    {
        public static void Configure(HttpConfiguration config, 
            IIssueStore issueStore = null)
        {
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "{controller}/{id}",
                new {id = RouteParameter.Optional});
            ConfigureFormatters(config);
            ConfigureAutofac(config, issueStore);
        }

        private static void ConfigureAutofac(HttpConfiguration config, IIssueStore issueStore)
        {
            var builder = new ContainerBuilder();

            if (issueStore == null)
            {
                builder.RegisterType<InMemoryIssueStore>()
                    .As<IIssueStore>()
                    .InstancePerLifetimeScope();

            }
            else
            {
                builder.RegisterInstance(issueStore);
            }

            builder.RegisterType<IssueStateFactory>()
                .As<IStateFactory<Issue, IssueState>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<IssueLinkFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterHttpRequestMessage(config);
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            var container = builder.Build();

            var resolver = 
                new AutofacWebApiDependencyResolver(container);
            
            config.DependencyResolver = resolver;

        }

        private static void ConfigureFormatters(HttpConfiguration config)
        {
            
            config.Formatters.Add(new CollectionJsonFormatter());
// TODO
            config.Formatters.JsonFormatter.SupportedMediaTypes
                .Add(new MediaTypeHeaderValue("application/vnd.collection+json"));

        }
    }
}