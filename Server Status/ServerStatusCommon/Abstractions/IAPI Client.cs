// Copyright © - Unpublished - Toby Hunter
using RestSharp;
using ServerStatusCommon.Models;
using ServerStatusCommon.Models.Requests;
using ServerStatusCommon.Models.Responses;
using ServerStatusCommon.Models.Responses.Related;

namespace ServerStatusCommon.Abstractions
{
    /// <summary>
    /// Interface for the API.
    /// </summary>
    public interface IAPIClient
    {
        Task<(DateTime?, bool)> Authorise();
        Task<(List<UserModel>, bool)> GetUsers();
        Task<(List<UserSettingsModel>, bool)> GetUserSettings(int userId);
        Task<(List<ServerModel>, bool)> GetServers();
        Task<(List<ServerEventModel>, bool)> GetServerStatuses(string component);
        Task<bool> UpdateUserSettings(int userSettingsId, string value);
        Task<bool> UpdateUser(UserModel user);
        Task<(AlertInformationModel?, bool)> GetAlerts(int pageNumber);
        Task<(AlertModel?, bool)> GetAlert(int alertId);
        Task<bool> UpdateAlert(int alertId, string status);
        Task<bool> RegisterAlert(NewAlertModel alert);
        Task<bool> RegisterServerEvent(NewEventModel newEvent);
    }
}
