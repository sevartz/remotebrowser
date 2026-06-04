using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using System.IO;
public static class PlayEndpoints
{
    public static void MapPlayEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/play").RequireAuthorization();
        group.MapGet("/screenshot", async (IBrowserService svc) => {
            return Results.File(await svc.TakeScreenshotAsync(), "image/jpeg");
        }).WithName("TakeScreenshot");
        group.MapPost("/click", async (IBrowserService svc, ClickRequest req) =>
        {
            await svc.ClickAsync(req.X, req.Y);
            return Results.Ok();
        }).WithName("Click");
        group.MapPost("/navigate", async (IBrowserService svc, NavigateRequest req) =>
        {
            await svc.NavigateAsync(req.Url);
            return Results.Ok();
        }).WithName("Navigate");
        group.MapPost("/input", async (IBrowserService svc, InputRequest req) =>
        {
            await svc.InputTextAsync(req.Text);
            return Results.Ok();
        }).WithName("InputText");
        group.MapPost("/key", async (IBrowserService svc, KeyRequest req) =>
        {
            await svc.PressKeyAsync(req.Key);
            return Results.Ok();
        }).WithName("PressKey");
    }
}

record ClickRequest(
    [Range(0,1)] double X,
    [Range(0,1)] double Y
);
record NavigateRequest(
    [Required] [Url] string Url
);
record InputRequest(
    [Required] string Text
);
record KeyRequest(
    [Required] string Key
);