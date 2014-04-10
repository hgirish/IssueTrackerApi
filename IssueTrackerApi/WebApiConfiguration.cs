using System.Web.Http;
using IssueTrackerApi.Infrastructure;

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
            // TODO
        }

        private static void ConfigureFormatters(HttpConfiguration config)
        {
            // TODO
        }
    }
}