using MiniValidation;
using System.ComponentModel.DataAnnotations;

public static class SessionEndpoints{
    public static void MapSessionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/sessions");
        group.MapGet("/", (string? search, int? limit, ISessionService svc, HttpContext ctx) => svc.GetAll(ctx.User.GetUserId(), search, limit));
        group.MapPost("/", (SessionRequest req, ISessionService svc, HttpContext ctx) =>
        {
            var userId = ctx.User.GetUserId();
            if (!MiniValidator.TryValidate(req, out var errors))
                return Results.ValidationProblem(errors);
            var info = svc.Create(userId, req.Url);
            return Results.Created($"/{info.Id}", info);
        }).WithName("CreateSession").RequireAuthorization();

        group.MapGet("/{id:guid}", (Guid id, ISessionService svc, HttpContext ctx) => {
            var userId = ctx.User.GetUserId();
            var session = svc.GetById(userId, id);
            return session is not null
            ? Results.Ok(session) : 
            Results.NotFound();
        }).RequireAuthorization();

        group.MapDelete("/{id:guid}", (Guid id, ISessionService svc, HttpContext ctx) => {
            var userId = ctx.User.GetUserId();
            return svc.Delete(userId, id) ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization().WithName("DeleteSession");

    }
 
}
public record SessionRequest(
    [Required] [Url] string Url
);