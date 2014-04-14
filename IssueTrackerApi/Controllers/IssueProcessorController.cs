using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using IssueTrackerApi.Infrastructure;
using IssueTrackerApi.Models;

namespace IssueTrackerApi.Controllers
{
    public class IssueProcessorController : ApiController
    {
        private readonly IIssueStore _issueStore;

        public IssueProcessorController(IIssueStore issueStore)
        {
            _issueStore = issueStore;
        }

        public async Task<HttpResponseMessage> Post(string id, string action)
        {
            
            bool isValid = IsValidAction(action);

            Issue issue = null;

            issue = await _issueStore.FindAsync(id);

            if (issue == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            if ((action == IssueLinkFactory.Actions.Open ||
                action == IssueLinkFactory.Actions.Transition) &&
               issue.Status == IssueStatus.Closed)
            {
                issue.Status = IssueStatus.Open;
                isValid = true;
            }
            else if ((action == IssueLinkFactory.Actions.Close ||
                action == IssueLinkFactory.Actions.Transition) &&
               issue.Status == IssueStatus.Open )
            {
                issue.Status = IssueStatus.Closed;
                isValid = true;
            }
            else
            {
                isValid = false;
            }
            if (!isValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    string.Format("Action '{0}' is invalid", action));
            }
            await _issueStore.UpdateAsync(issue);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private bool IsValidAction(string action)
        {
            return (action == IssueLinkFactory.Actions.Close || 
                action == IssueLinkFactory.Actions.Open ||
                action == IssueLinkFactory.Actions.Transition);
        }
    }
}