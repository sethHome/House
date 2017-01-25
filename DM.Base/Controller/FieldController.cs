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
    public class FieldController : ApiController
    {
        [Dependency]
        public IFieldService _IFieldService { get; set; }

        [Route("api/v1/field")]
        [HttpGet]
        public List<FieldInfo> GetFields(string key = "")
        {
            var k = string.Format("BusinessSystem.{0}.Field.{1}", ConstValue.BusinessKey, key);

            return _IFieldService.GetFields(k, key);
        }

        [Route("api/v1/field")]
        [HttpPost]
        public int AddField(FieldInfo Field)
        {
            Field.ParentKey = string.Format("BusinessSystem.{0}.Field.{1}", ConstValue.BusinessKey, Field.ParentKey);

            return _IFieldService.AddField(Field);
        }

        [Route("api/v1/field/check")]
        [HttpPost]
        public bool CheckField(FieldInfo Field)
        {
            Field.ParentKey = string.Format("BusinessSystem.{0}.Field.{1}", ConstValue.BusinessKey, Field.ParentKey);
            return _IFieldService.CheckField(Field);
        }

        [Route("api/v1/field")]
        [HttpPut]
        public void UpdateField(FieldInfo Field)
        {
            Field.ParentKey = string.Format("BusinessSystem.{0}.Field.{1}.{2}", ConstValue.BusinessKey, Field.ParentKey, Field.ID);

            _IFieldService.UpdateField(Field);
        }

        [Route("api/v1/field/{Key}/{ID}")]
        [HttpDelete]
        public void DeleteField(string Key,string ID)
        {
            var key = string.Format("BusinessSystem.{0}.Field.{1}.{2}", ConstValue.BusinessKey, Key, ID);

            _IFieldService.DeleteField(key);
        }

        [Route("api/v1/field")]
        [Route("api/v1/field/check")]
        [Route("api/v1/field/{Key}/{ID}")]
        [HttpOptions]
        public void Option()
        {
        }
    }
}
