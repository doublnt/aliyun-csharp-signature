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

        public static string GetRFC2616Date(DateTime datetime)
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

        public static string BuildQuerystring(string uri, IDictionary<string, string> queries, string querySeparator)
        {
            var uriParts = SplitSubResource(uri);
            var sortMap = new Dictionary<string, string>(queries);
            if (null != uriParts[1])
            {
                sortMap.Add(uriParts[1], null);
            }

            var queryBuilder = new StringBuilder(uriParts[0]);
            var sortedDictionary = new SortedDictionary<string, string>(sortMap);
            if (0 < sortedDictionary.Count)
            {
                queryBuilder.Append("?");
            }

            foreach (var e in sortedDictionary)
            {
                queryBuilder.Append(e.Key);
                if (null != e.Value)
                {
                    queryBuilder.Append("=").Append(e.Value);
                }

                queryBuilder.Append(querySeparator);
            }

            var querystring = queryBuilder.ToString();
            if (querystring.EndsWith(querySeparator))
            {
                querystring = querystring.Substring(0, querystring.Length - 1);
            }

            return querystring;
        }

        private static string[] SplitSubResource(string uri)
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

        public static string BuildCanonicalHeaders(IDictionary<string, string> headers, string headerBegin, string headerSeparator)
        {
            var sortMap = new Dictionary<string, string>();
            foreach (var e in headers)
            {
                var key = e.Key.ToLower();
                var val = e.Value;
                if (key.StartsWith(headerBegin))
                {
                    sortMap.Add(key, val);
                }
            }

            var sortedDictionary = new SortedDictionary<string, string>(sortMap);

            var headerBuilder = new StringBuilder();
            foreach (var e in sortedDictionary)
            {
                headerBuilder.Append(e.Key);
                headerBuilder.Append(':').Append(e.Value);
                headerBuilder.Append(headerSeparator);
            }

            return headerBuilder.ToString();
        }

        public static string ComposeUrl(string endpoint, IDictionary<string, string> queries)
        {
            var urlBuilder = new StringBuilder("");
            urlBuilder.Append("http");
            urlBuilder.Append("://").Append(endpoint);
            if (-1 == urlBuilder.ToString().IndexOf("?")) urlBuilder.Append("/?");

            var query = SignatureHelper.ConcatQueryString(queries);
            return urlBuilder.Append(query).ToString();
        }
    }
}