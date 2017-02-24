namespace Promact.Erp.Util.HashingMd5
{
    public interface IMd5Service
    {
        /// <summary>
        /// take any string and encrypt it using MD5 then
        /// return the encrypted data 
        /// </summary>
        /// <param name="data">input text you will enterd to encrypt it</param>
        /// <returns>return the encrypted text as hexadecimal string</returns>
        string GetMD5HashData(string data);
    }
}
