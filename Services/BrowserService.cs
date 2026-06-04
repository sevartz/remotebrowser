using Microsoft.Playwright;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using System.IO;
public class BrowserService: IBrowserService
{
    private readonly ILogger<BrowserService> _logger;
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IPage? _page;
    private bool _isInitialized; 
    public BrowserService(ILogger<BrowserService> logger)
    {
        _logger = logger;
    }
    private readonly List<string> AllowedDomains = new List<string> {"vk.com", "duckduckgo.com", "google.com"};

    private async Task EnsureInitializedAsync()
    {
        if (_isInitialized)
            return;
        _logger.LogInformation("Initializing Playwright...");

        _playwright = await Playwright.CreateAsync();
        _browser = await  _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        _page = await _browser!.NewPageAsync();
        await _page.GotoAsync("https://google.com");

        _isInitialized = true;
        _logger.LogInformation("Playwright initialized.");
    }


    public async Task<Byte[]> TakeScreenshotAsync()
    {
        await EnsureInitializedAsync();
        var bytes = await _page!.ScreenshotAsync(new PageScreenshotOptions{
        Type = ScreenshotType.Jpeg,
        Quality = 60
        });
        return bytes;
    }

    public async Task ClickAsync(double normalizedX, double normalizedY)
    {
        await EnsureInitializedAsync();

        var viewport = _page!.ViewportSize;
        if (viewport == null) return;
        double x = normalizedX * viewport.Width;
        double y = normalizedY * viewport.Height;
        await _page.Mouse.ClickAsync((float)x, (float)y);
        await _page.EvaluateAsync(@"
        (coords) => {
            const el = document.elementFromPoint(coords.x, coords.y);
            if (el) {
                if (el.tagName === 'INPUT' || el.tagName === 'TEXTAREA' || el.isContentEditable) {
                    el.focus();
                    el.click();
                }
            }
        }
    ", new { x, y });
    }

    public async Task InputTextAsync(string text)
    {
        await EnsureInitializedAsync();
        await _page!.Keyboard.TypeAsync(text);
    }
    public async Task PressKeyAsync(string key)
    {
        await EnsureInitializedAsync();
        await _page!.Keyboard.PressAsync(key);
    }

    public async Task NavigateAsync(string url)
    {
        await EnsureInitializedAsync();
        await _page!.GotoAsync(url);
        
    }


}