public interface ISessionService{
    IEnumerable<SessionInfo> GetAll(Guid OwnerId, string? search, int? limit);
    SessionInfo? GetById(Guid OwnerId, Guid id);
    SessionInfo Create(Guid OwnerId, string url);
    bool Delete(Guid OwnerId, Guid id);
}