public class InMemoryStore : ISessionStore
{
    private readonly Dictionary<Guid, SessionInfo> _sessions = new();
    public IEnumerable<SessionInfo> GetAll() => _sessions.Values;
    public SessionInfo? GetById(Guid id) => _sessions.TryGetValue(id, out var session) ? session : null;
    public void Add(SessionInfo session) => _sessions[session.Id] = session;
    public bool Remove(Guid id) => _sessions.Remove(id);
}
public interface ISessionStore
{
    IEnumerable<SessionInfo> GetAll();
    SessionInfo? GetById(Guid id);
    void Add(SessionInfo session);
    bool Remove(Guid id);
}
public record SessionInfo(Guid Id, Guid OwnerId, string Url, DateTime CreatedAt);
