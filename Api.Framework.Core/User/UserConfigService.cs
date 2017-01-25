using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.User
{
    public class UserConfigService : BaseService<UserConfigEntity>,IUserConfig
    {
        public int AddConfig(UserConfigEntity Config)
        {
            if (string.IsNullOrEmpty(Config.ConfigName))
            {
                return 1;
            }

            if (string.IsNullOrEmpty(Config.ConfigKey))
            {
                return 2;
            }

            if (base.DB.Count(c => c.UserID == Config.UserID && c.ConfigName == Config.ConfigName && c.ConfigKey == Config.ConfigKey) > 0)
            {
                return 3;
            }

            base.DB.Add(Config);

            return 0;
        }

        public IEnumerable<UserConfigEntity> GetUserConfig(int UserID)
        {
            return base.DB.GetList(c => c.UserID == UserID);
        }

        public IEnumerable<UserConfigEntity> GetUserConfig(int UserID, string ConfigName)
        {
            return base.DB.GetList(c => c.UserID == UserID && c.ConfigName == ConfigName);
        }

        public void RemoveConfig(int ID)
        {
             base.DB.Delete(ID);
        }

        public void RemoveConfig(int UserID, string ConfigName, string ConfigKey)
        {
            base.DB.Delete(c => c.UserID == UserID && c.ConfigName == ConfigName && c.ConfigKey == ConfigKey);
        }
    }
}
