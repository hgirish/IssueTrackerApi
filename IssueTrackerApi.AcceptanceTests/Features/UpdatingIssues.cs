using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using IssueTrackerApi.Models;
using Moq;
using Newtonsoft.Json.Linq;
using Should;
using Xbehave;

namespace IssueTrackerApi.AcceptanceTests.Features
{
    public class UpdatingIssues : IssueFeature
    {
        private Uri _uriIssue1 = new Uri("http://localhost/issue/1");

        [Scenario]
        public void UpdatingAnIssue(Issue fakeIssue, string title)
        {
            "Given an existing issue"
                .f(() =>
                   {
                       fakeIssue = FakeIssues.FirstOrDefault();
                       title = fakeIssue.Title;
                       Console.WriteLine(title);
                       MockIssueStore.Setup(i => i.FindAsync("1"))
                           .Returns(Task.FromResult(fakeIssue));
                       MockIssueStore.Setup(i => i.UpdateAsync("1",
                           It.IsAny<Issue>()))
                           .Callback<string, object>((a,x)=> fakeIssue = (Issue) x )
                           .Returns(Task.FromResult(""));
                   });

            "When a PATCH request is made"
                .f(() =>
                   {
                       var issue = new Issue();
                       issue.Description = "Updated description";
                       issue.Title = title;
                       Request.Method = new HttpMethod("PATCH");
                       Request.RequestUri = _uriIssue1;
                       Request.Content = new ObjectContent<Issue>(
                           issue, new JsonMediaTypeFormatter());
                       Response = Client.SendAsync(Request).Result;
                   });

            "Then a '200 OK' status is returned"
                .f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));

            "Then the issue should be updated"
                .f(() => MockIssueStore.Verify(i => i.UpdateAsync("1",It.IsAny<Issue>())));

            "Then the description should be updated"
                .f(() => fakeIssue.Description.ShouldEqual("Updated description"));

            "Then the title should not change"
                .f(() => fakeIssue.Title.ShouldEqual(title));

        }

        [Scenario]
        public void UpdatingAnIssueThatDoesNotExist()
        {
            "Given an issue does not exist"
                .f(() => MockIssueStore.Setup(i => i.FindAsync("1"))
                    .Returns(Task.FromResult((Issue)null)));

            "When a PATCH request is made"
                .f(() =>
                   {
                       Request.Method = new HttpMethod("PATCH");
                       Request.RequestUri = _uriIssue1;
                       Request.Content = new ObjectContent<dynamic>(new JObject(), new JsonMediaTypeFormatter());
                       Response = Client.SendAsync(Request).Result;
                   });
            "Then a 404 Not Found status is returned"
                .f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.NotFound));

        }
    }
}