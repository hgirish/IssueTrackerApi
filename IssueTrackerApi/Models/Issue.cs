﻿using System;

namespace IssueTrackerApi.Models
{
    public class Issue
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IssueStatus Status { get; set; }
        public DateTime LastModified { get; set; }
    }
}