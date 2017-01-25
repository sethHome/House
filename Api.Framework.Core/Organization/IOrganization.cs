using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Organization
{
    public interface IOrganization
    {
        void Delete(string Key);

        void Delete(int ID);

        void ReName(string Key,string Name);

        void AddUser(int UserID);

        void RemoveUser(int UserID);

        void AddPermission(int Index, long Value);

        void RemovePermission(int Index, long Value);

        void SetPermissions(string DeptKey, string BusinessKey, Dictionary<int, long[]> Permission);

        void SetUsers(string DeptKey, List<SysUserEntity> Users);

        
    }
}
