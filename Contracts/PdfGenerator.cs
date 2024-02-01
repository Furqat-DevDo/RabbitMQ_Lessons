using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace Contracts;

public class PdfGenerator(ILogger<PdfGenerator> logger) : IDisposable
{
    private IBrowser? _browser;

    public void Dispose()
    {
        if (_browser != null)
        {
            _browser.Dispose();
        }
    }

    public async ValueTask<IBrowser> Init()
    {
        return _browser ??= await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            Args = ["--no-sandbox", "--disable-dev-shm-usage"],
            ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe"
        });
    }

    public async Task<Stream> GeneratePdfAsync(string html, bool landscape = false)
    {
        var pdfOptions = new PdfOptions
        {
            Format = PaperFormat.A4,
            MarginOptions = new MarginOptions()
            {
                Bottom = "1cm",
                Top = landscape ? "2cm" : "1cm",
                Left = landscape ? "1cm" : "2cm",
                Right = "1cm"
            },
            Landscape = landscape
        };
    
        Stream stream;

        var browser = await Init().ConfigureAwait(false);
        await using (var page = await browser.NewPageAsync())
        {
            await page.SetContentAsync(html, new NavigationOptions { Timeout = 60000 });
            stream = await page.PdfStreamAsync(pdfOptions);
        }

        logger.LogInformation("Pdf generated successfully");

        return stream;
    }
}
