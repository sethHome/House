using Api.Framework.Core.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.BusinessSystem
{
    public interface IBusinessSystem
    {
        List<BusinessSystemInfo> All(bool withuser = false);

        void SetUsers(string BusinessKey, List<SysUserEntity> Users);

        void SetUserBusinesses(SysUserEntity User,List<BusinessSystemInfo> Businesses);

        List<BusinessSystemInfo> GetBusiness(int UserID);
    }
}
