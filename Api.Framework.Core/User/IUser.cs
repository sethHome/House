using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.User
{
    public interface IUser
    {
        List<UserInfo> GetUsers(bool WithDept = false,bool WithRole = false, bool WithPermission = false, bool WithBusiness = false);

        List<SysUserEntity> GetBaseUsers();

        int ChangePassword(int ID, UserPasswordInfo PaswordInfo);

        void SetUserPhotot(int ID, SysUserEntity UserInfo);

        void ResetUserPassword(int ID);

        void DisableUser(int ID);

        void EnableUser(int ID);

        void Update(int ID,UserInfo User);

        int Create(UserInfo User);

        bool CheckAccount(string Account);

        Dictionary<int, long> GetPermissions(string UserIdentity);

        void SetPermission(int UserID, string BusinessKey, Dictionary<int, long[]> Permission);

        void SetFlowTask(int ID, UserProductionInfo Info);

        
    }
}
