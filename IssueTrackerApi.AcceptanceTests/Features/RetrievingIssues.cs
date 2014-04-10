using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IssueTrackerApi.Infrastructure;
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
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));
            "Then it is returned".f(() => issue.ShouldNotBeNull());
            "Then it should have an id".f(() => issue.Id.ShouldEqual(fakeIssue.Id));
            "Then it should have a title".f(() => issue.Title.ShouldEqual(fakeIssue.Title));
            "Then it should have a description".f(() => issue.Description.ShouldEqual(fakeIssue.Description));
            "Then it should have a state".f(()=>issue.Status.ShouldEqual(Enum.GetName(typeof(IssueStatus),fakeIssue.Status)));
            "Then it should have a 'self' link".f(() =>
            {
                var link = issue.Links.FirstOrDefault(l => l.Rel == LinkFactory.Rels.Self);
                link.ShouldNotBeNull();
                link.Href.AbsoluteUri.ShouldEqual("http://localhost/issue/1");

            }); 
            "Then it should have a transition link".
                f(() =>
                {
                    var link = issue.Links.FirstOrDefault(l => l.Rel == IssueLinkFactory.Rels.IssueProcessor && l.Action == IssueLinkFactory.Actions.Transition);
                    link.ShouldNotBeNull();
                    link.Href.AbsoluteUri.ShouldEqual("http://localhost/issueprocessor/1?action=transition");
                });
        }
    }
}