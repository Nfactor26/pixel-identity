using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pixel.Identity.UI.Tests.ComponentModels;
/// <summary>
/// Automation wrapper around MudTable component
/// </summary>
internal class TableComponent
{
    protected readonly IPage page;
    protected readonly ILocator table;   
   
    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="page"></param>
    /// <param name="table"></param>
    public TableComponent(IPage page, ILocator table)
    {
        this.page = page;
        this.table = table;        
    }

    /// <summary>
    /// Apply filter without waiting for any request to finish.
    /// This is useful where filter are executed locally.
    /// </summary>
    /// <param name="filter">Filter text to apply</param>
    /// <returns></returns>
    public async Task SearchAsync(string filter)
    {
        await this.table.Locator("#txtSearchBox").FillAsync(filter);
        await this.table.Locator("#txtSearchBox").PressAsync("Enter");
    }

    /// <summary>
    /// Search roles matching specified filter and wait for requet to finish
    /// </summary>
    /// <param name="filter">search text</param>
    /// <param name="requestUrl">Used with RequestFinished predicate</param>
    /// <returns></returns>
    public async Task SearchAndWaitForRequestAsync(string filter, string requestUrl)
    {
        await this.page.RunAndWaitForRequestFinishedAsync(async () =>
        {
           await SearchAsync(filter);
        },
        new PageRunAndWaitForRequestFinishedOptions()
        {
           Predicate = request =>
           {
               return request.Url.Contains($"{requestUrl}?") && request.Method == "GET";
           }
        });
    }

    /// <summary>
    /// Get the number of roles avaialbe on roles table
    /// </summary>
    /// <returns></returns>
    public async Task<int> GetRowCountAsync()
    {
        return await this.table.Locator("table.mud-table-root>tbody tr").CountAsync();
    }

    /// <summary>
    /// Get all the matching rows filterd by predicate
    /// </summary>
    /// <param name="predicate">Predicate to filter rows</param>
    /// <returns></returns>
    public async Task<IEnumerable<ILocator>> GetMatchingRowsAsync(Func<ILocator, Task<bool>> predicate)
    {
        List<ILocator> matchingRows = new List<ILocator>();
        int rowCount = await GetRowCountAsync();
        for (int i = 0; i < rowCount; i++)
        {
            var row = this.table.Locator("table.mud-table-root>tbody tr").Nth(i);
            if (await predicate(row))
            {
                matchingRows.Add(row);  
            }
        }
        return matchingRows;
    }

    /// <summary>
    /// Search for specified role and click the edit button and wait for request to finish
    /// </summary>
    /// <param name="predicate">Predicate to select target row</param>
    /// <param name="requestUrl">Used with RequestFinished predicate</param>
    /// <returns></returns>
    public async Task<bool> EditFirstMatchingRowAsync(Func<ILocator, Task<bool>> predicate, string requestUrl)
    {
        int rowCount = await GetRowCountAsync();
        for (int i = 0; i < rowCount; i++)
        {
            var row = this.table.Locator("table.mud-table-root>tbody tr").Nth(i);
            if (await predicate(row))
            {
                await this.page.RunAndWaitForRequestFinishedAsync(async () =>
                {
                    await row.Locator("#btnEdit").ClickAsync();
                },
                new PageRunAndWaitForRequestFinishedOptions()
                {
                    Predicate = request => request.Url.Contains($"{requestUrl}") && request.Method == "GET"
                });                        
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Search for specifed role and click on delete button
    /// </summary>
    /// <param name="predicate">Predicate to select target row</param>
    /// <param name="requestUrl">Used with RequestFinished predicate</param>
    /// <returns></returns>
    public async Task<bool> DeleteFirstMatchingRowAsync(Func<ILocator, Task<bool>> predicate, string requestUrl)
    {
        int rowCount = await GetRowCountAsync();
        for (int i = 0; i < rowCount; i++)
        {
            var row = this.table.Locator("table.mud-table-root>tbody tr").Nth(i);          
            if (await predicate(row))
            {
                await this.page.RunAndWaitForRequestFinishedAsync(async () =>
                {
                    await row.Locator("#btnDelete").ClickAsync();
                    await this.page.Locator("div[role='dialog'] button.mud-button-text-primary").ClickAsync();
                },
                new PageRunAndWaitForRequestFinishedOptions()
                {
                    Predicate = request => request.Url.Contains($"{requestUrl}") && request.Method == "DELETE"
                });
                await this.page.ClickAsync("div.mud-snackbar.mud-alert-filled-success button");
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Change page size on table and wait for get request to finish.
    /// </summary>
    /// <param name="pageSize">Page size to be set</param>
    /// <param name="requestUrl">Used with RequestFinished predicate</param>
    /// <returns></returns>
    public async Task<bool> SetPageSizeAndWaitForRequestAsync(string pageSize, string requestUrl)
    {
        if ((await this.table.Locator("div.mud-table-pagination-select").InnerTextAsync()).Equals(pageSize))
        {
            return true;
        }
        await this.table.Locator("div.mud-table-pagination div.mud-select[id*='select']").ClickAsync();
        //popup node is not within table
        var itemsCount = await this.page.Locator("div.mud-popover-provider div.mud-list-item-text>p").CountAsync();
        for (int i = 0; i < itemsCount; i++)
        {
            var item = this.page.Locator("div.mud-popover-provider div.mud-list-item-text>p").Nth(i);
            var itemText = await item.TextContentAsync();
            if (itemText?.Equals(pageSize) ?? false)
            {
                await page.RunAndWaitForRequestFinishedAsync(async () =>
                {
                    await item.ClickAsync();
                },
                new PageRunAndWaitForRequestFinishedOptions()
                {
                    Predicate = request => request.Url.Contains($"{requestUrl}?") && request.Method == "GET"
                });
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Check if it is possible to navigate to next page
    /// </summary>
    /// <returns></returns>
    public async Task<bool> CanNavigateToNext()
    {
        return await this.table.Locator("div.mud-table-pagination div.mud-table-pagination-actions>button").Nth(2).IsEnabledAsync();
    }

    /// <summary>
    /// Navigate to next page and wait for request to finish
    /// </summary>
    /// <param name="requestUrl">Used with RequestFinished predicate</param>
    /// <returns></returns>
    public async Task<bool> NavigateToNextAndWaitForRequestAsync(string requestUrl)
    {
        if (await CanNavigateToNext())
        {
            await this.page.RunAndWaitForRequestFinishedAsync(async () =>
            {
                await this.table.Locator("div.mud-table-pagination div.mud-table-pagination-actions>button").Nth(2).ClickAsync();
            },
            new PageRunAndWaitForRequestFinishedOptions()
            {
                Predicate = request => request.Url.Contains($"{requestUrl}?") && request.Method == "GET"
            });
            return true;
        }
        return false;
    }

    /// <summary>
    /// Check if it is possible to navigate to previous page
    /// </summary>
    /// <returns></returns>
    public async Task<bool> CanNavigateToPrevious()
    {
        return await this.table.Locator("div.mud-table-pagination div.mud-table-pagination-actions>button").Nth(1).IsEnabledAsync();

    }

    /// <summary>
    /// Navigate to previous page and wait for request to finish
    /// </summary>
    /// <param name="requestUrl">Used with RequestFinished predicate</param>
    /// <returns></returns>
    public async Task<bool> NavigateToPreviousAndWaitForRequestAsync(string requestUrl)
    {
        if (await CanNavigateToPrevious())
        {
            await this.page.RunAndWaitForRequestFinishedAsync(async () =>
            {
                await this.table.Locator("div.mud-table-pagination div.mud-table-pagination-actions>button").Nth(1).ClickAsync();
            },
            new PageRunAndWaitForRequestFinishedOptions()
            {
                Predicate = request => request.Url.Contains($"{requestUrl}?") && request.Method == "GET"
            });
            return true;
        }
        return false;
    }
}
