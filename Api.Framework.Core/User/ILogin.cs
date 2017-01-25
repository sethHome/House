using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.User
{
    public interface ILogin
    {
        LoginResult Login(string Account, string Password);
    }
}
