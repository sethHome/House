using Api.Framework.Core.BaseData;
using Api.Framework.Core.BusinessSystem;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api.System.BaseData
{
    public class EnumController : ApiController
    {
        [Dependency]
        public IEnum _IEnum { get; set; }

        #region enum

        [Route("api/v1/enum")]
        [HttpGet]
        public object All(string system = "")
        {
            if (string.IsNullOrEmpty(system))
            {
                return _IEnum.All();
            }
            else {
                return _IEnum.GetSystemEnum(system);
            }
           
        }

        //[Route("api/v1/enum")]
        //[HttpGet]
        //public List<BusinessSystemInfo> All()
        //{
        //    return _IEnum.All();
        //}

        [Route("api/v1/enum/{Name}")]
        [HttpGet]
        public EnumInfo GetEnum(string Name, string system = "")
        {
            return _IEnum.GetEnumInfo(system, Name);
        }

        [Route("api/v1/enum/{System}")]
        [HttpPost]
        public string AddEnum(string System, EnumInfo Enum)
        {
            return _IEnum.AddEnum(System, Enum);
        }

        [Route("api/v1/enum/{System}")]
        [HttpPut]
        public void EditEnum(string System, EnumInfo Enum)
        {
            _IEnum.EditEnum(System, Enum);
        }

        [Route("api/v1/enum/{Name}/{System}")]
        [HttpDelete]
        public void DeleteEnum(string Name, string System)
        {
            _IEnum.DeleteEnum(System, Name);
        }
        #endregion

        #region item

        [Route("api/v1/enum/{Name}/item/{System}")]
        [HttpPost]
        public long AddEnumItem(string Name, string System, EnumItemInfo Item)
        {
            return _IEnum.AddEnumItem(System, Name, Item);
        }

        [Route("api/v1/enum/{Name}/item/{System}")]
        [HttpPut]
        public void EditEnumItem(string Name, string System, EnumItemInfo Item)
        {
            _IEnum.EditEnumItem(System, Name, Item);
        }

        [Route("api/v1/enum/{Name}/item/{Value}/{System}")]
        [HttpDelete]
        public void DeleteEnumItem(string Name, string Value, string System)
        {
            _IEnum.DeleteEnumItem(System, Name, Value);
        }
        #endregion

        #region tag

        [Route("api/v1/enum/{Name}/item/{Value}/tag/{System}")]
        [HttpPost]
        public void AddEnumItemTag(string Name, string Value, string System, KeyValuePair<string, string> Tag)
        {
            _IEnum.AddEnumItemTag(System, Name, Value, Tag);
        }

        [Route("api/v1/enum/{Name}/item/{Value}/tag/{System}")]
        [HttpPut]
        public void EditEnumItemTag(string Name, string Value, string System, KeyValuePair<string, string> Tag)
        {
            _IEnum.EditEnumItemTag(System, Name, Value, Tag);
        }

        [Route("api/v1/enum/{Name}/item/{Value}/tag/{Key}/{System}")]
        [HttpDelete]
        public void DeleteEnumItemTag(string Name, string Value,string Key, string System)
        {
            _IEnum.DeleteEnumItemTag(System, Name, Value, Key);
        }

        #endregion

        [Route("api/v1/enum")]
        [Route("api/v1/enum/{Name}")]
        [Route("api/v1/enum/{Name}/{System}")]
        [Route("api/v1/enum/{Name}/item/{System}")]
        [Route("api/v1/enum/{Name}/item/{Value}/{System}")]
        [Route("api/v1/enum/{Name}/item/{Value}/tag/{System}")]
        [Route("api/v1/enum/{Name}/item/{Value}/tag/{Key}/{System}")]
        [HttpOptions]
        public void Options2()
        {
        }

    }
}
