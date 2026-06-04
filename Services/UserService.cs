using Microsoft.AspNetCore.Mvc;

public class UserService : IUserService
{
    private readonly IUserStore _userStore;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserStore userStore, ILogger<UserService> logger)
    {
        _userStore = userStore;
        _logger = logger;
    }

    public User? Register(string email, string password)
    {
        var normalizedEmail = email.ToLowerInvariant();
        if (_userStore.GetByEmail(normalizedEmail) is not null)
        {
            _logger.LogWarning("Registration failed: email {Email} is already in use", email);
            return null;
        }

        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User(Guid.NewGuid(), normalizedEmail, hash);
        _userStore.Add(user);
        _logger.LogInformation("User registered: {Email}", email);
        return user;
    }

    public User? Authenticate(string email, string password)
    {
        var user = _userStore.GetByEmail(email.ToLowerInvariant());
        if (user is null)
        {
            _logger.LogWarning("Authentication failed: email {Email} not found", email);
            return null;
        }
        var isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!isValid)
        {
            _logger.LogWarning("Authentication failed: invalid password for email {Email}", email);
            return null;
        }
        _logger.LogInformation("User authenticated: {Email}", email);
        return user;
    }
}