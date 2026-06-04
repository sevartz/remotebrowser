using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Session;

public class SessionService : ISessionService
{
    private readonly ISessionStore _store;
    private readonly ILogger<SessionService> _logger;

    public SessionService(ISessionStore store, ILogger<SessionService> logger)
    {
        _store = store;
        _logger = logger;
    }
    public IEnumerable<SessionInfo> GetAll(Guid OwnerId, string? search, int? limit)
    {
        var query = _store.GetAll().Where(s => s.OwnerId == OwnerId);


        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(s => s.Url.Contains(search));
        }
        if (limit.HasValue)
        {
            query = query.Take(limit.Value);
        }
        return query;
    }

    public SessionInfo? GetById(Guid OwnerId, Guid id){
        var session = _store.GetById(id);
        return session is not null && session.OwnerId == OwnerId ? session : null;
    }
    public SessionInfo Create(Guid OwnerId, string url)
    {
        var session = new SessionInfo(Guid.NewGuid(), OwnerId, url, DateTime.UtcNow);
        _store.Add(session);
        _logger.LogInformation("Created {Id} for url {Url}", session.Id, session.Url);
        return session;
    }
    public bool Delete(Guid OwnerId, Guid id)
    {
        var session = GetById(OwnerId, id);
        if (session == null)
            return false;
        _logger.LogInformation("Deleted {Id} for url {Url}", session.Id, session.Url);
        return _store.Remove(id);
    }
}