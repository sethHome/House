using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.User
{
    public interface IUserConfig
    {
        int AddConfig(UserConfigEntity Config);

        void RemoveConfig(int UserID,string ConfigName,string ConfigKey);

        IEnumerable<UserConfigEntity> GetUserConfig(int UserID);

        IEnumerable<UserConfigEntity> GetUserConfig(int UserID,string ConfigName);
    }
}
