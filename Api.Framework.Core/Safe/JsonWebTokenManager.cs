using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.ServiceModel.Security.Tokens;
using System.Threading;
using System.Web;

namespace Api.Framework.Core.Safe
{
    /// <summary>
    /// JWT 代表 JSON Web Token ，它是一种用于认证头部的 token 格式。这个 token 帮你实现了在两个系统之间以一种安全的方式传递信息。出于教学目的，我们暂且把 JWT 作为“不记名 token”。一个不记名 token 包含了三部分：header，payload，signature。
	///    * header 是 token 的一部分，用来存放 token 的类型和编码方式，通常是使用 base-64 编码。
	///    * payload 包含了信息。你可以存放任一种信息，比如用户信息，产品信息等。它们都是使用 base-64 编码方式进行存储。
	///    * signature 包括了 header，payload 和密钥的混合体。密钥必须安全地保存储在服务端。
    /// </summary>
    public class JsonWebTokenManager : ITokenManager
    {
        /// <summary>
        /// 根据用户信息创建 token
        /// </summary>
        /// <param name="account">账户名（暂时存用户ID）</param>
        /// <param name="tokenDateDuring">过期时间（小时）</param>
        /// <returns></returns>
        public string CreateToken(SysUserEntity account, int tokenDateDuring = 24 * 30)
        {
            var now = DateTime.UtcNow;

            // 创建 token描述信息
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // 设置已颁发令牌中包含的输入声明
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.Name, account.ID.ToString()),    // 登录账户
                    new Claim(ClaimTypes.Role, "users")             // 所属角色
                }),

                // 设置用于对令牌进行签名的凭据
                SigningCredentials = new SigningCredentials(
                    new InMemorySymmetricSecurityKey(TokenConfig.SymmetricKey),
                    "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256",
                    "http://www.w3.org/2001/04/xmlenc#sha256"
                ),

                AppliesToAddress = TokenConfig.Audience,

                TokenIssuerName = TokenConfig.Issuer,

                Lifetime = new Lifetime(now, now.AddHours(tokenDateDuring)),

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }


        /// <summary>
        /// 验证 token
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        public HttpStatusCode ValidateToken(string jwtToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // 配置验证参数
            var validationParameters = new TokenValidationParameters()
            {
                IssuerSigningToken = new BinarySecretSecurityToken(TokenConfig.SymmetricKey),
                ValidAudience = TokenConfig.Audience,
                ValidIssuer = TokenConfig.Issuer
            };

            // token过期算法
            validationParameters.LifetimeValidator = new LifetimeValidator((dtFrom, dtTo, t, p) =>
            {
                return dtFrom <= DateTime.UtcNow && DateTime.UtcNow < dtTo;
            });

            try
            {
                // 开始正式验证token
                SecurityToken securityToken = null;
                ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(jwtToken, validationParameters, out securityToken);
                //ClaimsIdentity claimsIdentity = tokenHandler.ValidateToken(securityToken)[0];

                // 验证通过，返回当前请求用户
                Thread.CurrentPrincipal = claimsPrincipal;

                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = claimsPrincipal;
                }

                return HttpStatusCode.OK;
            }
            catch (SecurityTokenValidationException)
            {
                return HttpStatusCode.Unauthorized; // return 401
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError; // return 500
            }
        }
    }
}
