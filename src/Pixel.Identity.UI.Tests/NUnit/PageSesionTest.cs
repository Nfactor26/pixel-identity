using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.NUnit;

/// <summary>
/// Inherit fixtures from PageSessionTest when you want all tests in fixture to execute in a single tab
/// e.g. login during setup and execute all test case in same session in that fixture.
/// </summary>
internal class PageSesionTest
{
    public static string BrowserName => (Environment.GetEnvironmentVariable("BROWSER") ?? Microsoft.Playwright.BrowserType.Chromium).ToLower();

    private static readonly Task<IPlaywright> playwrightTask = Microsoft.Playwright.Playwright.CreateAsync();  

    public IPlaywright Playwright { get; private set; }
    
    public IBrowserType BrowserType { get; private set; }
   
    public IBrowser Browser { get; internal set; } 
   
    public IBrowserContext Context { get; private set; }
   
    public IPage Page { get; private set; }

    [OneTimeSetUp]
    public async Task PlaywrightSetup()
    {
        InstallBrowser();
        Playwright = await playwrightTask.ConfigureAwait(false);
        BrowserType = Playwright[BrowserName];    
        Assert.That(BrowserType, Is.Not.Null, $"The requested browser ({BrowserName}) could not be found - make sure your BROWSER env variable is set correctly.");
        Browser = await BrowserType.LaunchAsync(new()
        {
            Headless = Environment.GetEnvironmentVariable("HEADED") != "1"
        });
        Context = await NewContext(ContextOptions()).ConfigureAwait(false);
        Page = await Context.NewPageAsync().ConfigureAwait(false);
    }

    [OneTimeTearDown]
    public async Task BrowserTearDown()
    {
        await this.Context.CloseAsync().ConfigureAwait(false);
        this.Browser = null;
    }

    public virtual BrowserNewContextOptions ContextOptions()
    {
        return null;
    }

    public async Task<IBrowserContext> NewContext(BrowserNewContextOptions options)
    {
       return await Browser.NewContextAsync(options).ConfigureAwait(false);      
    }

    public ILocatorAssertions Expect(ILocator locator) => Assertions.Expect(locator);

    public IPageAssertions Expect(IPage page) => Assertions.Expect(page);
}
