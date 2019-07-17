using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AliyunSignatureDemo.Utils
{
    internal class SignatureHelper
    {
        private const string EncodingType = "UTF-8";

        public static string ValueEncode(string value)
        {
            var stringBuilder = new StringBuilder();
            var text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            var bytes = Encoding.GetEncoding(EncodingType).GetBytes(value);
            foreach (char c in bytes)
            {
                if (text.IndexOf(c) >= 0)
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    stringBuilder.Append("%").Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)c));
                }
            }

            return stringBuilder.ToString();
        }
    }
}
