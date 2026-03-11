// Copyright © - Unpublished - Toby Hunter
using RestSharp;
using ServerStatusCommon.Models.API;
using ServerStatusCommon.Models.Data;

namespace ServerStatusCommon.Abstractions
{
    /// <summary>
    /// Interface for the API.
    /// </summary>
    public interface IAPIClient
    {
        Task<DateTime?> Authorise();
        Task<RestResponse?> GetUsers();
        Task<RestResponse?> GetUserSettings(int userId);
        Task<RestResponse?> GetServers();
        Task<RestResponse?> GetServerStatuses(string component);
        Task<RestResponse?> UpdateUserSettings(int userSettingsId, string value);
        Task<RestResponse?> UpdateUser(UserModel user);
        Task<RestResponse?> GetAlerts(int pageNumber);
        Task<RestResponse?> GetAlert(int alertId);
        Task<RestResponse?> UpdateAlert(int alertId, string status);
        Task<RestResponse?> RegisterAlert(APINewAlertsModel alert);
        Task<RestResponse?> RegisterServerEvent(APIStatusModel status);
    }
}
