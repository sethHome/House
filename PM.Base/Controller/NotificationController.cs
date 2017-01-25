
using System.Collections;
using System.Collections.Generic;
using System.Web.Http;

using Microsoft.Practices.Unity;

using Api.Framework.Core;
using Api.Framework.Core.Safe;
using Api.Framework.Core.DBAccess;
namespace PM.Base
{
    /// <summary>
    /// Notification Controlle
    /// </summary>
    public partial class NotificationController : ApiController
    {
        [Dependency]
        public INotificationService _INotificationService { get; set; }

        [Token]
        [Route("api/v1/notification")]
        [HttpGet]
        public PageSource<NotificationInfo> All(
            int pagesize = 1000,
            int pageindex = 1,
            string orderby = "EffectDate",
            string orderdirection = "desc",
            string txtfilter = "",
            int read = 0,
            int effect = 0)
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

            param.FilterCondtion.Add("User", base.User.Identity.Name);
            param.FilterCondtion.Add("Effect", effect);
            param.FilterCondtion.Add("Read", read);

            return _INotificationService.GetPagedList(param);
        }

        [Route("api/v1/notification/{ID}")]
        [HttpGet]
        public NotificationEntity One(int ID)
        {
            return this._INotificationService.Get(ID);
        }

        [Route("api/v1/notification")]
        [HttpPost]
        public int Create(NotificationInfo Info)
        {
            return this._INotificationService.Add(Info);
        }

        [Route("api/v1/notification/{ID}")]
        [HttpPut]
        public void Update(int ID, NotificationEntity Entity)
        {
            Entity.ID = ID;
            this._INotificationService.Update(Entity);
        }

        [Route("api/v1/notification/{ID}/read")]
        [HttpPut]
        public void Read(int ID)
        {
            this._INotificationService.Read(ID);
        }

        [Route("api/v1/notification/{ID}")]
        [HttpDelete]
        public void Delete(string ID)
        {
            if (ID.Contains(","))
            {
                this._INotificationService.Delete(ID);
            }
            else
            {
                this._INotificationService.Delete(int.Parse(ID));
            }
        }
        
        [Route("api/v1/notification")]
        [Route("api/v1/notification/{ID}")]
        [Route("api/v1/notification/{ID}/read")]
        [HttpOptions]
        public void Option()
        { }

    }
}
