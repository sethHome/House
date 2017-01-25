using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.User
{
    public class LoginResult
    {
        public UserInfo Account { get; set; }

        public string Token { get; set; }

        /// <summary>
        /// 令牌失效日期
        /// </summary>
        public DateTime InvalidDate { get; set; }

        public int Error { get; set; }
    }
}
