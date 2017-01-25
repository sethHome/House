
using System.Collections;
using System.Collections.Generic;
using System.Web.Http;

using Microsoft.Practices.Unity;

using Api.Framework.Core;
using Api.Framework.Core.Safe;
using Api.Framework.Core.DBAccess;
using System.Threading.Tasks;
using Api.Framework.Core.Attach;

namespace PM.Base
{
    /// <summary>
    /// EngineeringVolume Controlle
    /// </summary>
    public partial class EngineeringVolumeController : ApiController
    {
        [Dependency]
        public IEngineeringVolumeService _IEngineeringVolumeService { get; set; }

        [Token]
        [Route("api/v1/volume/plan")]
        [HttpGet]
        public PageSource<EngineeringVolumeInfo> GetVolumePlanPagedList(
            int pagesize = 1000,
            int pageindex = 1,
            string orderby = "ID",
            string orderdirection = "desc",
            string txtfilter = "",
            int type = 0, int phase = 0, int vollevel = 0, int tasktype = 0, int status = 0, int manager = 0,long specialty = 0,
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
                FilterCondtion = new Hashtable(),
                CurrentUser = int.Parse(base.User.Identity.Name)
            };

            param.FilterCondtion.Add("Type", type);
            param.FilterCondtion.Add("Phase", phase);
            param.FilterCondtion.Add("VolLevel", vollevel);
            param.FilterCondtion.Add("TaskType", tasktype);
            param.FilterCondtion.Add("Status", status);
            param.FilterCondtion.Add("Manager", manager);
            param.FilterCondtion.Add("Specialty", specialty);
            param.FilterCondtion.Add("CreateDateFrom", createdatefrom);
            param.FilterCondtion.Add("CreateDateTo", createdateto);
            param.FilterCondtion.Add("DeliveryDateFrom", deliverydatefrom);
            param.FilterCondtion.Add("DeliveryDateTo", deliverydateto);

            return _IEngineeringVolumeService.GetVolumePlanPagedList(param);
        }

        [Token]
        [Route("api/v1/volume/process")]
        [HttpGet]
        public PageSource<EngineeringVolumeInfo> GetVolumeProcssPageList(
            int pagesize = 1000,
            int pageindex = 1,
            string orderby = "ID",
            string orderdirection = "desc",
            string txtfilter = "",
            int type = 0, int phase = 0, int vollevel = 0, int tasktype = 0, int status = 0, int manager = 0, long specialty = 0,
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
                FilterCondtion = new Hashtable(),
                CurrentUser = int.Parse(base.User.Identity.Name)
            };

            param.FilterCondtion.Add("Type", type);
            param.FilterCondtion.Add("Phase", phase);
            param.FilterCondtion.Add("VolLevel", vollevel);
            param.FilterCondtion.Add("TaskType", tasktype);
            param.FilterCondtion.Add("Status", status);
            param.FilterCondtion.Add("Manager", manager);
            param.FilterCondtion.Add("Specialty", specialty);
            param.FilterCondtion.Add("CreateDateFrom", createdatefrom);
            param.FilterCondtion.Add("CreateDateTo", createdateto);
            param.FilterCondtion.Add("DeliveryDateFrom", deliverydatefrom);
            param.FilterCondtion.Add("DeliveryDateTo", deliverydateto);

            return _IEngineeringVolumeService.GetVolumeProcssPageList(param);
        }

        [Route("api/v1/volume/engineering/{ID}")]
        [HttpGet]
        public EngineeringVolumeEntity One(int ID)
        {
            return this._IEngineeringVolumeService.Get(ID);
        }

        [Token]
        [Route("api/v1/volume")]
        [HttpPost]
        public async Task<int> Create(EngineeringVolumeNewInfo Info)
        {
            return await this._IEngineeringVolumeService.Create(int.Parse(base.User.Identity.Name),Info);
        }

        [Token]
        [Route("api/v1/volume/batch")]
        [HttpPost]
        public async Task<List<int>> BatchCreate(List<EngineeringVolumeNewInfo> Infos)
        {
            var result = new List<int>();
            var userID = int.Parse(base.User.Identity.Name);

            foreach (var item in Infos)
            {
                result.Add(await this._IEngineeringVolumeService.Create(userID, item));
            }

            return result;
        }

        [Route("api/v1/volume/{ID}")]
        [HttpPut]
        public void Update(int ID, EngineeringVolumeNewInfo Entity)
        {
            this._IEngineeringVolumeService.Update(ID, Entity);
        }

        [Route("api/v1/volume/{ID}")]
        [HttpDelete]
        public void Delete(string ID)
        {
            if (ID.Contains(","))
            {
                this._IEngineeringVolumeService.Delete(ID);
            }
            else
            {
                this._IEngineeringVolumeService.Delete(int.Parse(ID));
            }
        }

        [Route("api/v1/volume/engineering/{EngineeringID}/specialty/{SpecialtyID}")]
        [HttpGet]
        public List<EngineeringVolumeInfo> GetSpecialtyVolumes(int EngineeringID, long SpecialtyID)
        {
            return this._IEngineeringVolumeService.GetSpecialtyVolumes(EngineeringID, SpecialtyID);
        }

        [Token]
        [Route("api/v1/volume/engineering/{EngineeringID}/specialty/{SpecialtyID}")]
        [HttpPut]
        public async Task<List<EngineeringVolumeEntity>> BatchUpdate(int EngineeringID, long SpecialtyID, List<EngineeringVolumeNewInfo> Entitys)
        {
            return await this._IEngineeringVolumeService.BatchUpdate(int.Parse(base.User.Identity.Name), EngineeringID, SpecialtyID, Entitys);
        }

        

        [Route("api/v1/volume/{ID}/file")]
        [HttpGet]
        public List<SysAttachFileEntity> GetVolumeFiles(int ID)
        {
            return this._IEngineeringVolumeService.GetVolumeFiles(ID);
        }

        /// <summary>
        /// 卷册的统计信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Route("api/v1/volume/{ID}/statistics")]
        [HttpGet]
        public VolumeStatisticsInfo GetVolumeStatisticsInfo(int ID)
        {
            return this._IEngineeringVolumeService.GetVolumeStatisticsInfo(ID);
        }

        [Route("api/v1/volume")]
        [Route("api/v1/volume/batch")]
        [Route("api/v1/volume/{ID}")]
        [Route("api/v1/volume/plan")]
        [Route("api/v1/volume/process")]
        [Route("api/v1/volume/engineering")]
        [Route("api/v1/volume/{ID}/file")]
        [Route("api/v1/volume/engineering/{ID}")]
        [Route("api/v1/volume/engineering/{EngineeringID}/specialty/{SpecialtyID}")]
        [Route("api/v1/volume/{ID}/statistics")]
        [HttpOptions]
        public void Option()
        { }
    }
}
