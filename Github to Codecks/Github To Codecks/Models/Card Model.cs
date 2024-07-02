using System.Collections.Generic;

namespace Github_To_Codecks.Models
{
    internal class CardModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string DeckId { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
