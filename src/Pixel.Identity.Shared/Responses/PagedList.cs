using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Pixel.Identity.Shared.Responses
{
    [DataContract]
    public class PagedList<T>
    {
        /// <summary>
        /// Items for current page
        /// </summary>
        [DataMember]
        public List<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// Total number of available items in database
        /// </summary>
        public int ItemsCount { get; set; }

        /// <summary>
        /// Current Page
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// Desired Item count per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public PagedList()
        {

        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="items">Items for current page</param>
        /// <param name="totalCount">Total number of available items</param>
        /// <param name="currentPage">Index of current page</param>
        /// <param name="pageSize">Item count per page</param>
        public PagedList(IEnumerable<T> items, int totalCount, int currentPage, int pageSize)
        {
            Items.AddRange(items);
            PageSize = pageSize;
            CurrentPage = currentPage;
            ItemsCount = totalCount;
            PageCount = (int)Math.Ceiling(totalCount / (double)pageSize);
        }
    }
}
