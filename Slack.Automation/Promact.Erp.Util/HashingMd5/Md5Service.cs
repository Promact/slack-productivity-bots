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
                MD5 md5 = MD5.Create();
                //convert the input text to array of bytes
                byte[] hashData = md5.ComputeHash(Encoding.Default.GetBytes(data));
                //create new instance of StringBuilder to save hashed data
                StringBuilder returnValue = new StringBuilder();
                //loop for each byte and add it to StringBuilder
                for (int i = 0; i < hashData.Length; i++)
                {
                    returnValue.Append(hashData[i].ToString());
                }
                data = returnValue.ToString();
            }
            // return hexadecimal string
            return data;
        }
    }
}
