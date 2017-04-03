using System.Security.Cryptography;
using System.Text;



namespace Promact.Erp.Util.HashingMd5
{
    public class Md5Service : IMd5Service
    {
        /// <summary>
        /// take any string and encrypt it using MD5 then
        /// return the encrypted data 
        /// </summary>
        /// <param name="data">input text you will enterd to encrypt it</param>
        /// <returns>return the encrypted text as hexadecimal string</returns>
        public string GetMD5HashData(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                //create new instance of md5
                StringBuilder hash = new StringBuilder();
                MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
                byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(data));

                for (int i = 0; i < bytes.Length; i++)
                {
                    hash.Append(bytes[i].ToString("x2"));
                }
                // return hexadecimal string
                return hash.ToString();
            }
            return data;
        }
    }

}
