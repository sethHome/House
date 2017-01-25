using Api.Framework.Core;
using Api.Framework.Core.Safe;

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Service
{
    /// <summary>
    /// Token验证辅助类
    /// 
    /// 整个验证过程
    /// 1. 登录系统（index页面）,系统默认路由到 start app, 路由前触发 $stateChangeStart 事件，在该事件中验证登录，不通过跳转到登录界面（Login页面）。
    /// 2. 在登录界面（Login页面）填写帐户密码确认，前台发送 登录请求 到 GET：api/v1/login/{account}/{password}。
    /// 3. 登录请求被服务程序中的 Global.asax 中的 MessageHandlers 拦截，但由于是 登录请求，所以直接放行，不验证token。
    /// 4. 登录成功，重新路由到 start app，同样触发 $stateChangeStart 事件（index.js中)，在该事件中验证登录，此时应该验证通过，那么系统继续载入 start app 的页面或api请求。
    /// 5. 在请求触发前触发 自定义的 http 请求拦截器 AuthInterceptor（index.js中)中的 request 方法中，把本地token（暂时存在cookie中），写入http请求head中。
    /// 6. 请求被服务程序中的 Global.asax 中的 MessageHandlers 拦截，由于不是 登录请求，所以必须验证请求head中的token。
    /// 7. 如果验证通过则返回正确结果和 HttpStatusCode 200，如果验证不通过返回 HttpStatusCode 401，表明请求必须要验证。
    /// 8. 如果请求验证通过，页面继续载入相关内容，如果验证失败会触发 自定义的 http 请求拦截器 AuthInterceptor（index.js中)中的 responseError 方法中,并跳转到登录界面（login页面）。
    /// 
    /// 简单讲就是
    /// *. 在路由的 $stateChangeStart 事件中只是简单的判断本地是否有当前登录用户信息，有的话就算验证通过。
    /// *. 在每次像 Api Service 请求 http 数据时，则通过自定义的 http 请求拦截器 AuthInterceptor，发送token，并拦截http错误，如果是 401 错误则退回登录页面。
    /// </summary>
    internal class TokenValidationHandler : DelegatingHandler
    {
        /// <summary>
        /// token管理工具
        /// </summary>
        public ITokenManager _ITokenManager {get; set;}

        /// <summary>
        /// 构造函数
        /// </summary>
        public TokenValidationHandler()
        {
            _ITokenManager = UnityContainerHelper.GetServer<ITokenManager>();
        }

        /// <summary>
        /// 以异步操作发送 HTTP 请求到内部管理器以发送到服务器
        /// （重写该方法加入，加入token验证）
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 排除不验证token的地址
            if (request.RequestUri.LocalPath.Contains("/api/v1/login")
             || request.RequestUri.LocalPath.Contains("/api/v1/download"))
            {
                return base.SendAsync(request, cancellationToken);
            }

            // 如果请求头中不包含 Authorization 内容，则抛出 401 异常，表明要访问的资源需要验证
            if (!request.Headers.Contains("Authorization"))
            {
                return Task.FromResult<HttpResponseMessage>(new HttpResponseMessage(HttpStatusCode.Unauthorized));
            }

            // 获取请求头中的验证信息（即token）
            var jwtToken = request.Headers.GetValues("Authorization").First<string>();

            // 验证token
            HttpStatusCode statusCode = _ITokenManager.ValidateToken(jwtToken);

            // 验证通过（返回 http 200）则继续，否则抛出http错误
            if (statusCode == HttpStatusCode.OK) {
                return base.SendAsync(request, cancellationToken);
            }
            else
                return Task.FromResult<HttpResponseMessage>(new HttpResponseMessage(statusCode));
            }
    }
}