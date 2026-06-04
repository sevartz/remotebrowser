using System.ComponentModel.DataAnnotations;

public class InMemoryUserStore : IUserStore
{
    private readonly Dictionary<Guid, User> _byId = new();
    private readonly Dictionary<string, User> _byEmail = new();

    public User? GetByEmail(string email)
    {
        return _byEmail.TryGetValue(email.ToLowerInvariant(), out var user) ? user : null;
    }

    public User? GetById(Guid id)
    {
        return _byId.TryGetValue(id, out var user) ? user : null;
    }

    public void Add(User user)
    {
        _byId[user.Id] = user;
        _byEmail[user.Email.ToLowerInvariant()] = user;
    }
}