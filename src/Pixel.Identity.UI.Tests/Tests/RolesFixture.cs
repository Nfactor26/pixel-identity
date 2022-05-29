using NUnit.Framework;
using Pixel.Identity.UI.Tests.Helpers;
using Pixel.Identity.UI.Tests.NUnit;
using Pixel.Identity.UI.Tests.PageModels;
using Pixel.Identity.UI.Tests.PageModels.Roles;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests
{
    [TestFixture, Order(40)]    
    internal class RolesFixture : PageSesionTest
    {
        private readonly string baseUrl;      

        public RolesFixture()
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
        /// Verify that it is possible to open add role page by clicking on + icon on list role page
        /// </summary>
        /// <returns></returns>
        [Test, Order(10)]
        public async Task Test_That_Can_Open_AddNew_Page()
        {
            var listRolesPage = new ListRolesPage(this.Page);
            await listRolesPage.GoToAsync();
            await listRolesPage.AddNew();
            await Expect(this.Page).ToHaveURLAsync(new Regex(".*/roles/new"));
        }

        /// <summary>
        /// Verify that it is not possible to navigate to next page when table doesn't have
        /// enough rows.
        /// </summary>
        /// <returns></returns>
        [Test, Order(10)]
        public async Task Test_That_Can_Not_NavigateToNext_When_Only_Single_Page_Available()
        {          
            var listRolesPage = new ListRolesPage(this.Page);
            await listRolesPage.GoToAsync();
            int rolesCount = await listRolesPage.GetCountAsync();
            Assert.AreEqual(1, rolesCount);
            var canNavigateToNext = await listRolesPage.CanNavigateToNext();
            Assert.IsFalse(canNavigateToNext);
            await Expect(this.Page).ToHaveURLAsync(new Regex(".*/roles/list"));
        }

        /// <summary>
        /// Verify that it is not possible to navigate to previous page when current page is first page
        /// </summary>
        /// <returns></returns>
        [Test, Order(20)]
        public async Task Test_That_Can_Not_NavigateToPrevious_When_Currently_On_First_Page()
        {
            var listRolesPage = new ListRolesPage(this.Page);
            await listRolesPage.GoToAsync();
            var canNavigateToPrevious = await listRolesPage.CanNavigateToPrevious();
            Assert.IsFalse(canNavigateToPrevious);
            await Expect(this.Page).ToHaveURLAsync(new Regex(".*/roles/list"));
        }

        /// <summary>
        /// Verify that it is possible to add new roles
        /// </summary>
        /// <param name="roleName">Name of the role to add</param>
        /// <returns></returns>
        [Test, Order(30)]
        [TestCase("role-1")]
        [TestCase("role-2")]
        [TestCase("role-3")]
        [TestCase("role-4")]
        [TestCase("role-5")]
        [TestCase("role-6")]
        [TestCase("role-7")]
        [TestCase("role-8")]
        [TestCase("role-9")]
        [TestCase("role-10")]
        [TestCase("role-11")]    
        public async Task Test_That_Can_Create_New_Roles(string roleName)
        {
            var listRolesPage = new ListRolesPage(this.Page);
            await listRolesPage.GoToAsync();
            var addRolesPage = new AddRolePage(this.Page);
            await addRolesPage.GoToAsync();
            await addRolesPage.CreateRole(roleName);
            await Expect(this.Page).ToHaveURLAsync(new Regex(".*/roles/list"));
        }
        
        /// <summary>
        /// Verify that it is not possible to add a duplicate role
        /// </summary>
        /// <param name="roleName">Duplicate role name</param>
        /// <returns></returns>
        [Order(35)]
        [TestCase("role-1")]
        public async Task Test_That_Can_Not_Create_Duplicate_Roles(string roleName)
        {
            var listRolesPage = new ListRolesPage(this.Page);
            await listRolesPage.GoToAsync();
            var addRolesPage = new AddRolePage(this.Page);
            await addRolesPage.GoToAsync();
            await this.Page.FillAsync("#inputRoleName", roleName);
            await this.Page.ClickAsync("#btnCreate");
            await this.Page.ClickAsync("div.mud-snackbar.mud-alert-filled-error button");
            await Expect(this.Page).ToHaveURLAsync(new Regex(".*/roles/new"));
        }

        /// <summary>
        /// Verify that it is possible to navigate to next page when there is more than 1 page available
        /// </summary>
        /// <returns></returns>
        [Test, Order(40)]
        public async Task Test_That_Can_NavigateToNext_When_Multiple_Pages_Are_Available()
        {
            var listRolesPage = new ListRolesPage(this.Page);
            await listRolesPage.GoToAsync();
            int rolesCount = await listRolesPage.GetCountAsync();
            Assert.AreEqual(10, rolesCount); 
            var canNavigateToNext = await listRolesPage.CanNavigateToNext();
            Assert.IsTrue(canNavigateToNext);
            await listRolesPage.NavigateToNextAsync();
            rolesCount = await listRolesPage.GetCountAsync();
            Assert.AreEqual(2, rolesCount);
            await Expect(this.Page).ToHaveURLAsync(new Regex(".*/roles/list"));
        }

        /// <summary>
        /// Verify that it is possible to navigate to previous page when current page is not first page
        /// </summary>
        /// <returns></returns>
        [Test, Order(45)]
        public async Task Test_That_Can_NavigateToPrevious_If_Not_On_First_Page()
        {
            var listRolesPage = new ListRolesPage(this.Page);
            await listRolesPage.GoToAsync();
            await listRolesPage.NavigateToNextAsync();
            var canNavigateToPrevious = await listRolesPage.CanNavigateToPrevious();
            Assert.IsTrue(canNavigateToPrevious);
            await listRolesPage.NavigateToPreviousAsync();
            int rolesCount = await listRolesPage.GetCountAsync();
            Assert.AreEqual(10, rolesCount);
            await Expect(this.Page).ToHaveURLAsync(new Regex(".*/roles/list"));
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
            var listRolesPage = new ListRolesPage(this.Page);
            await listRolesPage.GoToAsync();        
            await listRolesPage.SetPageSizeAsync(pageSize);
            int rolesCount = await listRolesPage.GetCountAsync();
            Assert.AreEqual(expectedCount, rolesCount);
            await Expect(this.Page).ToHaveURLAsync(new Regex(".*/roles/list"));
        }

        /// <summary>
        /// Verify that it is possible to search for roles
        /// </summary>
        /// <param name="searchFilter"></param>
        /// <param name="pageSize"></param>
        /// <param name="expectedRowCount"></param>
        /// <returns></returns>
        [Order(60)]
        [TestCase("1", "10", 3)]
        [TestCase("2", "10", 1)]
        [TestCase("role-1", "10", 3)]
        [TestCase("role-2", "10", 1)]
        [TestCase("rol", "20", 11)]
        [TestCase("role", "10", 10)]       
        public async Task Test_That_Can_Search_Roles(string searchFilter, string pageSize, int expectedRowCount)
        {
            var listRolesPage = new ListRolesPage(this.Page);
            await listRolesPage.GoToAsync();          
            await listRolesPage.SetPageSizeAsync(pageSize);
            await listRolesPage.SearchAsync(searchFilter);
            int count = await listRolesPage.GetCountAsync();
            Assert.AreEqual(expectedRowCount, count);
            await Expect(this.Page).ToHaveURLAsync(new Regex(".*/roles/list"));

        }
      
        /// <summary>
        /// Verify that it is possible to update a role name
        /// </summary>
        /// <param name="existingName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        [Order(70)]
        [TestCase("role-1", "IdentityUser")]
        public async Task Test_That_Role_Name_Can_Be_Updated(string existingName, string newName)
        {
            var listRolesPage = new ListRolesPage(this.Page);
            await listRolesPage.GoToAsync();
            await listRolesPage.EditAsync(existingName);
            var editRolePage = new EditRolePage(this.Page);
            var result = await editRolePage.RenameRoleAsync(newName);
            Assert.IsTrue(result);
            await listRolesPage.GoToAsync();
            await listRolesPage.SearchAsync(newName);
            int count = await listRolesPage.GetCountAsync();
            Assert.AreEqual(1, count);
        }

        /// <summary>
        /// Verify that it should not be possible to edit a role name to another existing role name
        /// </summary>
        /// <param name="existingName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        [Order(80)]
        [TestCase("role-2", "IdentityUser")]
        public async Task Test_That_Role_Name_Can_Not_Be_Updated_To_Existing(string existingName, string newName)
        {
            var listRolesPage = new ListRolesPage(this.Page);
            await listRolesPage.GoToAsync();           
            await listRolesPage.EditAsync(existingName);
            var editRolePage = new EditRolePage(this.Page);
            var result = await editRolePage.RenameRoleAsync(newName);
            Assert.IsFalse(result);          
        }

        /// <summary>
        /// Verify that it is possible to add a new claim to role
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="claimType"></param>
        /// <param name="claimValue"></param>
        /// <param name="includeInAccessToken"></param>
        /// <param name="includeInIdentityToken"></param>
        /// <returns></returns>
        [Order(90)]
        [TestCase("IdentityUser", "read-write", "applications", true, false)]
        [TestCase("IdentityUser", "read-write", "roles", true, false)]
        [TestCase("IdentityUser", "read-write", "dummy", true, true)]
        public async Task Test_That_New_Claim_Can_Be_Added_To_Role(string roleName, string claimType, string claimValue, 
            bool includeInAccessToken, bool includeInIdentityToken)
        {
            var listRolesPage = new ListRolesPage(this.Page);
            await listRolesPage.GoToAsync();
            await listRolesPage.EditAsync(roleName);
            var editRolePage = new EditRolePage(this.Page);
            var result = await editRolePage.AddClaimAsync(claimType, claimValue, includeInAccessToken, includeInIdentityToken);
            Assert.IsTrue(result);
        }
       
        /// <summary>
        /// Verify that it is possible to delete a claim from a role
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="claimType"></param>
        /// <param name="claimValue"></param>
        /// <returns></returns>
        [Order(100)]      
        [TestCase("IdentityUser", "read-write", "dummy")]
        public async Task Test_That_Can_Delete_Claim_From_Role(string roleName, string claimType, string claimValue)
        {
            var listRolesPage = new ListRolesPage(this.Page);
            await listRolesPage.GoToAsync();
            await listRolesPage.EditAsync(roleName);
            var editRolePage = new EditRolePage(this.Page);
            int deletedCount = await editRolePage.DeleteClaimsAsync(claimType, claimValue);
            Assert.AreEqual(1, deletedCount);
        }

        /// <summary>
        /// Verify that it is possible to edit an existing claim on role
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="claimType"></param>
        /// <param name="claimValue"></param>
        /// <param name="newClaimType"></param>
        /// <param name="newClaimValue"></param>
        /// <param name="newIncludeInAccessToken"></param>
        /// <param name="newIncludeInIdentityToken"></param>
        /// <returns></returns>
        [Order(110)]
        [TestCase("IdentityUser", "read-write", "roles", "read-write", "scopes", true, false)]
        public async Task Test_That_Can_Edit_Claim_On_Role(string roleName, string claimType, string claimValue, 
            string newClaimType, string newClaimValue, bool newIncludeInAccessToken, bool newIncludeInIdentityToken)
        {
            var listRolesPage = new ListRolesPage(this.Page);
            await listRolesPage.GoToAsync();
            await listRolesPage.EditAsync(roleName);
            var editRolePage = new EditRolePage(this.Page);
            int editedCount = await editRolePage.EditClaimsAsync(claimType, claimValue, newClaimType, newClaimValue,
                newIncludeInAccessToken, newIncludeInIdentityToken);
            Assert.AreEqual(1, editedCount);
        }

        /// <summary>
        /// Verify that it is possible to delete existing roles
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [Order(120)]
        [TestCase("role-2")]
        [TestCase("role-3")]
        [TestCase("role-4")]
        [TestCase("role-5")]
        [TestCase("role-6")]
        [TestCase("role-7")]
        [TestCase("role-8")]
        [TestCase("role-9")]
        [TestCase("role-10")]
        [TestCase("role-11")]
        public async Task Test_That_Can_Delete_Roles(string roleName)
        {
            var listRolesPage = new ListRolesPage(this.Page);
            await listRolesPage.GoToAsync();
            await listRolesPage.SetPageSizeAsync("10");         
            var deleted = await listRolesPage.DeleteAsync(roleName);
            Assert.IsTrue(deleted);
            await listRolesPage.SearchAsync(string.Empty);
            await listRolesPage.SearchAsync(roleName);
            await Task.Delay(200);
            int count = await listRolesPage.GetCountAsync();
            Assert.AreEqual(0, count);
            await Expect(this.Page).ToHaveURLAsync(new Regex(".*/roles/list"));
        }

        /// <summary>
        /// Verify that it should not be possible to delete a role which is assigned to a user
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [Ignore("To do")]
        [Order(130)]
        public async Task Test_That_Can_Not_Delete_Assigned_Roles(string roleName)
        {
            await Task.CompletedTask;
        }
    }
}
