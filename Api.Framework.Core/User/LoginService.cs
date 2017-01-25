using Api.Framework.Core.Organization;
using Api.Framework.Core.Safe;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.User
{
    public class LoginService : BaseService<SysUserEntity>, ILogin
    {
        [Dependency]
        public ITokenManager _ITokenManager { get; set; }

        [Dependency]
        public IDepartment _IDepartment { get; set; }

        public LoginResult Login(string Account, string Password)
        {
            var user = base.DB.FirstOrDefault(a => a.Account == Account);

            if (user != null)
            {
                var md5Password = Md5.GetMd5Hash(Password).ToUpper();
                if (user.Password.ToUpper() != md5Password)
                {
                    // 密码错误
                    return new LoginResult()
                    {
                        Error = 2
                    };
                }

                if (!user.Visiable)
                {
                    // 帐号被禁用
                    return new LoginResult()
                    {
                        Error = 3
                    };
                }
                var now = DateTime.Now;
                var tokenDateDuring = 24 * 7; // 一个月

                var token = _ITokenManager.CreateToken(user, tokenDateDuring);

                return new LoginResult()
                {
                    Account = new UserInfo()
                    {
                        ID = user.ID,
                        Account = user.Account,
                        Name = user.Name,
                        PhotoImg = user.PhotoImg,
                        PhotoImgLarge = user.PhotoImgLarge,
                        Dept = _IDepartment.GetUserDept(user.ID)
                    },
                    Token = token,
                    InvalidDate = now.AddHours(tokenDateDuring),
                    Error = 0
                };
            }
            else
            {
                // 帐号不存在
                return new LoginResult()
                {
                    Error = 1
                };
            }
        }
    }
}
