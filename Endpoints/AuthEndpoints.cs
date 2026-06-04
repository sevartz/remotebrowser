using System.ComponentModel.DataAnnotations;
using MiniValidation;

public static class AuthEndpoints{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth");
        group.MapPost("/register", (RegisterRequest req, IUserService users, ITokenService tokens) =>
        {
            if (!MiniValidator.TryValidate(req, out var errors))
                return Results.ValidationProblem(errors);

            var user = users.Register(req.Email, req.Password);
            if (user is null)
                return Results.Conflict(new { Message = "Email is already in use" });
            
            var token = tokens.GenerateToken(user);
            return Results.Ok(new {token, userId = user.Id , email = user.Email });
        });

        group.MapPost("/login", (LoginRequest req, IUserService users, ITokenService tokens) =>
        {
            if (!MiniValidator.TryValidate(req, out var errors))
                return Results.ValidationProblem(errors);

            var user = users.Authenticate(req.Email, req.Password);
            if (user is null)
                return Results.Unauthorized();

            var token = tokens.GenerateToken(user);
            return Results.Ok(new { token, userId = user.Id, email = user.Email });
            });
    }
}

public record RegisterRequest(
    [Required] [EmailAddress] string Email,
    [Required] [MinLength(8)] string Password
);
public record LoginRequest(
    [Required] [EmailAddress] string Email,
    [Required] string Password
);