﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using IssueTrackerApi.Infrastructure;
using IssueTrackerApi.Models;
using Moq;
using Should;
using Should.Core.Assertions;
using Xbehave;

namespace IssueTrackerApi.AcceptanceTests.Features
{
    public class OutputCaching : IssueFeature
    {
        private Uri _uriIssues = new Uri("http://localhost/issue");
        private Uri _uriIssue1 = new Uri("http://localhost/issue/1");

        [Scenario]
        public void RetrievingAllIssues()
        {
            IssuesState issuesState = null;

            "Given existing issues"
                .f(() => MockIssueStore.Setup(i => i.FindAsync())
                    .Returns(Task.FromResult(FakeIssues)));

            "When all issues are retrieved"
                .f(() =>
                {
                    Request.RequestUri = _uriIssues;
                    Response = Client.SendAsync(Request).Result;
                    issuesState = Response.Content
                        .ReadAsAsync<IssuesState>().Result;
                });

            "Then a CacheControl header is returned"
                .f(() =>
                {
                    Response.Headers.CacheControl.Public.ShouldBeTrue();
                    Response.Headers.CacheControl.MaxAge.ShouldEqual(TimeSpan.FromMinutes(5));
                });
            "Then a '200 OK' status is returned"
                .f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));

            "Then they are returned"
                .f(() =>
                {
                    issuesState.Issues.FirstOrDefault(i => i.Id == "1").ShouldNotBeNull();
                    issuesState.Issues.FirstOrDefault(i => i.Id == "2").ShouldNotBeNull();
                });
        }

        [Scenario]
        public void RetrievingAnIssue(IssueState issue, Issue fakeIssue)
        {
            "Given an existing issue".
                f(() =>
                {
                    fakeIssue = FakeIssues.FirstOrDefault();
                    fakeIssue.LastModified = new DateTime(2013, 9, 4);
              
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

            "Then a lastModified header is returned"
                .f(() => Response.Content.Headers.LastModified
                    .ShouldEqual(new DateTimeOffset(new DateTime(2013, 9, 4))));

            "Then a CacheControl header is returned"
                .f(() =>
                {
                    Response.Headers.CacheControl.Public.ShouldBeTrue();
                    Response.Headers.CacheControl.MaxAge.ShouldEqual(TimeSpan.FromMinutes(5));
                });
            
            "Then a '200 OK' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));
            "Then it is returned".f(() => issue.ShouldNotBeNull());
            "Then it should have an id".f(() => issue.Id.ShouldEqual(fakeIssue.Id));
            "Then it should have a title".f(() => issue.Title.ShouldEqual(fakeIssue.Title));
            "Then it should have a description".f(() => issue.Description.ShouldEqual(fakeIssue.Description));
            "Then it should have a state".f(() => issue.Status.ShouldEqual(Enum.GetName(typeof(IssueStatus), fakeIssue.Status)));
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


        [Scenario]
        public void RetrievingNonModifiedIssue(IssueState issue, Issue fakeIssue)
        {
            "Given an existing issue".
                f(() =>
                {
                    fakeIssue = FakeIssues.FirstOrDefault();
                    fakeIssue.LastModified = new DateTime(2013, 9, 4);
                    MockIssueStore.Setup(i => i.FindAsync("1"))
                        .Returns(Task.FromResult(fakeIssue));
                });
            "When it is retrieved with an IfModifiedSince header".
                f(() =>
                {
                    Request.RequestUri = _uriIssue1;
                    Request.Headers.IfModifiedSince = fakeIssue.LastModified;
                    Response = Client.SendAsync(Request).Result;
//                    issue = Response.Content.ReadAsAsync<IssueState>().Result;

                });

            //"Then a lastModified header is returned"
            //    .f(() => Response.Content.Headers.LastModified
            //        .ShouldEqual(new DateTimeOffset(new DateTime(2013, 9, 4))));

            "Then a CacheControl header is returned"
                .f(() =>
                {
                    Response.Headers.CacheControl.Public.ShouldBeTrue();
                    Response.Headers.CacheControl.MaxAge.ShouldEqual(TimeSpan.FromMinutes(5));
                });
            "Then a '304 NOT MODIFIED' status is returened"
                .f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.NotModified));
            "Then it is not returned"
                .f(() => Assert.Null(Response.Content));
           
          
        }

        [Scenario]
        public void RetrievingModifiedIssue(IssueState issue, Issue fakeIssue)
        {
            "Given an existing issue".
                f(() =>
                {
                    fakeIssue = FakeIssues.FirstOrDefault();
                    fakeIssue.LastModified = new DateTime(2013, 9, 4);
                    MockIssueStore.Setup(i => i.FindAsync("1"))
                        .Returns(Task.FromResult(fakeIssue));
                });
            "When it is retrieved with an IfModifiedSince header".
                f(() =>
                {
                    Request.RequestUri = _uriIssue1;
                    Request.Headers.IfModifiedSince = fakeIssue.LastModified
                        .Subtract(TimeSpan.FromDays(1));
                    Response = Client.SendAsync(Request).Result;
                     issue = Response.Content.ReadAsAsync<IssueState>().Result;

                });

            "Then a lastModified header is returned"
                .f(() => Response.Content.Headers.LastModified
                    .ShouldEqual(fakeIssue.LastModified));

            "Then a CacheControl header is returned"
                .f(() =>
                {
                    Response.Headers.CacheControl.Public.ShouldBeTrue();
                    Response.Headers.CacheControl.MaxAge.ShouldEqual(TimeSpan.FromMinutes(5));
                });
            "Then a '200 OK' status is returened"
                .f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));
            "Then it is not returned"
                .f(() => issue.ShouldNotBeNull());


        }

        [Scenario]
        public void UpdatingAnIssueWithNoConflict(Issue fakeIssue)
        {
            "Given an existing issue"
                .f(() =>
                   {
                       fakeIssue = FakeIssues.FirstOrDefault();
                       MockIssueStore.Setup(i => i.FindAsync("1"))
                           .Returns(Task.FromResult(fakeIssue));
                       MockIssueStore.Setup(i => i.UpdateAsync("1", It.IsAny<object>()))
                           .Returns(Task.FromResult(""));
                   });

            "When a PATCH request is made with IfModifiedSince"
                .f(() =>
                   {
                       var issue = new Issue();
                       issue.Title = "Updated title";
                       issue.Description = "Updated description";
                       Request.Method = new HttpMethod("PATCH");
                       Request.RequestUri = _uriIssue1;
                       Request.Content = new ObjectContent<Issue>(issue,
                           new JsonMediaTypeFormatter());
                       Request.Headers.IfModifiedSince = fakeIssue.LastModified;
                       Response = Client.SendAsync(Request).Result;
                   });

            "Then a 200 OK status is returned"
                .f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));

            "Then the issue should be updated"
                .f(() => MockIssueStore.Verify(i => i.UpdateAsync("1",
                    It.IsAny<Object>())));
        }
    }
}