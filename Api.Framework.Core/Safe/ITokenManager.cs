using System.Net;
namespace Api.Framework.Core.Safe
{
    /// <summary>
    /// Token管理类
    /// </summary>
    public interface ITokenManager
    {
        /// <summary>
        /// 创建Token
        /// </summary>
        /// <param name="account">账户名（暂时存用户ID）</param>
        /// <param name="lifetime">过期时间（小时）</param>
        /// <returns></returns>
        string CreateToken(SysUserEntity account, int lifetime = 24 * 30);

        /// <summary>
        /// 验证 token
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        HttpStatusCode ValidateToken(string jwtToken);
    }
}
