public interface IUserStore
{
    User? GetByEmail(string email);
    User? GetById(Guid id);
    void Add(User user);
}
public record User(Guid Id, string Email, string PasswordHash);