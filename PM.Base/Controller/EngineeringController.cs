
using System.Collections;
using System.Collections.Generic;
using System.Web.Http;

using Microsoft.Practices.Unity;

using Api.Framework.Core;
using Api.Framework.Core.Safe;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.User;

namespace PM.Base
{
    /// <summary>
    /// Engineering Controlle
    /// </summary>
    public partial class EngineeringController : ApiController
    {
        [Dependency]
        public IEngineeringService _IEngineeringService { get; set; }

        [Dependency]
        public IEngineeringSpecialtyService _IEngineeringSpecialtyService { get; set; }

        [Dependency]
        public IEngineeringNoteService _IEngineeringNoteService { get; set; }


        [Token]
        [Route("api/v1/engineering")]
        [HttpGet]
        public PageSource<EngineeringInfo> All(
            int pagesize = 1000,
            int pageindex = 1,
            string orderby = "ID",
            string orderdirection = "desc",
            string txtfilter = "",
            string ids = "",
            bool trash = false,
            int type = 0, int phase = 0, int vollevel = 0, int tasktype = 0, int status = 0, int manager = 0,
            string createdatefrom = "", string createdateto = "",
            string deliverydatefrom = "", string deliverydateto = "")
        {
            var param = new PageQueryParam()
            {
                PageSize = pagesize,
                PageIndex = pageindex,
                IsAllowPage = pagesize > 0 && pageindex > 0,
                OrderFiled = orderby,
                IsDesc = orderdirection.ToLower().Equals("desc"),
                TextCondtion = txtfilter,
                IsDelete = trash,
                FilterCondtion = new Hashtable(),
                CurrentUser = int.Parse(base.User.Identity.Name)
            };

            param.FilterCondtion.Add("IDs", ids);
            param.FilterCondtion.Add("Type", type);
            param.FilterCondtion.Add("Phase", phase);
            param.FilterCondtion.Add("VolLevel", vollevel);
            param.FilterCondtion.Add("TaskType", tasktype);
            param.FilterCondtion.Add("Status", status);
            param.FilterCondtion.Add("Manager", manager);
            param.FilterCondtion.Add("CreateDateFrom", createdatefrom);
            param.FilterCondtion.Add("CreateDateTo", createdateto);
            param.FilterCondtion.Add("DeliveryDateFrom", deliverydatefrom);
            param.FilterCondtion.Add("DeliveryDateTo", deliverydateto);

            return _IEngineeringService.GetPagedList(param);
        }

        [Route("api/v1/engineering/source")]
        [HttpGet]
        public List<EngineeringEntity> Source(string txtfilter = "",
            string exceptids = "", string name = "", string number = "", int count = 10)
        {
            var condition = new Dictionary<string, string>();
            condition.Add("TxtFilter", txtfilter);
            condition.Add("ExceptIds", exceptids);
            condition.Add("Number", number);
            condition.Add("Name", name);

            return this._IEngineeringService.GetSource(count, condition);
        }

        [Route("api/v1/engineering/{ID}")]
        [HttpGet]
        public EngineeringInfo One(int ID)
        {
            return this._IEngineeringService.Get(ID);
        }

        [Route("api/v1/engineering")]
        [HttpPost]
        public int Create(EngineeringInfo Info)
        {
            return this._IEngineeringService.Add(Info);

        }

        [Route("api/v1/engineering/{ID}")]
        [HttpPut]
        public void Update(int ID, EngineeringEntity Entity)
        {
            this._IEngineeringService.Update(ID, Entity);
        }

        [Route("api/v1/engineering/{IDs}/backup")]
        [HttpDelete]
        public void BackUp(string IDs)
        {
            this._IEngineeringService.BackUp(IDs);
        }

        [Route("api/v1/engineering/{ID}")]
        [HttpDelete]
        public void Delete(string ID)
        {
            if (ID.Contains(","))
            {
                this._IEngineeringService.Delete(ID);
            }
            else
            {
                this._IEngineeringService.Delete(int.Parse(ID));
            }
        }

        [Token]
        [Route("api/v1/engineering/{ID}/stop")]
        [HttpPut]
        public void Stop(int ID, EngineeringNoteInfo Reason)
        {
            this._IEngineeringService.Stop(ID, int.Parse(base.User.Identity.Name), Reason.Content, Reason.ReceiveUsers);
        }

        [Token]
        [Route("api/v1/engineering/{ID}/start")]
        [HttpPut]
        public void Start(int ID, EngineeringNoteInfo Reason)
        {
            this._IEngineeringService.Start(ID, int.Parse(base.User.Identity.Name));
        }

        [Token]
        [Route("api/v1/engineering/{ID}/follow")]
        [HttpPut]
        public int Follow(int ID)
        {
            return _IEngineeringService.Follow(ID, int.Parse(base.User.Identity.Name));
        }

        [Token]
        [Route("api/v1/engineering/{ID}/unfollow")]
        [HttpPut]
        public void UnFollow(int ID)
        {
            _IEngineeringService.UnFollow(ID, int.Parse(base.User.Identity.Name));
        }

        [Token]
        [Route("api/v1/engineering/{ID}/isfollow")]
        [HttpPut]
        public bool IsFollow(int ID)
        {
            return _IEngineeringService.IsFollow(ID, int.Parse(base.User.Identity.Name));
        }

        /// <summary>
        /// 工程的甘特图
        /// </summary>
        [Token]
        [Route("api/v1/engineering/{ID}/gantt")]
        [HttpGet]
        public List<EngineeringGanttInfo> GetEngineeringGantt(int ID)
        {
            return _IEngineeringService.GetEngineeringGantt(ID, int.Parse(User.Identity.Name));
        }

        [Route("api/v1/engineering/{ID}/specialty/{SpecialtyID}/file")]
        [HttpGet]
        public List<EngineeringVolumeFileInfo> GetEngineeringSpecilAttachs(int ID, long SpecialtyID, int user = 0)
        {
            return _IEngineeringSpecialtyService.GetSpecialtyAttachs(ID, SpecialtyID, user);
        }

        [Route("api/v1/engineering/{ID}/specialty/{SpecialtyID}/volume")]
        [HttpGet]
        public List<EngineeringVolumeEntity> GetEngineeringSpecilVolume(int ID, long SpecialtyID)
        {
            return _IEngineeringSpecialtyService.GetSpecialtyVolumes(ID, SpecialtyID);
        }

        /// <summary>
        /// 获取工程策划的专业信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Route("api/v1/engineering/{ID}/specialty")]
        [HttpGet]
        public List<EngineeringSpecialtyEntity> GetEngineeringSpecil(int ID)
        {
            return _IEngineeringSpecialtyService.GetEngineeringSpecialtyValue(ID);
        }

        [Route("api/v1/engineering/{ID}/note")]
        [HttpGet]
        public List<EngineeringNoteEntity> GetEngineeringNote(int ID)
        {
            return _IEngineeringNoteService.GetEngineeringNote(ID);
        }

        [Route("api/v1/engineering")]
        [Route("api/v1/engineering/{ID}")]
        [Route("api/v1/engineering/source")]
        [Route("api/v1/engineering/{IDs}/backup")]
        [Route("api/v1/engineering/{ID}/stop")]
        [Route("api/v1/engineering/{ID}/start")]
        [Route("api/v1/engineering/{ID}/gantt")]
        [Route("api/v1/engineering/{ID}/follow")]
        [Route("api/v1/engineering/{ID}/unfollow")]
        [Route("api/v1/engineering/{ID}/isfollow")]
        [Route("api/v1/engineering/{ID}/note")]
        [Route("api/v1/engineering/{ID}/specialty")]
        [Route("api/v1/engineering/{ID}/specialty/{SpecialtyID}/file")]
        [Route("api/v1/engineering/{ID}/specialty/{SpecialtyID}/volume")]
        [HttpOptions]
        public void Options()
        {
        }

    }
}
