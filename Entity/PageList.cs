using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmiteRepository.Page;
namespace TaskManager.Entity
{
    public class PageList<T> : PagedList<T>
    {
        public PageList(SmiteRepository.Page.PagedList<T> pList) : base()
        {
            this.DataList = pList.DataList;
            this.PageIndex = pList.PageIndex;
            this.PageSize = pList.PageSize;
            this.Total = pList.Total;
        }
        public PageList() : base()
        {
        }
    }
}
