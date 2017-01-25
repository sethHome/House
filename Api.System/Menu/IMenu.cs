using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.System.Menu
{
    public interface IMenu
    {
        List<MenuInfo> GetMenus(string BusinessKey, string UserIdentity);

        List<BusinessMenuInfo> GetAllMenus(string UserIdentity);

        void AddMenu(CreateMenuInfo Menu);

        void Remove(string Key);

        void Update(MenuInfo Menu);

        void Favorites(string Key, string UserIdentity);

        void RemoveFavorites(string Key, string UserIdentity);
    }
}
