using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Attach;
using Api.Framework.Core.Chat;
using Api.Framework.Core.User;

namespace PM.Base
{   
    /// <summary>
    /// News 服务
    /// </summary>
    public partial class NewsService : INewsService
    {    
		private BaseRepository<NewsEntity> _DB;
		private PMContext _PMContext;

		[Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        [Dependency]
        public WSHandler _NotifySrv { get; set; }

        public NewsService()
        {
			this._PMContext = new PMContext();
            this._DB = new BaseRepository<NewsEntity>(this._PMContext);
        }

		public PageSource<NewsInfo> GetPagedList(PageQueryParam PageParam)
		{
			Expression<Func<NewsEntity, bool>> expression = c => !c.IsDelete;

			#region Filter
			if (!string.IsNullOrEmpty(PageParam.TextCondtion))
            {
                //expression = expression.And(p => p.Name.Contains(PageParam.TextCondtion) || p.Number.Contains(PageParam.TextCondtion));
            }

            foreach (DictionaryEntry filter in PageParam.FilterCondtion)
            {
                var val = filter.Value.ToString();

                if (string.IsNullOrEmpty(val))
                {
                    continue;
                }

                switch (filter.Key.ToString())
                {
                    //case "ID":
                    //    {
                    //        var id = int.Parse(val);
                    //        expression = expression.And(c => c.ID == id);
                    //        break;
                    //    }
                    default:
                        break;
                }
            }
			#endregion

            var pageSource = this._DB.GetOrderPagedList(expression, PageParam);
            var source = new List<NewsInfo>();
           
            foreach (var entity in pageSource)
            {
                source.Add(new NewsInfo(entity)
                {
                    
                });
            }

            return new PageSource<NewsInfo>()
            {
                Source = source,
                PageCount = pageSource.TotalPageCount,
                TotalCount = pageSource.TotalItemCount
            };
		}

		public NewsEntity Get(int ID)
		{
			return this._DB.Get(ID);
		}

		public int Add(NewsInfo News)
		{
			var entity = new NewsEntity(News);
            
            entity.IsDelete = false;
            entity.CreateDate = DateTime.Now;
           
            this._DB.Add(entity);

            _NotifySrv.Send(new
            {
                Head = "新闻公告",
                Title = entity.Title,
                Content = ReplaceHtmlTag(entity.Content,100),
            });

            //foreach (var attachID in News.AttachIDs)
            //{
            //    AddAttach(entity.ID, attachID);
            //}

            return entity.ID;
		}

        private string ReplaceHtmlTag(string html, int length = 0)
        {
            string strText = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", "");
            strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");

            if (length > 0 && strText.Length > length)
                return strText.Substring(0, length);

            return strText;
        }

        public void Update(int ID,NewsEntity News)
		{
			var entity = this._DB.Get(ID);

			entity.SetEntity(News);

			this._DB.Edit(entity);
		}

		public void Delete(int ID)
		{
            var entity = this._DB.Get(ID);

            entity.IsDelete = true;

            this._DB.Edit(entity);
        }

		public void Delete(string IDs)
        {
            var ids = IDs.Split(',');

            foreach (var id in ids)
            {
                this.Delete(int.Parse(id));
            }
        }

		private void AddAttach(int ID, int AttachID)
        {
            _IObjectAttachService.Add(new ObjectAttachEntity()
            {
                ObjectKey = "News",
                ObjectID = ID,
                AttachID = AttachID
            });
        }
		
    } 
}
