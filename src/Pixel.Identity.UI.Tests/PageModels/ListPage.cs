using Microsoft.Playwright;
using Pixel.Identity.UI.Tests.ComponentModels;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.PageModels
{
  
    internal abstract class ListPage
    {
        protected readonly IPage page;
        protected readonly TableComponent table;
        
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="page"></param>
        public ListPage(IPage page, TableComponent table)
        {
            this.page = page;
            this.table = table;
        }

        protected abstract string GetRequestUrl();
       
        /// <summary>
        /// Click on add new button on list page to navigate to add new page
        /// </summary>
        /// <returns></returns>
        public async Task AddNew()
        {
            await page.ClickAsync("#btnNew");
        }

        /// <summary>
        /// Search rows matching specified filter
        /// </summary>
        /// <param name="filter">search text</param>
        /// <returns></returns>
        public async Task SearchAsync(string filter)
        {
            await table.SearchAndWaitForRequestAsync(filter, GetRequestUrl());
        }

        /// <summary>
        /// Get the number of rows avaialbe in table
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetCountAsync()
        {
            return await table.GetRowCountAsync();
        }

        /// <summary>
        /// Search for specified key and click edit button on first matching row
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> EditAsync(string key)
        {
            await SearchAsync(key);
            return await table.EditFirstMatchingRowAsync(async (row) => (await row.Locator("td[data-label='#']").TextContentAsync())?.Equals(key)
                                                                                   ?? false, GetRequestUrl());
        }

        /// <summary>
        /// Search for specifed key and click delete button on first matching row
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(string key)
        {
            await SearchAsync(key);
            return await table.DeleteFirstMatchingRowAsync(async (row) => (await row.Locator("td[data-label='#']").TextContentAsync())?.Equals(key)
                                                                                    ?? false, GetRequestUrl());
        }

        /// <summary>
        /// Set page size on table
        /// </summary>
        /// <param name="pageSize">PageSize e.g. 10, 20</param>
        /// <returns></returns>
        public async Task<bool> SetPageSizeAsync(string pageSize)
        {
            return await table.SetPageSizeAndWaitForRequestAsync(pageSize, GetRequestUrl());
        }

        /// <summary>
        /// Check if it possible to navigate to next page
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CanNavigateToNext()
        {
            return await table.CanNavigateToNext();
        }

        /// <summary>
        /// Navigate to next page
        /// </summary>
        /// <returns></returns>
        public async Task<bool> NavigateToNextAsync()
        {
            return await table.NavigateToNextAndWaitForRequestAsync(GetRequestUrl());
        }

        /// <summary>
        /// Check if it is possible to navigate to previous page
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CanNavigateToPrevious()
        {
            return await table.CanNavigateToPrevious();
        }

        /// <summary>
        /// Navigate to previous page
        /// </summary>
        /// <returns></returns>
        public async Task<bool> NavigateToPreviousAsync()
        {
            return await table.NavigateToPreviousAndWaitForRequestAsync(GetRequestUrl());
        }
    }
}
