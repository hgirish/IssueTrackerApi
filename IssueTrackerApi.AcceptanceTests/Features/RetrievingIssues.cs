using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IssueTrackerApi.Models;
using Should;
using Xbehave;

namespace IssueTrackerApi.AcceptanceTests.Features
{
    public class RetrievingIssues : IssueFeature
    {
        private Uri _uriIssues = new Uri("http://localhost/issue");
        private Uri _uriIssue1 = new Uri("http://localhost/issue/1");
        private Uri _uriIssue2 = new Uri("http://localhost/issue/2");

        [Scenario]
        public void RetrievingAnIssue(IssueState issue, Issue fakeIssue)
        {
            "Given an existing issue".
                f(() =>
                  {
                      fakeIssue = FakeIssues.FirstOrDefault();
                      MockIssueStore.Setup(i => i.FindAsync("1"))
                          .Returns(Task.FromResult(fakeIssue));
                  });
            "When it is retrieved".
                f(() =>
                  {
                      Request.RequestUri = _uriIssue1;
                      Response = Client.SendAsync(Request).Result;
                      issue = Response.Content.ReadAsAsync<IssueState>().Result;

                  });

            "Then a '200 OK' status is returned".
                f(() =>
                  {
                      Response.StatusCode.ShouldEqual(HttpStatusCode.OK);
                  });
        }
    }
}