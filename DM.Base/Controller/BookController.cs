using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Safe;
using DM.Base.Entity;
using DM.Base.Service;
using Microsoft.Practices.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DM.Base.Controller
{
    public class BookController : ApiController
    {
        [Dependency]
        public IBookService _IBookService { get; set; }

        [Route("api/v1/book")]
        [HttpGet]
        public PageSource<BookInfo> GetBooks(
            int pagesize = 1000,
            int pageindex = 1,
            int status = 0,
            string category = "",
            string orderby = "ID",
            string orderdirection = "desc",
            string txtfilter = "")
        {
            var param = new PageQueryParam()
            {
                PageSize = pagesize,
                PageIndex = pageindex,
                IsAllowPage = pagesize > 0 && pageindex > 0,
                OrderFiled = orderby,
                IsDesc = orderdirection.ToLower().Equals("desc"),
                TextCondtion = txtfilter,
                FilterCondtion = new Hashtable(),
            };

            param.FilterCondtion.Add("Category", category);
            //param.FilterCondtion.Add("Status", status);

            return _IBookService.GetPagedList(param);
        }

        [Token]
        [Route("api/v1/book")]
        [HttpPost]
        public int AddBook(BookInfo BookInfo)
        {
            BookInfo.CreateUser = int.Parse(base.User.Identity.Name);
            return _IBookService.AddBook(BookInfo);
        }

        [Route("api/v1/book/category")]
        [HttpGet]
        public List<CategoryInfo> GetBookCategorys()
        {
            return _IBookService.GetCategorys();
        }

        [Route("api/v1/book/category")]
        [HttpPost]
        public void AddBookCategory(CategoryInfo Info)
        {
            _IBookService.AddCategory(Info);
        }

        [Route("api/v1/book/category/{Number}")]
        [HttpPut]
        public void UpdateCategory(string Number, CategoryInfo Info)
        {
            _IBookService.UpdateCategory(Number, Info);
        }

        [Route("api/v1/book/category/{Number}")]
        [HttpDelete]
        public void DeleteCategory(string Number)
        {
            _IBookService.DeleteCategory(Number);
        }

        [Route("api/v1/book")]
        [Route("api/v1/book/category")]
        [Route("api/v1/book/category/{Number}")]
        [HttpOptions]
        public void Option()
        {
        }
    }
}
