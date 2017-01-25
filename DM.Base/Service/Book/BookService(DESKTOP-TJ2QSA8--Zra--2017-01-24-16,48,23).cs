using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM.Base.Entity;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using System.Linq.Expressions;
using System.Collections;
using Microsoft.Practices.Unity;
using Api.Framework.Core.Config;

namespace DM.Base.Service
{
    public class BookService : IBookService
    {
        private BaseRepository<BookEntity> _DBBook;
        private BaseRepository<BookItemEntity> _DBBookItem;
        private DMContext _DMContext;

        [Dependency]
        public IBaseConfig _IBaseConfig { get; set; }


        public BookService()
        {
            _DMContext = new DMContext();
            _DBBook = new BaseRepository<BookEntity>(_DMContext);
            _DBBookItem = new BaseRepository<BookItemEntity>(_DMContext);
        }

        public PageSource<BookInfo> GetPagedList(PageQueryParam PageParam)
        {
            Expression<Func<BookInfo, bool>> expression = c => true;

            #region Filter
            if (!string.IsNullOrEmpty(PageParam.TextCondtion))
            {
                expression = expression.And(p => p.Name.Contains(PageParam.TextCondtion) || p.Number.Contains(PageParam.TextCondtion) || p.BarCode == PageParam.TextCondtion);
            }

            foreach (DictionaryEntry filter in PageParam.FilterCondtion)
            {
                if (filter.Value == null)
                {
                    continue;
                }

                var val = filter.Value.ToString();

                if (string.IsNullOrEmpty(val))
                {
                    continue;
                }

                switch (filter.Key.ToString())
                {
                    case "Category":
                        {
                            expression = expression.And(c => c.Category == val);
                            break;
                        }
                    default:
                        break;
                }
            }
            #endregion

            var query = from i in this._DMContext.BookItemEntity
                        join b in this._DMContext.BookEntity
                        on i.BookID equals b.ID 
                        
                        select new BookInfo
                        {
                           BookID = i.BookID,
                           Author = b.Author,
                           BackDate = i.BackDate,
                           BarCode = i.BarCode,
                           BorrowOutDate = i.BorrowOutDate,
                           BorrowUser = i.BorrowUser,
                           Category = b.Category,
                           Count = b.Count,
                           CreateDate = b.CreateDate,
                           CreateUser = b.CreateUser,
                           DestroyDate = i.DestroyDate,
                           ItemID = i.ID,
                           LastModifyDate = b.LastModifyDate,
                           LastModifyUser = b.LastModifyUser,
                           Name = b.Name,
                           Note = b.Note,
                           Number = b.Number,
                           Press = b.Press,
                           Price = b.Price,
                           Status = i.Status,
                           Style = b.Style,
                           Type = b.Type,
                           Year = b.Year
                        };

            query = query.Where(expression);

            var result = query.OrderByDescending(a => a.Year).ThenByDescending(a => a.Category).AsEnumerable().ToPagedList(PageParam.PageIndex, PageParam.PageSize);

            PageParam.Count = result.TotalItemCount;

            return new PageSource<BookInfo>()
            {
                Source = result,
                PageCount = result.TotalPageCount,
                TotalCount = result.TotalItemCount
            };
        }

        public int AddBook(BookInfo BookInfo)
        {
            var book = new BookEntity() {
                Author = BookInfo.Author,
                Category = BookInfo.Category,
                Count = BookInfo.Count,
                CreateDate = BookInfo.CreateDate,
                CreateUser = BookInfo.CreateUser,
                LastModifyDate = BookInfo.LastModifyDate,
                LastModifyUser = BookInfo.LastModifyUser,
                Name = BookInfo.Name,
                Note = BookInfo.Note,
                Number = BookInfo.Number,
                Press = BookInfo.Press,
                Price = BookInfo.Price,
                Style = BookInfo.Style,
                Type = BookInfo.Type,
                Year = BookInfo.Year
            };
            book.CreateDate = DateTime.Now;
            _DBBook.Add(book);

            _DBBookItem.Add(new BookItemEntity()
            {
                BookID = book.ID,
                Status = (int)BookStatus.正常,
                BarCode = BookInfo.BarCode

            });

            return book.ID;
        }

        public void DestoryBook(int BookItemID)
        {
            var book = _DBBookItem.Get(BookItemID);

            if (book.Status == (int)BookStatus.正常)
            {
                book.Status = (int)BookStatus.销毁;
                book.DestroyDate = DateTime.Now;

                _DBBookItem.Edit(book);
            }
        }

        public void UpdateBook(BookEntity BookInfo)
        {
            var book = _DBBook.Get(BookInfo);

            book.Number = BookInfo.Number;
            book.Name = BookInfo.Name;

            book.Press = BookInfo.Press;
            book.Type = BookInfo.Type;
            book.Price = BookInfo.Price;
            book.Year = BookInfo.Year;
            book.Author = BookInfo.Author;
            book.Note = BookInfo.Note;

            book.LastModifyDate = DateTime.Now;

            _DBBook.Edit(book);
        }

        public List<CategoryInfo> GetCategorys()
        {
            var key = string.Format("BusinessSystem.{0}.Book.Category.", ConstValue.BusinessKey);

            var nodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(key), key);

            return _GetCategorys(nodes);
        }

        private List<CategoryInfo> _GetCategorys(List<ConfigNode> nodes)
        {
            var result = new List<CategoryInfo>();

            nodes.ForEach(n =>
            {
                var info = new CategoryInfo()
                {
                    Number = n.NodeName,
                    Name = n.NodeValue,
                };

                if (n.ChildNodes.Count > 0)
                {
                    info.Children = _GetCategorys(n.ChildNodes);
                }

                result.Add(info);
            });

            return result;
        }


        public void AddCategory(CategoryInfo Info)
        {
            var key = string.Format("BusinessSystem.{0}.Book.Category.{1}", ConstValue.BusinessKey,string.IsNullOrEmpty(Info.Parent) ? Info.Number.Replace(".", "") : Info.Parent.Trim('.') + "." + Info.Number.Replace(".", ""));

            _IBaseConfig.Add(new ConfigEntity()
            {
                Key = key,
                Value = Info.Name,
                IsDeleted = false,
                Type = "1"
            });
        }

        public void UpdateCategory(string Number, CategoryInfo Info)
        {
            var key = string.Format("BusinessSystem.{0}.Book.Category.{1}.{2}", ConstValue.BusinessKey, Info.Parent, Number);

            var config = _IBaseConfig.GetConfig(key);

            config.Key = string.Format("BusinessSystem.{0}.Book.Category.{1}.{2}", ConstValue.BusinessKey, Info.Parent, Info.Number);
            config.Value = Info.Name;

            _IBaseConfig.Update(config);
        }

        public void DeleteCategory(string FullKey)
        {
            var key = string.Format("BusinessSystem.{0}.Book.Category.{1}", ConstValue.BusinessKey,FullKey);

            _IBaseConfig.Delete(key);
        }
    }
}
