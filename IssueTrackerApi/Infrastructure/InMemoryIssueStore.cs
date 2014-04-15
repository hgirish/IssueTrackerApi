using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IssueTrackerApi.Models;

namespace IssueTrackerApi.Infrastructure
{
    public class InMemoryIssueStore : IIssueStore
    {
        private IList<Issue> _issues;
        private int _id = 0;
        private static Type _issueType = typeof(Issue);

        public InMemoryIssueStore()
        {
            _issues = new List<Issue>();
            _issues.Add(new Issue { Description = "This is an issue", Id = "1", Status = IssueStatus.Open, Title = "An issue" });
            _issues.Add(new Issue { Description = "This is a another issue", Id = "2", Status = IssueStatus.Closed, Title = "Another Issue" });
            _id = _issues.Count + 1;
        }
        public Task<IEnumerable<Issue>> FindAsync()
        {
            return Task.FromResult(_issues.AsEnumerable());
        }

        public Task<Issue> FindAsync(string issueId)
        {
            return Task.FromResult(_issues.Single(i => i.Id == issueId));
        }

        public Task<IEnumerable<Issue>> FindAsyncQuery(string searchText)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(string issueId, dynamic values)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(string issueId)
        {
            throw new System.NotImplementedException();
        }

        public Task CreateAsync(Issue issue)
        {
            throw new System.NotImplementedException();
        }
    }
}