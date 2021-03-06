﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IssueTrackerApi.Models;
using Should;
using Xbehave;

namespace IssueTrackerApi.AcceptanceTests.Features
{
    public class ProcessingIssue : IssueFeature
    {
        private string _uriProcessor = "http://localhost/issueprocessor/1?";

        [Scenario]
        public void ClosingAnOpenIssue(Issue issue)
        {
            "Given an existing open issue"
                .f(() =>
                   {
                       issue = FakeIssues.FirstOrDefault(
                           i => i.Status == IssueStatus.Open);
                       MockIssueStore.Setup(i => i.FindAsync("1"))
                           .Returns(Task.FromResult(issue));
                       MockIssueStore.Setup(i => i.UpdateAsync("1",issue))
                           .Returns(Task.FromResult(""));
                   });

            "When a POST request is made to the issue processor AND the action is 'close'"
                .f(() =>
                   {
                       Request.RequestUri = new Uri(_uriProcessor + "action=close");
                       Request.Method = HttpMethod.Post;
                       Response = Client.SendAsync(Request).Result;
                   });

            "Then a '200 OK' status is returned"
                .f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));

            "Then the issue is closed"
                .f(() =>
                   {
                       issue.Status.ShouldEqual(IssueStatus.Closed);
                       MockIssueStore.Verify(i => i.UpdateAsync("1",issue));
                   });
        }

        [Scenario]
        public void TransitioningAnOpenIssue(Issue issue)
        {
            "Given an exisiting issue"
                .f(() =>
                   {
                       issue = FakeIssues.FirstOrDefault(
                           i => i.Status == IssueStatus.Open);
                       MockIssueStore.Setup(i => i.FindAsync("1"))
                           .Returns(Task.FromResult(issue));
                       MockIssueStore.Setup(i => i.UpdateAsync("1",issue))
                           .Returns(Task.FromResult(""));
                   });

            "When a POST is made to the issue processor AND the action is 'transition'"
                .f(() =>
                   {
                       Request.RequestUri = new Uri(_uriProcessor + "action=transition");
                       Request.Method = HttpMethod.Post;
                       Response = Client.SendAsync(Request).Result;
                   });

            "Then a '200 OK' status is returned"
                .f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));
            "Then the issue is closed"
                .f(() =>
                   {
                       issue.Status.ShouldEqual(IssueStatus.Closed);
                       MockIssueStore.Verify(i => i.UpdateAsync("1",issue));
                   });
        }

        [Scenario]
        public void ClosingAClosedIssue(Issue issue)
        {
            "Given an exisiting closed issue"
                .f(() =>
                   {
                       issue = FakeIssues.FirstOrDefault(
                           i => i.Status == IssueStatus.Closed);
                       MockIssueStore.Setup(i => i.FindAsync("1"))
                           .Returns(Task.FromResult(issue));
                       MockIssueStore.Setup(i => i.UpdateAsync("1",issue))
                           .Returns(Task.FromResult(""));
                   });

            "When a POST request is made to the issue processor AND the action is 'close'"
                .f(() =>
                   {
                       Request.RequestUri = new Uri(_uriProcessor + "action=close");
                       Request.Method = HttpMethod.Post;
                       Response = Client.SendAsync(Request).Result;
                   });

            "Then a 'BAD Request' status is returned"
                .f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest));
        }

        [Scenario]
        public void OpeningAClosedIssue(Issue issue)
        {
            "Given an existing open issue"
                .f(() =>
                {
                    issue = FakeIssues.FirstOrDefault(
                        i => i.Status == IssueStatus.Closed);
                    MockIssueStore.Setup(i => i.FindAsync("1"))
                        .Returns(Task.FromResult(issue));
                    MockIssueStore.Setup(i => i.UpdateAsync("1",issue))
                        .Returns(Task.FromResult(""));
                });

            "When a POST request is made to the issue processor AND the action is 'close'"
                .f(() =>
                {
                    Request.RequestUri = new Uri(_uriProcessor + "action=open");
                    Request.Method = HttpMethod.Post;
                    Response = Client.SendAsync(Request).Result;
                });

            "Then a '200 OK' status is returned"
                .f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));

            "Then the issue is closed"
                .f(() =>
                {
                    issue.Status.ShouldEqual(IssueStatus.Open);
                    MockIssueStore.Verify(i => i.UpdateAsync("1",issue));
                });
        }

        [Scenario]
        public void TransitioningAClosedIssue(Issue issue)
        {
            "Given an existing closed issue".
                f(() =>
                {
                    issue = FakeIssues.FirstOrDefault(i => i.Status == IssueStatus.Closed);
                    MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(issue));
                    MockIssueStore.Setup(i => i.UpdateAsync("1",issue)).Returns(Task.FromResult(""));
                });
            "When a POST request is made to the issue processor AND the action is 'transition'".
                f(() =>
                {
                    Request.RequestUri = new Uri(_uriProcessor + "action=transition");
                    Request.Method = HttpMethod.Post;
                    Response = Client.SendAsync(Request).Result;
                });
            "Then a '200 OK' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));
            "Then the issue is open".
                f(() =>
                {
                    issue.Status.ShouldEqual(IssueStatus.Open);
                    MockIssueStore.Verify(i => i.UpdateAsync("1",issue));
                });
        }

        [Scenario]
        public void OpeningAnOpenIssue(Issue issue)
        {
            "Given an existing open issue".
                f(() =>
                {
                    issue = FakeIssues.FirstOrDefault(i => i.Status == IssueStatus.Open);
                    MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(issue));
                    MockIssueStore.Setup(i => i.UpdateAsync("1",issue)).Returns(Task.FromResult(""));
                });
            "When a POST request is made to the issue processor AND the action is 'open'".
                f(() =>
                {
                    Request.RequestUri = new Uri(_uriProcessor + "action=open");
                    Request.Method = HttpMethod.Post;
                    Response = Client.SendAsync(Request).Result;
                });
            "Then a '400 Bad Request' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest));
        }

        [Scenario]
        public void OpeningAnIssueThatDoesNotExist()
        {
            "Given an issue does not exist".
                f(() => MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult((Issue)null)));
            "When a POST request is made to the issue processor AND the action is 'open'".
                f(() =>
                {
                    Request.RequestUri = new Uri(_uriProcessor + "action=open");
                    Request.Method = HttpMethod.Post;
                    Response = Client.SendAsync(Request).Result;
                });
            "Then a '404 Not Found' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.NotFound));
        }
        [Scenario]
        public void ClosingAnIssueThatDoesNotExist()
        {
            "Given an issue does not exist".
                f(() => MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult((Issue)null)));
            "When a POST request is made to the issue processor AND the action is 'close'".
                f(() =>
                {
                    Request.RequestUri = new Uri(_uriProcessor + "action=close");
                    Request.Method = HttpMethod.Post;
                    Response = Client.SendAsync(Request).Result;
                });
            "Then a '404 Not Found' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.NotFound));
        }

        [Scenario]
        public void TransitioningAnIssueThatDoesNotExist()
        {
            "Given an issue does not exist".
                f(() => MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult((Issue)null)));
            "When a POST request is made to the issue processor AND the action is 'transition'".
                f(() =>
                {
                    Request.RequestUri = new Uri(_uriProcessor + "action=transition");
                    Request.Method = HttpMethod.Post;
                    Response = Client.SendAsync(Request).Result;
                });
            "Then a '404 Not Found' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.NotFound));

        }

        [Scenario]
        public void InvalidActionOnAnOpenIssue(Issue issue)
        {
            "Given an existing open issue".
                f(() =>
                {
                    issue = FakeIssues.FirstOrDefault(i => i.Status == IssueStatus.Open);
                    MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(issue));
                    MockIssueStore.Setup(i => i.UpdateAsync("1",issue)).Returns(Task.FromResult(""));
                });
            "When a POST request is made to the issue processor AND the action is 'unknown'".
                f(() =>
                {
                    Request.RequestUri = new Uri(_uriProcessor + "action=unknown");
                    Request.Method = HttpMethod.Post;
                    Response = Client.SendAsync(Request).Result;
                });
            "Then a '400 Bad Request' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest));
        }
    }
}