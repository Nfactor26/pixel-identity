using Microsoft.Playwright;
using NUnit.Framework;
using Pixel.Identity.UI.Tests.Helpers;
using Pixel.Identity.UI.Tests.NUnit;
using Pixel.Identity.UI.Tests.PageModels;
using Pixel.Identity.UI.Tests.PageModels.Users;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests;

[TestFixture, Order(35)]
internal class UsersFixture : PageSesionTest
{
    private readonly string baseUrl;

    public UsersFixture()
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
    /// Verify that it is possible to navigate to next page when there is more than 1 page available
    /// </summary>
    /// <returns></returns>
    [Test, Order(10)]
    public async Task Test_That_Can_NavigateToNext_When_Multiple_Pages_Are_Available()
    {
        var listUsersPage = new ListUsersPage(this.Page);
        await listUsersPage.GoToAsync();
        int usersCount = await listUsersPage.GetCountAsync();
        Assert.AreEqual(10, usersCount);
        var canNavigateToNext = await listUsersPage.CanNavigateToNext();
        Assert.IsTrue(canNavigateToNext);
        await listUsersPage.NavigateToNextAsync();
        usersCount = await listUsersPage.GetCountAsync();
        Assert.AreEqual(2, usersCount);
        await Expect(this.Page).ToHaveURLAsync(new Regex(".*/users/list"));
    }

    /// <summary>
    /// Verify that it is possible to navigate to previous page when current page is not first page
    /// </summary>
    /// <returns></returns>
    [Test, Order(20)]
    public async Task Test_That_Can_NavigateToPrevious_If_Not_On_First_Page()
    {
        var listUsersPage = new ListUsersPage(this.Page);
        await listUsersPage.GoToAsync();
        await listUsersPage.NavigateToNextAsync();
        var canNavigateToPrevious = await listUsersPage.CanNavigateToPrevious();
        Assert.IsTrue(canNavigateToPrevious);
        await listUsersPage.NavigateToPreviousAsync();
        int usersCount = await listUsersPage.GetCountAsync();
        Assert.AreEqual(10, usersCount);
        await Expect(this.Page).ToHaveURLAsync(new Regex(".*/users/list"));
    }

    /// <summary>
    /// Verify that it is possible to change page size
    /// </summary>
    /// <param name="pageSize"></param>
    /// <param name="expectedCount"></param>
    /// <returns></returns>
    [Order(30)]
    [TestCase("20", 12)]
    [TestCase("10", 10)]
    public async Task Test_That_Can_ChangePageSize(string pageSize, int expectedCount)
    {
        var listUsersPage = new ListUsersPage(this.Page);
        await listUsersPage.GoToAsync();
        await listUsersPage.SetPageSizeAsync(pageSize);
        int usersCount = await listUsersPage.GetCountAsync();
        Assert.AreEqual(expectedCount, usersCount);
        await Expect(this.Page).ToHaveURLAsync(new Regex(".*/users/list"));
    }

    /// <summary>
    /// Verify that it is possible to search for users
    /// </summary>
    /// <param name="searchFilter"></param>
    /// <param name="pageSize"></param>
    /// <param name="expectedRowCount"></param>
    /// <returns></returns>
    [Order(40)]
    [TestCase("1", "10", 3)]
    [TestCase("2", "10", 1)]
    [TestCase("user", "20", 11)]
    [TestCase("user_1", "10", 3)]
    [TestCase("user_2", "10", 1)]
    [TestCase("user", "10", 10)]
    [TestCase("users_1", "10", 0)]
    public async Task Test_That_Can_Search_Users(string searchFilter, string pageSize, int expectedRowCount)
    {
        var listUsersPage = new ListUsersPage(this.Page);
        await listUsersPage.GoToAsync();
        await listUsersPage.SetPageSizeAsync(pageSize);
        await listUsersPage.SearchAsync(searchFilter);
        int count = await listUsersPage.GetCountAsync();
        Assert.AreEqual(expectedRowCount, count);
        await Expect(this.Page).ToHaveURLAsync(new Regex(".*/users/list"));

    }

    /// <summary>
    /// Verify that roles can be assigned to user
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="roleToAdd"></param>
    /// <returns></returns>
    [Order(50)]
    [TestCase("test_user_1@pixel.com", "IdentityAdmin")] 
    public async Task Verify_that_Can_Add_Roles_To_Users(string userName, string roleToAdd)
    {
        var listUsersPage = new ListUsersPage(this.Page);
        await listUsersPage.GoToAsync();
        await listUsersPage.EditAsync(userName);
        var editUserPage = new EditUserPage(this.Page);
        Assert.IsTrue(await editUserPage.TryAddRole(roleToAdd));
    }

    [Order(55)]
    [TestCase("test_user_1@pixel.com", "IdentityAdmin")]
    public async Task Verify_that_Can_Not_Add_Duplciate_Roles_To_Users(string userName, string roleToAdd)
    {
        var listUsersPage = new ListUsersPage(this.Page);
        await listUsersPage.GoToAsync();
        await listUsersPage.EditAsync(userName);
        await this.Page.Locator("button#btnAddRole").ClickAsync();
        var dialog = this.Page.Locator("div[role='dialog']");
        await dialog.Locator("input.mud-select-input").TypeAsync(roleToAdd);
        await this.Page.Locator("div.mud-popover-provider div.mud-list div.mud-list-item-text").WaitForAsync(new LocatorWaitForOptions()
        {            
            Timeout = 5000
        });     
        var exists = (await this.Page.Locator("div.mud-popover-provider div.mud-list div.mud-list-item-text").CountAsync()) == 1;
        Assert.IsTrue(exists);
        if (exists)
        {
            await this.Page.Locator("div.mud-popover-provider div.mud-list div.mud-list-item-text").ClickAsync();
            await dialog.Locator("button#btnAddRole").ClickAsync();
            Assert.AreEqual("IdentityAdmin role is already assigned to user.", await dialog.Locator("div#errorMessage").InnerTextAsync());
            await dialog.Locator("button[aria-label='close']").ClickAsync();
        }
    }

    /// <summary>
    /// Verify that assigned roles can be removed from users
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="roleToDelete"></param>
    /// <returns></returns>
    [Order(60)]
    [TestCase("test_user_1@pixel.com", "IdentityAdmin")]   
    public async Task Verify_that_Can_Delete_Roles_From_Users(string userName, string roleToDelete)
    {
        var listUsersPage = new ListUsersPage(this.Page);
        await listUsersPage.GoToAsync();
        await listUsersPage.EditAsync(userName);
        var editUserPage = new EditUserPage(this.Page);
        Assert.IsTrue(await editUserPage.TryRemoveRole(roleToDelete));
    }


    /// <summary>
    /// Verify that claims can be assigned to user
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="roleToAdd"></param>
    /// <returns></returns>
    [Order(70)]
    [TestCase("test_user_3@pixel.com", "read-write", "applications", true, false)]
    [TestCase("test_user_4@pixel.com", "read-write", "roles", true, false)]   
    public async Task Verify_that_Can_Add_Claims_To_Users(string userName, string claimType, string claimValue,
            bool includeInAccessToken, bool includeInIdentityToken)
    {
        var listUsersPage = new ListUsersPage(this.Page);
        await listUsersPage.GoToAsync();
        await listUsersPage.EditAsync(userName);
        var editUserPage = new EditUserPage(this.Page);
        Assert.IsTrue(await editUserPage.AddClaimAsync(claimType, claimValue, includeInAccessToken, includeInIdentityToken));
    }

    /// <summary>
    /// Verify that assigned claims can be removed from users
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="roleToDelete"></param>
    /// <returns></returns>
    [Order(80)]
    [TestCase("test_user_3@pixel.com", "read-write", "applications")]
    [TestCase("test_user_4@pixel.com", "read-write", "roles")]
    public async Task Verify_that_Can_Delete_Claims_From_Users(string userName, string claimType, string claimValue)
    {
        var listUsersPage = new ListUsersPage(this.Page);
        await listUsersPage.GoToAsync();
        await listUsersPage.EditAsync(userName);
        var editUserPage = new EditUserPage(this.Page);
        int deletedCount = await editUserPage.DeleteClaimsAsync(claimType, claimValue);
        Assert.AreEqual(1, deletedCount);
    }

    /// <summary>
    /// Verify that it is possible to delete existing users
    /// </summary>
    /// <param name="roleName"></param>
    /// <returns></returns>
    [Order(120)]
    [TestCaseSource(typeof(UserCollection), nameof(UserCollection.GetAllUsers))]
    public async Task Test_That_Can_Delete_Users(User user)
    {
        var listUsersPage = new ListUsersPage(this.Page);
        await listUsersPage.GoToAsync();
        await listUsersPage.SetPageSizeAsync("10");
        var deleted = await listUsersPage.DeleteAsync(user.Email);
        Assert.IsTrue(deleted);
        await listUsersPage.SearchAsync(string.Empty);
        await listUsersPage.SearchAsync(user.Email);
        await Task.Delay(200);
        int count = await listUsersPage.GetCountAsync();
        Assert.AreEqual(0, count);
        await Expect(this.Page).ToHaveURLAsync(new Regex(".*/users/list"));
    }
}
