using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Pixel.Identity.Shared.Request
{
    public class UserRequest
    {
        private readonly int maxPageSize = 50;
      
        private int currentPage;
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

        [IgnoreDataMember]
        public int Skip => (CurrentPage - 1) * PageSize;

        [IgnoreDataMember]
        public int Take => PageSize;
    }
}
