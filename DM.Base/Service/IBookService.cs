using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using DM.Base.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public interface IBookService
    {
        PageSource<BookInfo> GetPagedList(PageQueryParam PageParam);

        int AddBook(BookInfo BookInfo);

        void UpdateBook(BookEntity BookInfo);

        void DestoryBook(int BookItemID);

        List<CategoryInfo> GetCategorys();

        void AddCategory(CategoryInfo Info);

        void UpdateCategory(string Number, CategoryInfo Info);

        void DeleteCategory(string FullKey);
    }
}
