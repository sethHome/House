
using System.Collections;
using System.Collections.Generic;
using System.Web.Http;

using Microsoft.Practices.Unity;

using Api.Framework.Core;
using Api.Framework.Core.Safe;
using Api.Framework.Core.DBAccess;
using System.Threading.Tasks;

namespace PM.Base
{   
    /// <summary>
    /// EngineeringSpecialty Controlle
    /// </summary>
    public partial class EngineeringSpecialtyController : ApiController
    {   
		[Dependency]
		public IEngineeringSpecialtyService _IEngineeringSpecialtyService { get; set; }
        [Dependency]
        public IEngineeringSpecialtyProvideService _IEngineeringSpecialtyProvideService { get; set; }

        [Token]
        [Route("api/v1/specialty/engineering")]
        [HttpGet]
        public PageSource<EngineeringSpecialtyInfo> All(
			int pagesize = 1000,
            int pageindex = 1,
            string orderby = "EngineeringID",
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
            param.FilterCondtion.Add("Specialty", specialty);
            param.FilterCondtion.Add("Manager", manager);
            param.FilterCondtion.Add("CreateDateFrom", createdatefrom);
            param.FilterCondtion.Add("CreateDateTo", createdateto);
            param.FilterCondtion.Add("DeliveryDateFrom", deliverydatefrom);
            param.FilterCondtion.Add("DeliveryDateTo", deliverydateto);

            return _IEngineeringSpecialtyService.GetPagedList(param);
        }

        [Route("api/v1/specialty/engineering/{ID}")]
        [HttpGet]
        public EngineeringSpecialtyEntity One(int ID)
        {
            return this._IEngineeringSpecialtyService.Get(ID);
        }

        [Route("api/v1/specialty/engineering")]
        [HttpPost]
        public int Create(EngineeringSpecialtyInfo Info)
        {
           return this._IEngineeringSpecialtyService.Add(Info);
        }

		[Route("api/v1/specialty/engineering/{ID}")]
        [HttpPut]
        public void Update(int ID,List<EngineeringSpecialtyEntity> Entity)
        {
            this._IEngineeringSpecialtyService.Update(ID, Entity);
        }

		[Route("api/v1/specialty/engineering/{ID}")]
        [HttpDelete]
        public void Delete(string ID)
        {
			if (ID.Contains(","))
            {
                this._IEngineeringSpecialtyService.Delete(ID);
            }
            else
            {
                this._IEngineeringSpecialtyService.Delete(int.Parse(ID));
            }
        }


        #region 提资，收资
        [Token]
        [Route("api/v1/specialty/provide")]
        [HttpGet]
        public PageSource<EngineeringSpecialtyProvideInfo> AllProvide(
            int pagesize = 1000,
            int pageindex = 1,
            string orderby = "EngineeringID",
            string orderdirection = "desc",
            string txtfilter = "",
            int myreceive = 0,
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

            param.FilterCondtion.Add("MyReceive", myreceive);

            param.CurrentUser = int.Parse(base.User.Identity.Name);

            return _IEngineeringSpecialtyProvideService.GetPagedList(param);
        }

        [Route("api/v1/specialty/provide/{ID}")]
        [HttpGet]
        public EngineeringSpecialtyProvideEntity OneProvide(int ID)
        {
            return this._IEngineeringSpecialtyProvideService.Get(ID);
        }

        [Token]
        [Route("api/v1/specialty/provide")]
        [HttpPost]
        public async Task<int> CreateProvide(EngineeringSpecialtyProvideInfo Info)
        {
            Info.SendUserID = int.Parse(base.User.Identity.Name);
            return await this._IEngineeringSpecialtyProvideService.Add(Info);
        }

        [Route("api/v1/specialty/provide/{ID}")]
        [HttpPut]
        public void UpdateProvide(int ID, EngineeringSpecialtyProvideInfo Info)
        {
            this._IEngineeringSpecialtyProvideService.Update(ID, Info);
        }

        [Route("api/v1/specialty/provide/{ID}")]
        [HttpDelete]
        public void DeleteProvide(string ID)
        {
            if (ID.Contains(","))
            {
                this._IEngineeringSpecialtyProvideService.Delete(ID);
            }
            else
            {
                this._IEngineeringSpecialtyProvideService.Delete(int.Parse(ID));
            }
        }
        #endregion

        [Route("api/v1/specialty/engineering")]
        [Route("api/v1/specialty/provide")]
        
        [Route("api/v1/specialty/engineering/{ID}")]
        [Route("api/v1/specialty/provide/{ID}")]
        [HttpOptions]
        public void Option()
        { }
    } 
}
