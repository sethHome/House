using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Safe
{
    /// <summary>
    /// MD5加密
    /// </summary>
    public class Md5
    {
        /// <summary>
        /// 根据字符串生成MD5
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMd5Hash(string input)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        //计算文件的MD5码

        public static string GetMd5Hash(Stream fileStream)
        {
            byte[] arrbytHashValue;

            var oMD5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();

            arrbytHashValue = oMD5Hasher.ComputeHash(fileStream);
            //计算指定Stream 对象的哈希值
            //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”
            var strHashData = System.BitConverter.ToString(arrbytHashValue);

            //替换-
            return strHashData.Replace("-", "");
        }
    }
}
