using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using IssueTrackerApi.Infrastructure;
using IssueTrackerApi.Models;

namespace IssueTrackerApi.Controllers
{
    public class IssueController : ApiController
    {
        private readonly IIssueStore _store;
        private readonly IStateFactory<Issue, IssueState> _stateFactory;
        public IssueController(IIssueStore store, 
            IStateFactory<Issue, IssueState> stateFactory)
        {
            _store = store;
            _stateFactory = stateFactory;
        }

        public async Task<HttpResponseMessage> Get()
        {
            var issues = await _store.FindAsync();
            var issuesState = new IssuesState();
            issuesState.Issues = issues.Select(i => _stateFactory.Create(i));
            issuesState.Links.Add(new Link
                                  {
                                      Href = Request.RequestUri,
                                      Rel = LinkFactory.Rels.Self
                                  });
            return Request.CreateResponse(HttpStatusCode.OK, issuesState);
        }

        public async Task<HttpResponseMessage> Get(string id)
        {
            var issue = await _store.FindAsync(id);
            if (issue == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Request.CreateResponse(HttpStatusCode.OK,
                _stateFactory.Create(issue));
        }
    }
}