namespace SuggestionApp.Library.DataAccess;

public interface IStatusData
{
    Task CreateStatus(StatusModel category);
    Task<List<StatusModel>> GetAllStatuses();
}