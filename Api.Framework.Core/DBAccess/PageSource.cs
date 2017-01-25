using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.DBAccess
{
    public class PageSource<T>
    {
        public PageSource()
        {
        }

        public PageSource(PageList<T> list)
        {
            this.Source = list;
            this.TotalCount = list.TotalItemCount;
            this.PageCount = list.TotalPageCount;
        }

        public IEnumerable<T> Source { get; set; }

        public DataTable TableSource { get; set; }

        public int TotalCount { get; set; }

        public int PageCount { get; set; }

    }

    public class TableSource
    {
        public TableSource(DataTable table, int TotalCount, int PageSize)
        {
            this.Source = table;
            this.TotalCount = TotalCount;
            if (PageSize > 0)
            {
                this.PageCount = TotalCount / PageSize + (TotalCount % PageSize == 0 ? 0 : 1);
            }
        }

        public DataTable Source { get; set; }

        public int TotalCount { get; set; }

        public int PageCount { get; set; }
    }
}
