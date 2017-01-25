
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
    /// Car Controlle
    /// </summary>
    public partial class CarController : ApiController
    {
        [Dependency]
        public ICarService _ICarService { get; set; }

        [Dependency]
        public ICarUseService _ICarUseService { get; set; }

        [Route("api/v1/car")]
        [HttpGet]
        public PageSource<CarInfo> All(
            int pagesize = 1000,
            int pageindex = 1,
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


            return _ICarService.GetPagedList(param);
        }

        [Route("api/v1/car/use")]
        [HttpGet]
        public PageSource<CarUseInfo> AllUse(
            int pagesize = 1000,
            int pageindex = 1,
            int myapply = 0,
            int car = 0,
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

            param.FilterCondtion.Add("MyApply", myapply);
            param.FilterCondtion.Add("Car", car);

            return _ICarUseService.GetPagedList(param);
        }

        [Route("api/v1/car/used")]
        [HttpGet]
        public List<CarEntity> GetMyUsedCar(int user = 0)
        {
            return this._ICarService.GetMyUsedCar(user);
        }

        [Route("api/v1/car/{ID}")]
        [HttpGet]
        public CarEntity One(int ID)
        {
            return this._ICarService.Get(ID);
        }

        [Route("api/v1/car/use/{ID}")]
        [HttpGet]
        public CarUseEntity CarUse(int ID)
        {
            return this._ICarUseService.Get(ID);
        }

        [Route("api/v1/car")]
        [HttpPost]
        public int Create(CarInfo Info)
        {
            return this._ICarService.Add(Info);
        }

        [Route("api/v1/car/{ID}")]
        [HttpPut]
        public void Update(int ID, CarEntity Entity)
        {
            this._ICarService.Update(ID, Entity);
        }

        [Route("api/v1/car/{ID}/maintain")]
        [HttpPut]
        public int Maintain(int ID)
        {
            this._ICarService.ChangeStatus(ID, CarStatus.维修保养);
            return (int)CarStatus.维修保养;
        }

        [Route("api/v1/car/{ID}/scrap")]
        [HttpPut]
        public int Scrap(int ID)
        {
            this._ICarService.ChangeStatus(ID, CarStatus.报废);
            return (int)CarStatus.报废;
        }

        [Route("api/v1/car/{ID}/normal")]
        [HttpPut]
        public int Normal(int ID)
        {
            this._ICarService.ChangeStatus(ID, CarStatus.正常);
            return (int)CarStatus.正常;
        }

        [Route("api/v1/car/{ID}")]
        [HttpDelete]
        public void Delete(string ID)
        {
            if (ID.Contains(","))
            {
                this._ICarService.Delete(ID);
            }
            else
            {
                this._ICarService.Delete(int.Parse(ID));
            }
        }

        [Token]
        [Route("api/v1/car/{ID}/apply")]
        [HttpPost]
        public async Task<int> Apply(int ID, CarUseInfo ApplyInfo)
        {
            ApplyInfo.CarID = ID;
            ApplyInfo.Manager = int.Parse(base.User.Identity.Name);
            return await this._ICarUseService.Add(ApplyInfo);
        }

        [Route("api/v1/car")]
        [Route("api/v1/car/use")]
        [Route("api/v1/car/use/{ID}")]
        [Route("api/v1/car/used")]
        [Route("api/v1/car/{ID}")]
        [Route("api/v1/car/{ID}/use")]
        [Route("api/v1/car/{ID}/maintain")]
        [Route("api/v1/car/{ID}/scrap")]
        [Route("api/v1/car/{ID}/normal")]
        [Route("api/v1/car/{ID}/apply")]
        [HttpOptions]
        public void Option()
        { }

    }
}
