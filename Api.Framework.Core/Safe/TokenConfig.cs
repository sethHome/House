using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Safe
{
    /// <summary>
    /// Token配置参数
    /// </summary>
    public class TokenConfig
    {
        /// <summary>
        /// 数字签名的加密密钥
        /// </summary> 
        public static byte[] SymmetricKey = new byte[32] { 
                123,98,54,34,5,76,12,6,32,31,
                43,42,13,98,92,72,5,81,80,76,
                69,75,146,1,97,231,11,72,35,81,
                157,49
            };

        /// <summary>
        /// 授权者
        /// </summary>
        public static string Audience = "http://www.jinqu.cn";

        /// <summary>
        /// 发行者
        /// </summary>
        public static string Issuer = "GoldSoft";
    }
}
