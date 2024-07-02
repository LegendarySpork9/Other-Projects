using Github_To_Codecks.Models;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Github_To_Codecks.Services
{
    internal class CodeckService
    {
        CodeckTokenModel codeckToken;
        private string accountId;

        public CodeckService(CodeckTokenModel _codeckToken)
        {
            codeckToken = _codeckToken;
        }

        // Gets all the cards in my organisation.
        public async Task<List<CardModel>> GetCards()
        {
            List<CardModel> cards = new List<CardModel>();

            RestClient rest = new RestClient(@"https://api.codecks.io");
            RestRequest request = new RestRequest()
            {
                Method = Method.Get
            };
            request.AddHeader("X-Auth-Token", $"{codeckToken.Token}");
            request.AddHeader("X-Account", "hunter-industries");
            request.AddParameter("query", "{\"_root\": [{\"account\": [{\"cards\": [\"title\",\"status\",\"tags\",\"content\"]}]}]}");
            RestResponse response = await rest.ExecuteAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JObject jsonObject = JObject.Parse(response.Content);

                accountId = jsonObject["_root"]["account"].ToString();

                JToken cardToken = jsonObject["account"][accountId]["cards"];

                string[] cardIds = cardToken.ToObject<string[]>();

                foreach (string id in cardIds)
                {
                    CardModel card = new CardModel
                    {
                        Title = jsonObject["card"][id]["title"].ToString(),
                        Content = jsonObject["card"][id]["content"].ToString()
                    };

                    cards.Add(card);
                }
            }

            return cards;
        }

        // Gets all the projects in my organisation.
        public async Task<List<ProjectModel>> GetProjects()
        {
            List<ProjectModel> projects = new List<ProjectModel>();

            RestClient rest = new RestClient(@"https://api.codecks.io");
            RestRequest request = new RestRequest()
            {
                Method = Method.Get
            };
            request.AddHeader("X-Auth-Token", $"{codeckToken.Token}");
            request.AddHeader("X-Account", "hunter-industries");
            request.AddParameter("query", "{\"_root\": [{\"account\": [{\"projects\": [\"name\"]}]}]}");
            RestResponse response = await rest.ExecuteAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JObject jsonObject = JObject.Parse(response.Content);
                JToken projectToken = jsonObject["account"][accountId]["projects"];
                string[] projectIds = projectToken.ToObject<string[]>();

                foreach (string id in projectIds)
                {
                    ProjectModel project = new ProjectModel
                    {
                        Id = id,
                        Name = jsonObject["project"][id]["name"].ToString()
                    };

                    projects.Add(project);
                }
            }

            return projects;
        }

        // Gets all the decks in my organisation with the name Awaiting Triage.
        public async Task<string> GetDeck(string projectId)
        {
            string deckId = string.Empty;

            RestClient rest = new RestClient(@"https://api.codecks.io");
            RestRequest request = new RestRequest()
            {
                Method = Method.Get
            };
            request.AddHeader("X-Auth-Token", $"{codeckToken.Token}");
            request.AddHeader("X-Account", "hunter-industries");
            request.AddParameter("query", "{\"_root\": [{\"account\": [{\"decks({\\\"title\\\":{\\\"op\\\":\\\"contains\\\",\\\"value\\\":\\\"Awaiting Triage\\\"}})\": [\"title\",\"project\"]}]}]}");
            RestResponse response = await rest.ExecuteAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JObject jsonObject = JObject.Parse(response.Content);
                JToken deckToken = jsonObject["account"][accountId]["decks({\"title\":{\"op\":\"contains\",\"value\":\"Awaiting Triage\"}})"];
                string[] deckIds = deckToken.ToObject<string[]>();

                foreach (string id in deckIds)
                {
                    if (jsonObject["deck"][id]["project"].ToString() == projectId)
                    {
                        deckId = id;
                    }
                }
            }

            return deckId;
        }

        // Creates the card with the given details.
        public async Task<System.Net.HttpStatusCode> CreateCard(CardModel card)
        {
            JObject json = new JObject
            {
                new JProperty("title", card.Title),
                new JProperty("content", card.Content),
                new JProperty("deckId", card.DeckId),
                new JProperty("tags", new JArray(card.Tags)),
                new JProperty("masterTags", new JArray(card.Tags))
            };

            RestClient rest = new RestClient(@"https://api.codecks.io/dispatch/cards/create");
            RestRequest request = new RestRequest()
            {
                Method = Method.Post
            };
            request.AddHeader("X-Auth-Token", $"{codeckToken.Token}");
            request.AddHeader("X-Account", "hunter-industries");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", json.ToString(), ParameterType.RequestBody);
            RestResponse response = await rest.ExecuteAsync(request);

            return response.StatusCode;
        }
    }
}
