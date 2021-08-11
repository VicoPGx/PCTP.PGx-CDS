using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.App.Utility.Helpers
{
    public static class XorEncryptor
    {
        #region 加解密
        private const string KEY = "VicoSoft";            // 密钥

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string Encrypt(string txt)
        {
            try
            {
                StringBuilder sb = new StringBuilder();                
                byte[] bs = System.Text.Encoding.Default.GetBytes(txt);        // 原字符串转换成字节数组
                byte[] keys = System.Text.Encoding.Default.GetBytes(KEY);        // 密钥转换成字节数组

                // 异或
                for (int i = 0; i < bs.Length; i++)
                {
                    bs[i] = (byte)(bs[i] ^ keys[i % keys.Length]);
                }

                // 编码成16进制数组
                foreach (byte b in bs)
                {
                    sb.AppendFormat("{0:x2}", b);
                }
                return sb.ToString();
            }
            catch (System.Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string Decrypt(string txt)
        {
            try
            {
                int len = txt.Length;
                byte[] bs = new byte[len / 2];

                // 16进制数组转换会byte数组
                for (int i = 0; i < len / 2; i++)
                {
                    bs[i] = (byte)(Convert.ToInt32(txt.Substring(i * 2, 2), 16));
                }

                byte[] keys = System.Text.Encoding.Default.GetBytes(KEY);        // 密钥转换成字节数组

                // 异或
                for (int i = 0; i < bs.Length; i++)
                {
                    bs[i] = (byte)(bs[i] ^ keys[i % keys.Length]);
                }

                // byte数组还原成字符串
                return System.Text.Encoding.Default.GetString(bs);
            }
            catch (System.Exception)
            {
                return string.Empty;
            }
        }
        #endregion

    }
}
