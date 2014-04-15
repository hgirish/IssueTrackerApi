using System.Collections.Generic;
using System.Threading.Tasks;
using IssueTrackerApi.Models;

namespace IssueTrackerApi.Infrastructure
{
    public interface IIssueStore
    {
        Task<IEnumerable<Issue>> FindAsync();
        Task<Issue> FindAsync(string issueId);
        Task<IEnumerable<Issue>> FindAsyncQuery(string searchText);
        Task UpdateAsync(string issueId, dynamic values);
        Task DeleteAsync(string issueId);
        Task CreateAsync(Issue issue);
    }
}