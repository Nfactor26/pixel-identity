using Microsoft.Playwright;
using NUnit.Framework;
using Pixel.Identity.UI.Tests.Helpers;
using Pixel.Identity.UI.Tests.NUnit;
using Pixel.Identity.UI.Tests.PageModels;
using Pixel.Identity.UI.Tests.PageModels.Applications;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests;

[TestFixture, Order(60)]
internal class ApplicationsFixture : PageSesionTest
{
    private readonly string baseUrl;

    /// <summary>
    /// constructor
    /// </summary>
    public ApplicationsFixture()
    {
        this.baseUrl = ConfigurationFactory.Create()["BaseUrl"];
    }


    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        var configuration = ConfigurationFactory.Create();
        var loginPage = new LoginPage(this.Page);
        await loginPage.GoToAsync(baseUrl);
        await loginPage.LoginAsync(configuration["UserEmail"], configuration["UserSecret"], false);
        Thread.Sleep(2000);
        await Expect(this.Page.Locator("#signedInMenu")).ToBeVisibleAsync();
    }

    /// <summary>
    /// Before each test navigate to home page by clicking on home link
    /// </summary>
    /// <returns></returns>
    [SetUp]
    public async Task SetUp()
    {
        await this.Page.ClickAsync("a[href='./']");
        await Expect(this.Page).ToHaveURLAsync(new Regex(".*/pauth/"));
    }

    /// <summary>
    /// Verify that it is possible to open add application page by clicking on + icon on list application page
    /// </summary>
    /// <returns></returns>
    [Test, Order(10)]
    public async Task Test_That_Can_Open_AddNew_Page()
    {
        var listApplicationsPage = new ListApplicationsPage(this.Page);
        await listApplicationsPage.GoToAsync();
        await listApplicationsPage.AddNew();
        await Expect(this.Page).ToHaveURLAsync(new Regex(".*/applications/new"));
    }

    /// <summary>
    /// Verify that it is not possible to navigate to next page when table doesn't have
    /// enough rows.
    /// </summary>
    /// <returns></returns>
    [Test, Order(10)]
    public async Task Test_That_Can_Not_NavigateToNext_When_Only_Single_Page_Available()
    {
        var listApplicationsPage = new ListApplicationsPage(this.Page);
        await listApplicationsPage.GoToAsync();
        int rolesCount = await listApplicationsPage.GetCountAsync();
        Assert.That(rolesCount, Is.EqualTo(1));
        var canNavigateToNext = await listApplicationsPage.CanNavigateToNext();
        Assert.That(canNavigateToNext, Is.False);
        await Expect(this.Page).ToHaveURLAsync(new Regex(".*/applications/list"));
    }

    /// <summary>
    /// Verify that it is not possible to navigate to previous page when current page is first page
    /// </summary>
    /// <returns></returns>
    [Test, Order(20)]
    public async Task Test_That_Can_Not_NavigateToPrevious_When_Currently_On_First_Page()
    {
        var listApplicationsPage = new ListApplicationsPage(this.Page);
        await listApplicationsPage.GoToAsync();
        var canNavigateToPrevious = await listApplicationsPage.CanNavigateToPrevious();
        Assert.That(canNavigateToPrevious, Is.False);
        await Expect(this.Page).ToHaveURLAsync(new Regex(".*/applications/list"));
    }


    /// <summary>
    /// Validate that can not add application without required details
    /// </summary>
    /// <returns></returns>
    [Test, Order(25)]    
    public async Task Test_That_Can_Not_Create_Application_Without_Required_Details()
    {
        var application = new Application()
        {
            FromTemplate = "Authorization Code Flow"
        };
        var listApplicationsPage = new ListApplicationsPage(this.Page);
        await listApplicationsPage.GoToAsync();
        var addApplicationPage = new AddApplicationPage(this.Page);
        await addApplicationPage.GoToAsync();
        await addApplicationPage.ApplyPreset(application.FromTemplate);
        await this.Page.Locator("button[type='submit']").ClickAsync();
        await Assert.ThatAsync(async () => await this.Page.Locator("p.mud-input-helper-text.mud-input-error").Nth(0).InnerTextAsync(), Is.EqualTo("'Client Id' must not be empty."));
        await Assert.ThatAsync(async () => await this.Page.Locator("p.mud-input-helper-text.mud-input-error").Nth(1).InnerTextAsync(), Is.EqualTo("'Display Name' must not be empty."));
        await Assert.ThatAsync(async () => await this.Page.Locator("div.validation-message").Nth(0).InnerTextAsync(), Is.EqualTo("RedirectUris is required for Authorization Code Flow"));
        await Assert.ThatAsync(async () => await this.Page.Locator("div.validation-message").Nth(1).InnerTextAsync(), Is.EqualTo("PostLogoutRedirectUris is required for Authorization Code Flow"));

    }

    /// <summary>
    /// Validate that can add new application   
    /// </summary>
    /// <returns></returns>
    [Test, Order(30)]
    [TestCaseSource(typeof(ApplicationCollection), nameof(ApplicationCollection.GetAllApplications))]
    public async Task Test_That_Can_Create_New_Application(Application application)
    {
        var listApplicationsPage = new ListApplicationsPage(this.Page);
        await listApplicationsPage.GoToAsync();
        var addApplicationPage = new AddApplicationPage(this.Page);
        await addApplicationPage.GoToAsync();
        await addApplicationPage.AddApplication(application);
        await Expect(this.Page).ToHaveURLAsync(new Regex(".*/applications/list"));
    }


    /// <summary>
    /// Verify that it is not possible to add a duplicate application
    /// </summary>
    [Test, Order(35)]
    public async Task Test_That_Can_Not_Create_Duplicate_Application()
    {
        var application = ApplicationCollection.GetAllApplications().First();
        var listApplicationsPage = new ListApplicationsPage(this.Page);
        await listApplicationsPage.GoToAsync();
        var addApplicationPage = new AddApplicationPage(this.Page);
        await addApplicationPage.GoToAsync();
        await addApplicationPage.ApplyPreset(application.FromTemplate);
        await addApplicationPage.SetClientId(application.ClientId);
        await addApplicationPage.SetDisplayName(application.DisplayName);
        await addApplicationPage.AddRedirectUri(application.RedirectUris);
        await addApplicationPage.AddPostLogoutRedirectUri(application.PostLogoutRedirectUris);
        await this.Page.Locator("button[type='submit']").ClickAsync();
        //wait for the snackbar to show up
        await this.Page.Locator("div.mud-snackbar").WaitForAsync(new LocatorWaitForOptions()
        {
            Timeout = 5000
        });
        await this.Page.Locator("div.mud-snackbar.mud-alert-filled-error button").ClickAsync();
        await this.Page.WaitForSelectorAsync("div.mud-snackbar.mud-alert-filled-error", new PageWaitForSelectorOptions()
        {
            State = WaitForSelectorState.Detached,
            Timeout = 5000
        });      
    }

    /// <summary>
    /// Verify that it is possible to navigate to next page when there is more than 1 page available
    /// </summary>
    /// <returns></returns>
    [Test, Order(40)]
    public async Task Test_That_Can_NavigateToNext_When_Multiple_Pages_Are_Available()
    {
        var listApplicationsPage = new ListApplicationsPage(this.Page);
        await listApplicationsPage.GoToAsync();
        int applicationCount = await listApplicationsPage.GetCountAsync();
        Assert.That(applicationCount, Is.EqualTo(10));
        var canNavigateToNext = await listApplicationsPage.CanNavigateToNext();
        Assert.That(canNavigateToNext, Is.True);
        await listApplicationsPage.NavigateToNextAsync();
        applicationCount = await listApplicationsPage.GetCountAsync();
        Assert.That(applicationCount, Is.EqualTo(2));
        await Expect(this.Page).ToHaveURLAsync(new Regex(".*/applications/list"));
    }

    /// <summary>
    /// Verify that it is possible to navigate to previous page when current page is not first page
    /// </summary>
    /// <returns></returns>
    [Test, Order(45)]
    public async Task Test_That_Can_NavigateToPrevious_If_Not_On_First_Page()
    {
        var listApplicationsPage = new ListApplicationsPage(this.Page);
        await listApplicationsPage.GoToAsync();     
        await Assert.ThatAsync(async () => await listApplicationsPage.GetCountAsync(), Is.EqualTo(10));
        await listApplicationsPage.NavigateToNextAsync();       
        await Assert.ThatAsync(async () => await listApplicationsPage.GetCountAsync(), Is.EqualTo(2));
        var canNavigateToPrevious = await listApplicationsPage.CanNavigateToPrevious();
        Assert.That(canNavigateToPrevious, Is.True);
        await listApplicationsPage.NavigateToPreviousAsync();      
        await Assert.ThatAsync(async () => await listApplicationsPage.GetCountAsync(), Is.EqualTo(10));
        await Expect(this.Page).ToHaveURLAsync(new Regex(".*/applications/list"));
    }

    /// <summary>
    /// Verify that it is possible to change page size
    /// </summary>
    /// <param name="pageSize"></param>
    /// <param name="expectedCount"></param>
    /// <returns></returns>
    [Order(50)]
    [TestCase("20", 12)]
    [TestCase("10", 10)]
    public async Task Test_That_Can_ChangePageSize(string pageSize, int expectedCount)
    {
        var listApplicationsPage = new ListApplicationsPage(this.Page);
        await listApplicationsPage.GoToAsync();
        await listApplicationsPage.SetPageSizeAsync(pageSize);
        int scopesCount = await listApplicationsPage.GetCountAsync();
        Assert.That(scopesCount, Is.EqualTo(expectedCount));
        await Expect(this.Page).ToHaveURLAsync(new Regex(".*/applications/list"));
    }

    /// <summary>
    /// Verify that it is possible to search for applications
    /// </summary>
    /// <param name="searchFilter"></param>
    /// <param name="pageSize"></param>
    /// <param name="expectedRowCount"></param>
    /// <returns></returns>
    [Order(60)]
    [TestCase("1", "10", 3)]
    [TestCase("2", "10", 1)]
    [TestCase("application", "20", 11)]
    [TestCase("application-1", "10", 2)]
    [TestCase("application", "10", 10)]
    public async Task Test_That_Can_Search_Applications(string searchFilter, string pageSize, int expectedRowCount)
    {
        var listApplicationsPage = new ListApplicationsPage(this.Page);
        await listApplicationsPage.GoToAsync();
        await listApplicationsPage.SetPageSizeAsync(pageSize);
        await listApplicationsPage.SearchAsync(searchFilter);
        int count = await listApplicationsPage.GetCountAsync();
        Assert.That(count, Is.EqualTo(expectedRowCount));
        await Expect(this.Page).ToHaveURLAsync(new Regex(".*/applications/list"));
    }

    [Test, Order(70)]
    public async Task Test_That_Can_Edit_Application_Details()
    {        
        var listApplicationsPage = new ListApplicationsPage(this.Page);
        await listApplicationsPage.GoToAsync();
        var editApplicationPage = new EditApplicationPage(this.Page);
        await editApplicationPage.GoToAsync(baseUrl, "application-10");
        await editApplicationPage.SetDisplayName("application-one-zero");
        await editApplicationPage.RemoveRedirectUri(new[] { "http://application-ten/pauth/authentication/login-callback" });
        await editApplicationPage.RemovePostLogoutRedirectUri(new[] { "http://application-zero-ten/pauth/authentication/logout-callback" });
        await editApplicationPage.AddRedirectUri(new[] { "http://application-one-zero/pauth/authentication/login-callback" });
        await editApplicationPage.AddPostLogoutRedirectUri(new[] { "http://application-one-zero/pauth/authentication/logout-callback" });
        await editApplicationPage.AddPermission("address");
        await editApplicationPage.RemovePermission("profile");
        await editApplicationPage.Submit(false);
    }

    [Test, Order(80)]
    public async Task Test_That_Can_Update_JsonWebKey()
    {
        var listApplicationsPage = new ListApplicationsPage(this.Page);
        await listApplicationsPage.GoToAsync();
        var editApplicationPage = new EditApplicationPage(this.Page);
        await editApplicationPage.GoToAsync(baseUrl, "application-11");
        await editApplicationPage.SetJsonWebKey($"""
                    -----BEGIN EC PRIVATE KEY-----
                    MHcCAQEEIMGxf/eMzKuW2F8KKWPJo3bwlrO68rK5+xCeO1atwja2oAoGCCqGSM49
                    AwEHoUQDQgAEI23kaVsRRAWIez/pqEZOByJFmlXda6iSQ4QqcH23Ir8aYPPX5lsV
                    nBsExNsl7SOYOiIhgTaX6+PTS7yxTnmvSw==
                    -----END EC PRIVATE KEY-----
                    """);
        await editApplicationPage.Submit(false);
    }

    [Test, Order(90)]
    public async Task Test_That_Can_Add_New_Setting()
    {
        var listApplicationsPage = new ListApplicationsPage(this.Page);
        await listApplicationsPage.GoToAsync();
        var editApplicationPage = new EditApplicationPage(this.Page);
        await editApplicationPage.GoToAsync(baseUrl, "application-01");
        await editApplicationPage.AddSetting("IdentityToken", "00:05:00");       
        await editApplicationPage.Submit(false);
    }

    [Test, Order(100)]
    public async Task Test_That_Can_Remove_Existing_Setting()
    {
        var listApplicationsPage = new ListApplicationsPage(this.Page);
        await listApplicationsPage.GoToAsync();
        var editApplicationPage = new EditApplicationPage(this.Page);
        await editApplicationPage.GoToAsync(baseUrl, "application-01");   
        await editApplicationPage.RemoveSetting("AccessToken");
        await editApplicationPage.Submit(false);
    }

    [Test, Order(110)]
    public async Task Test_That_Can_Add_Custom_Scope()
    {
        var listApplicationsPage = new ListApplicationsPage(this.Page);
        await listApplicationsPage.GoToAsync();
        var editApplicationPage = new EditApplicationPage(this.Page);
        await editApplicationPage.GoToAsync(baseUrl, "application-10");
        await Assert.ThatAsync(async () => await editApplicationPage.TryAddScope("Offline Access"), Is.True);
        await editApplicationPage.Submit(false);
    }

    /// <summary>
    /// Verify that it is possible to delete existing applications
    /// </summary>
    /// <param name="scope"></param>
    /// <returns></returns>
    [Order(120)]
    [TestCaseSource(typeof(ApplicationCollection), nameof(ApplicationCollection.GetAllApplications))]
    public async Task Test_That_Can_Delete_Applications(Application application)
    {
        var listApplicationsPage = new ListApplicationsPage(this.Page);
        await listApplicationsPage.GoToAsync();
        await listApplicationsPage.SetPageSizeAsync("10");
        var deleted = await listApplicationsPage.DeleteAsync(application.ClientId);
        Assert.That(deleted, Is.True);
        await listApplicationsPage.SearchAsync(string.Empty);
        await listApplicationsPage.SearchAsync(application.ClientId);
        await Task.Delay(200);
        int count = await listApplicationsPage.GetCountAsync();
        Assert.That(count, Is.EqualTo(0));
        await Expect(this.Page).ToHaveURLAsync(new Regex(".*/applications/list"));
    }
}

