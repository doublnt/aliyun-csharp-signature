using System;
using System.Security.Cryptography;
using System.Text;

namespace AliyunSignatureDemo.Signer
{
    public class HmacSha1Signer
    {
        public static string ComputeSignature(string stringToSign, string accessKeySecret)
        {
            using (var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(accessKeySecret)))
            {
                var hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
                return Convert.ToBase64String(hashValue);
            }
        }

        public static string GetSignatureMethod()
        {
            return "HMAC-SHA1";
        }

        public static string GetSignatureVersion()
        {
            return "1.0";
        }

        public static string GetSignatureNonce()
        {
            return Guid.NewGuid().ToString();
        }
    }
}