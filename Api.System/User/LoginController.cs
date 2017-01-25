using Api.Framework.Core;
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
    public class LoginController : ApiController
    {
        [Dependency]
        public ILogin _ILogin { get; set; }

        [Route("api/v1/login/{account}/{password}")]
        [HttpGet]
        public LoginResult Login(string account, string password)
        {
           return _ILogin.Login(account, password);
        }

        [Route("api/v1/login/{account}/{password}")]
        [HttpOptions]
        public void Options()
        {
        }
    }
}
