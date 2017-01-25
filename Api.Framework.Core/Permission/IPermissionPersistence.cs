using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Permission
{
    public interface IPermissionPersistence
    {
        List<Permission> All(Dictionary<string, object> filters);

        void Create(PermissionEntity Permission);

        void Update(PermissionEntity Permission);

        void Delete(int ID);

        void Delete(string Key);
    }
}
