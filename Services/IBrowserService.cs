using Microsoft.Playwright;
public interface IBrowserService
{
    Task<Byte[]> TakeScreenshotAsync();
    Task ClickAsync(double normalizedX, double normalizedY);
    Task NavigateAsync(string url);
    Task InputTextAsync(string text);
    Task PressKeyAsync(string key);
}