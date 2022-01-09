using System.Runtime.Serialization;

namespace Pixel.Identity.Shared.Request
{
    /// <summary>
    /// Base class for a paged data request
    /// </summary>
    [DataContract]
    public class PagedDataRequest
    {
        protected readonly int maxPageSize = 50;

        private int currentPage;
        /// <summary>
        /// Current page for the request
        /// </summary>
        [DataMember(IsRequired = true)]
        public int CurrentPage
        {
            get => currentPage;
            set
            {
                if (value >= 1)
                {
                    currentPage = value;
                }
            }
        }

        private int pagesSize = 10;
        /// <summary>
        /// Page size for the request
        /// </summary>
        [DataMember(IsRequired = true)]
        public int PageSize
        {
            get
            {
                return pagesSize;
            }
            set
            {
                pagesSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }

        /// <summary>
        /// Number of items to skip
        /// </summary>
        [IgnoreDataMember]
        public int Skip => (CurrentPage - 1) * PageSize;

        /// <summary>
        /// Number of items to take
        /// </summary>
        [IgnoreDataMember]
        public int Take => PageSize;
    }
}
