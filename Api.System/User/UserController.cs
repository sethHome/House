using Api.Framework.Core;
using Api.Framework.Core.BusinessSystem;
using Api.Framework.Core.Safe;
using Api.Framework.Core.User;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api.System.User
{
    public class UserController : ApiController
    {
        [Dependency]
        public IUser _IUser { get; set; }

        [Dependency]
        public IUserConfig _IUserConfig { get; set; }

        [Dependency]
        public IBusinessSystem _IBusinessSystem { get; set; }


        [Route("api/v1/user")]
        [HttpGet]
        public List<SysUserEntity> GetUsers()
        {
            return _IUser.GetBaseUsers();
        }

        [Route("api/v1/user")]
        [HttpPost]
        public int CreateUsers(UserInfo User)
        {
            return _IUser.Create(User);
        }

        [Route("api/v1/user/{ID}")]
        [HttpPut]
        public void UpdateUser(int ID, UserInfo User)
        {
            _IUser.Update(ID, User);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [Token]
        [Route("api/v1/user/changepassword")]
        [HttpPut]
        public int ChangePassword(UserPasswordInfo Password)
        {
            return _IUser.ChangePassword(int.Parse(base.User.Identity.Name), Password);
        }
        /// <summary>
        /// 设置用户头像
        /// </summary>
        /// <returns></returns>
        [Token]
        [Route("api/v1/user/photo")]
        [HttpPut]
        public void SetUserPhotot(SysUserEntity UserInfo)
        {
            _IUser.SetUserPhotot(int.Parse(base.User.Identity.Name), UserInfo);
        }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/user/{ID}/resetpsw")]
        [HttpPut]
        public void ResetUserPassword(int ID)
        {
            _IUser.ResetUserPassword(ID);
        }

        /// <summary>
        /// 禁用用户
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/user/{ID}/disable")]
        [HttpPut]
        public void DisableUser(int ID)
        {
            _IUser.DisableUser(ID);
        }

        /// <summary>
        /// 启用用户
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/user/{ID}/enable")]
        [HttpPut]
        public void EnableUser(int ID)
        {
            _IUser.EnableUser(ID);
        }

        /// <summary>
        /// 帐号检查
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        [Route("api/v1/user/{Account}/check")]
        [HttpGet]
        public bool AccountCheck(string Account)
        {
            return _IUser.CheckAccount(Account);
        }

        /// <summary>
        /// 添加用户设置
        /// </summary>
        /// <returns></returns>
        [Token]
        [Route("api/v1/user/config")]
        [HttpPost]
        public int AddConfig(UserConfigEntity Info)
        {
            Info.UserID = int.Parse(base.User.Identity.Name);
            return _IUserConfig.AddConfig(Info);
        }

        /// <summary>
        /// 删除用户设置
        /// </summary>
        /// <returns></returns>
        [Token]
        [Route("api/v1/user/config/{Name}/{Key}")]
        [HttpDelete]
        public void RemoveConfig(string Name,string Key)
        {
            _IUserConfig.RemoveConfig(int.Parse(base.User.Identity.Name), Name, Key);
        }

        /// <summary>
        /// 添加用户设置
        /// </summary>
        /// <returns></returns>
        [Token]
        [Route("api/v1/user/config")]
        [HttpGet]
        public object GetUserConfig()
        {
            return _IUserConfig.GetUserConfig(int.Parse(base.User.Identity.Name));
        }

        [Route("api/v1/user/ex")]
        [HttpGet]
        public List<UserInfo> GetUsersEx(bool withdept = true, bool withrole = false, bool withpermission = false, bool withsys = false)
        {
            return _IUser.GetUsers(withdept, withrole, withpermission, withsys);
        }

        [Route("api/v1/user/{ID}/{BusinessKey}/permission")]
        [HttpPut]
        public void SetUserPermission(int ID, string BusinessKey, Dictionary<int, long[]> Permission)
        {
            _IUser.SetPermission(ID, BusinessKey, Permission);
        }

        [Route("api/v1/user/{ID}/production")]
        [HttpPut]
        public void SetUserProduction(int ID, UserProductionInfo Info)
        {
            _IUser.SetFlowTask(ID, Info);
        }

        [Route("api/v1/user/{ID}/business")]
        [HttpGet]
        public List<BusinessSystemInfo> GetUserBisiness(int ID)
        {
            return _IBusinessSystem.GetBusiness(ID);
        }

        [Route("api/v1/user")]
        [Route("api/v1/user/ex")]
        [Route("api/v1/user/config")]
        [Route("api/v1/user/config/{Name}/{Key}")]
        [Route("api/v1/user/{ID}")]
        [Route("api/v1/user/{ID}/disable")]
        [Route("api/v1/user/{ID}/enable")]
        [Route("api/v1/user/{ID}/business")]
        [Route("api/v1/user/{ID}/{BusinessKey}/permission")]
        [Route("api/v1/user/{ID}/production")]
        [Route("api/v1/user/{ID}/resetpsw")]
        [Route("api/v1/user/changepassword")]
        [Route("api/v1/user/photo")]
        [Route("api/v1/user/{Account}/check")]
        [HttpOptions]
        public void Options()
        {
        }
    }
}
