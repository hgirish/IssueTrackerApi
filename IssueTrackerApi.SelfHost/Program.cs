using System;
using System.Web.Http.SelfHost;

namespace IssueTrackerApi.SelfHost
{
    class Program
    {
        static void Main()
        {
            var config = new HttpSelfHostConfiguration("http://localhost:8181");
            WebApiConfiguration.Configure(config);
            var host = new HttpSelfHostServer(config);
            host.OpenAsync();
            Console.WriteLine("IssueApi hosted at {0}", config.BaseAddress);
            Console.ReadLine();
        }
    }
}
