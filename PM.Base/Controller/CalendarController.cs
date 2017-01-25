
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
    /// Calendar Controlle
    /// </summary>
    public partial class CalendarController : ApiController
    {
        [Dependency]
        public ICalendarService _ICalendarService { get; set; }

        [Token]
        [Route("api/v1/calendar")]
        [HttpGet]
        public PageSource<CalendarEntity> All(
            int pagesize = 1000,
            int pageindex = 1,
            string orderby = "StartTime",
            string orderdirection = "asc",
            string txtfilter = "",
            bool today = false,
            int type = 0, int self = 0, int user = 0)
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

            param.FilterCondtion.Add("Type", type);

            if (today)
            {
                param.FilterCondtion.Add("Today", today);
            }
            
            if (user > 0)
            {
                param.FilterCondtion.Add("User", user);
            }
            else if (self > 0)
            {
                param.FilterCondtion.Add("User", base.User.Identity.Name);
            }

            return _ICalendarService.GetPagedList(param);
        }

        [Route("api/v1/calendar/{ID}")]
        [HttpGet]
        public CalendarEntity One(int ID)
        {
            return this._ICalendarService.Get(ID);
        }

        [Token]
        [Route("api/v1/calendar")]
        [HttpPost]
        public int Create(CalendarInfo Info)
        {
            Info.UserID = int.Parse(base.User.Identity.Name);
            Info.CreateUser = Info.UserID;
            return this._ICalendarService.Add(Info);
        }

        [Route("api/v1/calendar/{ID}")]
        [HttpPut]
        public void Update(int ID, CalendarEntity Entity)
        {
            this._ICalendarService.Update(ID, Entity);
        }

        [Route("api/v1/calendar/{ID}")]
        [HttpDelete]
        public void Delete(string ID)
        {
            if (ID.Contains(","))
            {
                this._ICalendarService.Delete(ID);
            }
            else
            {
                this._ICalendarService.Delete(int.Parse(ID));
            }
        }

        [Route("api/v1/calendar")]
        [Route("api/v1/calendar/{ID}")]
        [HttpOptions]
        public void Option()
        { }

    }
}
