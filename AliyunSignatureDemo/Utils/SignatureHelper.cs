using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;

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
                if (text.IndexOf(c) >= 0)
                    stringBuilder.Append(c);
                else
                    stringBuilder.Append("%").Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)c));

            return stringBuilder.ToString();
        }

        public static string FormatIso8601Date(DateTime date)
        {
            var ISO8601_DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:ss'Z'";

            return date.ToUniversalTime()
                .ToString(ISO8601_DATE_FORMAT, CultureInfo.CreateSpecificCulture("en-US"));
        }

        public static string GetRfc2616Date(DateTime datetime)
        {
            return datetime.ToUniversalTime().GetDateTimeFormats('r')[0];
        }

        public static string FormatTypeToString(string formatType)
        {
            formatType = formatType.ToUpper();

            switch (formatType)
            {
                case "XML":
                    return "application/xml";
                case "JSON":
                    return "application/json";
                case "FORM":
                    return "application/x-www-form-urlencoded";
                default:
                    return "application/octet-stream";
            }
        }

        public static string ConcatQueryString(IDictionary<string, string> parameters)
        {
            if (null == parameters) return null;

            var sb = new StringBuilder();

            foreach (var entry in parameters)
            {
                var key = entry.Key;
                var val = entry.Value;

                sb.Append(HttpUtility.UrlEncode(key, Encoding.UTF8));
                if (val != null) sb.Append("=").Append(HttpUtility.UrlEncode(val, Encoding.UTF8));

                sb.Append("&");
            }

            var strIndex = sb.Length;
            if (parameters.Count > 0) sb.Remove(strIndex - 1, 1);

            return sb.ToString();
        }

        public static string[] SplitSubResource(string uri)
        {
            var queIndex = uri.IndexOf("?");
            var uriParts = new string[2];
            if (-1 != queIndex)
            {
                uriParts[0] = uri.Substring(0, queIndex);
                uriParts[1] = uri.Substring(queIndex + 1);
            }
            else
            {
                uriParts[0] = uri;
            }

            return uriParts;
        }
    }
}