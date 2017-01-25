using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Api.Framework.Core.Safe
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class TokenAttribute : ActionFilterAttribute
    {
        public TokenAttribute()
        {
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var _ITokenManager = UnityContainerHelper.GetServer<ITokenManager>();

            // 如果请求头中不包含 Authorization 内容，则抛出 401 异常，表明要访问的资源需要验证
            if (!actionContext.Request.Headers.Contains("Authorization"))
            {
                throw new Exception("请求头中未包含身份验证信息,拒绝访问");
            }

            // 获取请求头中的验证信息（即token）
            var jwtToken = actionContext.Request.Headers.GetValues("Authorization").First<string>();

            // 验证token
            HttpStatusCode statusCode = _ITokenManager.ValidateToken(jwtToken);

            // 验证通过（返回 http 200）则继续，否则抛出http错误
            if (statusCode == HttpStatusCode.OK)
            {
                base.OnActionExecuting(actionContext);
            }
            else
            {
                throw new Exception("身份验证失败,拒绝访问");
            }
        }
    }
}
