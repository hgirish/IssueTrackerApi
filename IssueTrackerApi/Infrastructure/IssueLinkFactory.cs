using System.Net.Http;
using IssueTrackerApi.Controllers;
using IssueTrackerApi.Models;

namespace IssueTrackerApi.Infrastructure
{
    public class IssueLinkFactory : LinkFactory<IssueController>
    {
        private const string PREFIX = "http://webapibook.net/profile#";

        public new class Rels : LinkFactory.Rels
        {
            public const string IssueProcessor = PREFIX + "issue-processor";
            public const string SearchQuery = PREFIX + "search";
        }

        public class Actions
        {
            public const string Open = "open";
            public const string Close = "close";
            public const string Transition = "transition";
        }

        public IssueLinkFactory(HttpRequestMessage request)
            :base(request)
        {
        }

        public Link Transition(string id)
        {
            return GetLink<IssueProcessorController>(
                Rels.IssueProcessor, id, Actions.Transition);
        }

        public Link Open(string id)
        {
            return GetLink<IssueProcessorController>(
                Rels.IssueProcessor, id, Actions.Open);
        }
        public Link Close(string id)
        {
            return GetLink<IssueProcessorController>(
                Rels.IssueProcessor, id, Actions.Close);
        }

    }

   
}