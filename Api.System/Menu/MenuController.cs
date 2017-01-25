using Api.Framework.Core.Safe;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api.System.Menu
{
    public class MenuController : ApiController
    {
        [Dependency]
        public IMenu _IMenu { get; set; }

        [Token]
        [Route("api/v1/menu")]
        [HttpGet]
        public List<MenuInfo> GetMenus(string business = "")
        {
            return _IMenu.GetMenus(business, base.User.Identity.Name);
        }
        [Token]
        [Route("api/v1/menu/all")]
        [HttpGet]
        public List<BusinessMenuInfo> GetAllMenus()
        {
            return _IMenu.GetAllMenus(base.User.Identity.Name);
        }

        [Route("api/v1/menu")]
        [HttpPost]
        public void AddModule(CreateMenuInfo Module)
        {
            _IMenu.AddMenu(Module);
        }

        [Route("api/v1/menu")]
        [HttpDelete]
        public void RemoveModule(string Key = "")
        {
            _IMenu.Remove(Key);
        }

        [Route("api/v1/menu")]
        [HttpPut]
        public void RemoveModule(MenuInfo Module)
        {
            _IMenu.Update(Module);
        }

        /// <summary>
        /// 用户收藏该菜单
        /// </summary>
        /// <param name="Module"></param>
        [Token]
        [Route("api/v1/menu/{Key}/favorites")]
        [HttpPut]
        public void FavoritesMenu(string Key)
        {
            _IMenu.Favorites(Key,base.User.Identity.Name);
        }

        /// <summary>
        /// 用户取消收藏该菜单
        /// </summary>
        /// <param name="Module"></param>
        [Token]
        [Route("api/v1/menu/{Key}/favorites")]
        [HttpDelete]
        public void RemoveFavoritesMenu(string Key)
        {
            _IMenu.RemoveFavorites(Key, base.User.Identity.Name);
        }

        [Route("api/v1/menu")]
        [Route("api/v1/menu/all")]
        [Route("api/v1/menu/{ID}/favorites")]
        [HttpOptions]
        public void Options()
        {
        }
    }
}
