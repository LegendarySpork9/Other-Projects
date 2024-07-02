using System.Collections.Generic;

namespace Github_To_Codecks.Models
{
    internal class IssueModel
    {
        public int Number { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
