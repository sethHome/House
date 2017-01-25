using DM.Base.Service;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DM.Base.Controller
{
    public class FondsController : ApiController
    {
        [Dependency]
        public IFondService _IFond { get; set; }

        [Route("api/v1/fonds")]
        [HttpGet]
        public List<FondInfo> GetFonds()
        {
            return _IFond.GetAll();
        }

        [Route("api/v1/fonds")]
        [HttpPost]
        public void AddFonds(FondInfo Fonds)
        {
             _IFond.Add(Fonds);
        }

        [Route("api/v1/fonds")]
        [HttpPut]
        public void UpdateFonds(FondInfo Fonds)
        {
            _IFond.Update(Fonds);
        }

        [Route("api/v1/fonds/{Number}")]
        [HttpDelete]
        public void DeleteFonds(string Number)
        {
            _IFond.Delete(Number);
        }

        [Route("api/v1/fonds/{Number}/check")]
        [HttpGet]
        public bool ChcekFondsNumber(string Number)
        {
            return _IFond.Check(Number);
        }

        [Route("api/v1/fonds")]
        [Route("api/v1/fonds/{Number}")]
        [Route("api/v1/fonds/{Number}/check")]
        [HttpOptions]
        public void Option()
        {
        }

    }
}
