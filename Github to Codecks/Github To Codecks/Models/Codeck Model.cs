using System.Configuration;

namespace Github_To_Codecks.Models
{
    // Stores all the information about the Codeck.
    internal class CodeckModel
    {
        public string Token { get; set; }
        public string SubDomain { get; set; }
    }
}
