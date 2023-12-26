using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Task17_Consumewebapiinmvc_.Models
{
    public class PageInfo
    {
        public int Totalitems { get; set; }
        public int ItemsperPage { get; set; }
        public int currentPage { get; set; }
        public PageInfo()
        {
            currentPage = 1;
        }
        public int PageStart
        {
            get { return (currentPage - 1) * ItemsperPage + 1; }
        }
        public int PageEnd
        {
            get
            {
                int currentTotal = (currentPage - 1) * ItemsperPage + ItemsperPage;
                return (currentTotal < Totalitems ? currentTotal : Totalitems);
            }
        }
        public int LastPage
        {
            get { return (int)Math.Ceiling((decimal)Totalitems / ItemsperPage); }
        }
    }   
    
}
