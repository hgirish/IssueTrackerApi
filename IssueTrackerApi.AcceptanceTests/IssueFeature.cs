using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using IssueTrackerApi.Infrastructure;
using IssueTrackerApi.Models;
using Moq;

namespace IssueTrackerApi.AcceptanceTests
{
    public abstract class IssueFeature
    {
        public Mock<IIssueStore> MockIssueStore;
        public HttpResponseMessage Response;
        public IssueLinkFactory IssueLinks;
        public IssueStateFactory StateFactory;
        public IEnumerable<Issue> FakeIssues;
        public HttpRequestMessage Request { get; private set; }
        public HttpClient Client;

        public IssueFeature()
        {
            MockIssueStore = new Mock<IIssueStore>();
            Request = new HttpRequestMessage();
            Request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue(
                    "application/vnd.issue+json"));
            IssueLinks = new IssueLinkFactory(Request);
            StateFactory = new IssueStateFactory(IssueLinks);
            FakeIssues = GetFakeIssues();
            var config = new HttpConfiguration();
            IssueTrackerApi.WebApiConfiguration.Configure(
                config, MockIssueStore.Object);
            var server = new HttpServer(config);
            Client = new HttpClient(server);
        }

        private IEnumerable<Issue> GetFakeIssues()
        {
            var fakeIssues = new List<Issue>();
            fakeIssues.Add(
                new Issue
                {
                    Id = "1",
                    Title = "An issue",
                    Description = "This is an issue",
                    Status = IssueStatus.Open
                });
            fakeIssues.Add(
                new Issue
                {
                    Id = "2",
                    Title = "Another issue",
                    Description = "This is another issue",
                    Status = IssueStatus.Closed
                });
            return fakeIssues;
        }
    }
}