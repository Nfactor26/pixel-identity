using Microsoft.Playwright;
using NUnit.Framework;
using Pixel.Identity.UI.Tests.Helpers;
using Pixel.Identity.UI.Tests.NUnit;
using Pixel.Identity.UI.Tests.PageModels;
using Pixel.Identity.UI.Tests.PageModels.Scopes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests
{
    [TestFixture, Order(50)]
    internal class ScopesFixture : PageSesionTest
    {
        private readonly string baseUrl;

        public ScopesFixture()
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
        /// Verify that it is possible to open add scope page by clicking on + icon on list scopes page
        /// </summary>
        /// <returns></returns>
        [Test, Order(10)]
        public async Task Test_That_Can_Open_AddNew_Scope_Page()
        {
            var listScopesPage = new ListScopesPage(this.Page);
            await listScopesPage.GoToAsync();
            await listScopesPage.AddNew();
            await Expect(this.Page).ToHaveURLAsync(new Regex(".*/scopes/new"));
        }

        /// <summary>
        /// Verify that it is not possible to navigate to next page when table doesn't have
        /// enough rows.
        /// </summary>
        /// <returns></returns>
        [Test, Order(10)]
        public async Task Test_That_Can_Not_NavigateToNext_When_Only_Single_Page_Available()
        {
            var listScopesPage = new ListScopesPage(this.Page);
            await listScopesPage.GoToAsync();
            int scopesCount = await listScopesPage.GetCountAsync();
            Assert.AreEqual(2, scopesCount);
            var canNavigateToNext = await listScopesPage.CanNavigateToNext();
            Assert.IsFalse(canNavigateToNext);
            await Expect(this.Page).ToHaveURLAsync(new Regex(".*/scopes/list"));
        }

        /// <summary>
        /// Verify that it is not possible to navigate to previous page when current page is first page
        /// </summary>
        /// <returns></returns>
        [Test, Order(20)]
        public async Task Test_That_Can_Not_NavigateToPrevious_When_Currently_On_First_Page()
        {
            var listScopesPage = new ListScopesPage(this.Page);
            await listScopesPage.GoToAsync();
            var canNavigateToPrevious = await listScopesPage.CanNavigateToPrevious();
            Assert.IsFalse(canNavigateToPrevious);
            await Expect(this.Page).ToHaveURLAsync(new Regex(".*/scopes/list"));
        }

        /// <summary>
        /// Verify that it is possible to add new scopes
        /// </summary>
        /// <param name="roleName">Name of the scope to add</param>
        /// <returns></returns>
        [Test, Order(30)]
        [TestCase("scope-01", "resource-01", "scope description")]
        [TestCase("scope-02", "resource-02", "scope description")]
        [TestCase("scope-03", "resource-03", "scope description")]
        [TestCase("scope-04", "resource-04", "scope description")]
        [TestCase("scope-05", "resource-05", "scope description")]
        [TestCase("scope-06", "resource-06", "scope description")]
        [TestCase("scope-07", "resource-07", "scope description")]
        [TestCase("scope-08", "resource-08", "scope description")]
        [TestCase("scope-09", "resource-09", "scope description")]
        [TestCase("scope-10", "resource-10", "scope description")]      
        public async Task Test_That_Can_Create_New_Scopes(string scopeName, string resource, string description)
        {
            var listScopesPage = new ListScopesPage(this.Page);
            await listScopesPage.GoToAsync();
            var addScopesPage = new AddScopePage(this.Page);
            await addScopesPage.GoToAsync();
            await addScopesPage.CreateScope(scopeName, description, resource);
            await Expect(this.Page).ToHaveURLAsync(new Regex(".*/scopes/list"));
        }

        /// <summary>
        /// Verify that it is not possible to add a duplicate scope
        /// </summary>
        /// <param name="scopeName">Duplicate scope name</param>
        /// <returns></returns>
        [Order(35)]
        [TestCase("scope-01")]
        [Ignore("To fix error reponse")]
        public async Task Test_That_Can_Not_Create_Duplicate_Scope(string scopeName)
        {
            var listScopesPage = new ListScopesPage(this.Page);
            await listScopesPage.GoToAsync();
            var addScoesPage = new AddScopePage(this.Page);
            await addScoesPage.GoToAsync();
            await this.Page.FillAsync("#inputScopeName", scopeName);
            await this.Page.FillAsync("#inputDisplayName", scopeName);
            await this.Page.ClickAsync("#btnAddScope");
            await this.Page.ClickAsync("div.mud-snackbar.mud-alert-filled-error button");
            await Expect(this.Page).ToHaveURLAsync(new Regex(".*/scopes/new"));
        }

        /// <summary>
        /// Verify that it is possible to navigate to next page when there is more than 1 page available
        /// </summary>
        /// <returns></returns>
        [Test, Order(40)]
        public async Task Test_That_Can_NavigateToNext_When_Multiple_Pages_Are_Available()
        {
            var listScopesPage = new ListScopesPage(this.Page);
            await listScopesPage.GoToAsync();
            int scopesCount = await listScopesPage.GetCountAsync();
            Assert.AreEqual(10, scopesCount);
            var canNavigateToNext = await listScopesPage.CanNavigateToNext();
            Assert.IsTrue(canNavigateToNext);
            await listScopesPage.NavigateToNextAsync();
            scopesCount = await listScopesPage.GetCountAsync();
            Assert.AreEqual(2, scopesCount);
            await Expect(this.Page).ToHaveURLAsync(new Regex(".*/scopes/list"));
        }

        /// <summary>
        /// Verify that it is possible to navigate to previous page when current page is not first page
        /// </summary>
        /// <returns></returns>
        [Test, Order(45)]
        public async Task Test_That_Can_NavigateToPrevious_If_Not_On_First_Page()
        {
            var listScopesPage = new ListScopesPage(this.Page);
            await listScopesPage.GoToAsync();
            await listScopesPage.NavigateToNextAsync();
            var canNavigateToPrevious = await listScopesPage.CanNavigateToPrevious();
            Assert.IsTrue(canNavigateToPrevious);
            await listScopesPage.NavigateToPreviousAsync();
            int scopesCount = await listScopesPage.GetCountAsync();
            Assert.AreEqual(10, scopesCount);
            await Expect(this.Page).ToHaveURLAsync(new Regex(".*/scopes/list"));
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
            var listScopesPage = new ListScopesPage(this.Page);
            await listScopesPage.GoToAsync();
            await listScopesPage.SetPageSizeAsync(pageSize);
            int scopesCount = await listScopesPage.GetCountAsync();
            Assert.AreEqual(expectedCount, scopesCount);
            await Expect(this.Page).ToHaveURLAsync(new Regex(".*/scopes/list"));
        }

        /// <summary>
        /// Verify that it is possible to search for scopes
        /// </summary>
        /// <param name="searchFilter"></param>
        /// <param name="pageSize"></param>
        /// <param name="expectedRowCount"></param>
        /// <returns></returns>
        [Order(60)]
        [TestCase("1", "10", 2)]
        [TestCase("2", "10", 1)]
        [TestCase("scope", "20", 10)]
        [TestCase("scope-1", "10", 1)]
        [TestCase("scope", "10", 10)]       
        public async Task Test_That_Can_Search_Scopes(string searchFilter, string pageSize, int expectedRowCount)
        {
            var listScopesPage = new ListScopesPage(this.Page);
            await listScopesPage.GoToAsync();
            await listScopesPage.SetPageSizeAsync(pageSize);
            await listScopesPage.SearchAsync(searchFilter);
            int count = await listScopesPage.GetCountAsync();
            Assert.AreEqual(expectedRowCount, count);
            await Expect(this.Page).ToHaveURLAsync(new Regex(".*/scopes/list"));
        }

        /// <summary>
        /// Verify that it is possible to update scope details
        /// </summary>
        /// <param name="existingName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        [Order(70)]
        [TestCase("scope-01", "Scope One", "Scope One description")]
        public async Task Test_That_Can_Update_DisplayName_And_Description(string scopeName, string newDisplayName, string newDescription)
        {
            var listScopesPage = new ListScopesPage(this.Page);
            await listScopesPage.GoToAsync();
            await listScopesPage.EditAsync(scopeName);
            var editScopePage = new EditScopePage(this.Page);
            var result = await editScopePage.UpdateAsync(newDisplayName, newDescription);
            Assert.IsTrue(result);
            await listScopesPage.GoToAsync();
            await listScopesPage.SearchAsync(newDisplayName);
            int count = await listScopesPage.GetCountAsync();
            Assert.AreEqual(1, count);
        }

        /// <summary>
        /// Verify that it is possible to add a new resource to scope
        /// </summary>
        /// <param name="scopeName"></param>
        /// <param name="resourceToAdd"></param>
        /// <returns></returns>
        [Order(80)]
        [TestCase("scope-01", "resource-02")]
        public async Task Test_That_Can_Add_Resource_On_Existing_Scope(string scopeName, string resourceToAdd)
        {
            var listScopesPage = new ListScopesPage(this.Page);
            await listScopesPage.GoToAsync();
            await listScopesPage.EditAsync(scopeName);
            var editScopePage = new EditScopePage(this.Page);
            var result = await editScopePage.AddResourceAsync(resourceToAdd);
            Assert.IsTrue(result);          
        }

        /// <summary>
        /// Verify that it is possible to remove existing resource from scope
        /// </summary>
        /// <param name="existingName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        [Order(90)]
        [TestCase("scope-01", "resource-01")]
        public async Task Test_That_Can_Not_Add_Duplicate_Resource_On_Existing_Scope(string scopeName, string resourceToAdd)
        {
            var listScopesPage = new ListScopesPage(this.Page);
            await listScopesPage.GoToAsync();
            await listScopesPage.EditAsync(scopeName);
            await this.Page.ClickAsync("#btnAddResources");
            await this.Page.FillAsync("#inputResourceName", resourceToAdd);
            await this.Page.ClickAsync("#btnAddResource");
            await this.Page.Locator("div#errorAlert").WaitForAsync(new LocatorWaitForOptions()
            {
                Timeout = 5000
            });
            await this.Page.Locator("div#errorAlert").IsVisibleAsync();
            await this.Page.Locator("button[aria-label='close']").ClickAsync();
        }


        /// <summary>
        /// Verify that it is possible to delete resource from scope
        /// </summary>
        /// <param name="scopeName"></param>
        /// <param name="resourceToDelete"></param>       
        /// <returns></returns>
        [Order(90)]
        [TestCase("scope-01", "resource-02")]
        public async Task Test_That_Can_Delete_Resource_From_Scope(string scopeName, string resourceToDelete)
        {
            var listScopesPage = new ListScopesPage(this.Page);
            await listScopesPage.GoToAsync();
            await listScopesPage.EditAsync(scopeName);
            var editScopePage = new EditScopePage(this.Page);
            var deleted = await editScopePage.DeleteResourceAsync(resourceToDelete);
            Assert.IsTrue(deleted);
            var exists = await editScopePage.CheckIfResourceExists(resourceToDelete);
            Assert.IsFalse(exists);
        }

        /// <summary>
        /// Verify that it is possible to delete existing scopes
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        [Order(120)]
        [TestCase("scope-01")]
        [TestCase("scope-02")]
        [TestCase("scope-03")]
        [TestCase("scope-04")]
        [TestCase("scope-05")]
        [TestCase("scope-06")]
        [TestCase("scope-07")]
        [TestCase("scope-08")]
        [TestCase("scope-09")]
        [TestCase("scope-10")]
        public async Task Test_That_Can_Delete_Scopes(string scope)
        {
            var listScopesPage = new ListScopesPage(this.Page);
            await listScopesPage.GoToAsync();
            await listScopesPage.SetPageSizeAsync("10");
            var deleted = await listScopesPage.DeleteAsync(scope);
            Assert.IsTrue(deleted);
            await listScopesPage.SearchAsync(string.Empty);
            await listScopesPage.SearchAsync(scope);
            await Task.Delay(200);
            int count = await listScopesPage.GetCountAsync();
            Assert.AreEqual(0, count);
            await Expect(this.Page).ToHaveURLAsync(new Regex(".*/scopes/list"));
        }

    }
}
