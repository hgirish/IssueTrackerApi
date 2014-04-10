using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using IssueTrackerApi.Infrastructure;

namespace IssueTrackerApi.Controllers
{
    public class IssueController : ApiController
    {
        private IIssueStore _store;

        public IssueController(IIssueStore store)
        {
            _store = store;
        }

        public string Get()
        {
            return "Hello";
        }

        public async Task<HttpResponseMessage> Get(string id)
        {
            var issue = await _store.FindAsync(id);
            if (issue == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}