using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web;

using AliyunSignatureDemo.Utils;

namespace AliyunSignatureDemo.Rpc
{
    internal class RpcSignature
    {
        private const string SplitStr = "&";
        private const string RdsUrl = "rds.aliyuncs.com";
        private static readonly string AccessKeyId = Environment.GetEnvironmentVariable("ACCESS_KEY_ID");

        private static readonly string AccessKeySecret = Environment.GetEnvironmentVariable("ACCESS_KEY_SECRET");
        private static readonly IDictionary<string, string> Headers = new Dictionary<string, string>();
        private static readonly IDictionary<string, string> Queries = new Dictionary<string, string>();

        internal static string ComposeUrl(string endpoint, IDictionary<string, string> queries)
        {
            var urlBuilder = new StringBuilder("");
            urlBuilder.Append("http");
            urlBuilder.Append("://").Append(endpoint);
            if (-1 == urlBuilder.ToString().IndexOf("?")) urlBuilder.Append("/?");

            var query = ConcatQueryString(queries);
            return urlBuilder.Append(query).ToString();
        }

        internal static string ConcatQueryString(IDictionary<string, string> parameters)
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

        internal static void RpcHeaders(IDictionary<string, string> headers)
        {
            headers.Add("x-sdk-client", "Net/2.0.0");
            headers.Add("x-sdk-invoke-type", "normal");
            headers.Add("User-Agent",
                "User-Agent, Alibaba Cloud (Microsoft Windows 10.0.18362 ) netcoreapp/2.0.9 Core/1.5.1.0");
        }

        internal static void RpcQueries(IDictionary<string, string> queries)
        {
            var version = "2014-08-15";
            var action = "DescribeRegions";

            queries.TryAdd("Action", action);
            queries.TryAdd("Version", version);
            queries.TryAdd("Format", "JSON");
            var timeStamp = FormatIso8601Date(DateTime.Now);
            var signatureMethod = "HMAC-SHA1";
            var signatureVersion = "1.0";
            var signatureNonce = Guid.NewGuid().ToString();

            queries.TryAdd("RegionId", "cn-hangzhou");
            queries.TryAdd("Timestamp", timeStamp);
            queries.TryAdd("SignatureMethod", signatureMethod);
            queries.TryAdd("SignatureVersion", signatureVersion);
            queries.TryAdd("SignatureNonce", signatureNonce);
            queries.TryAdd("AccessKeyId", AccessKeyId);
            queries.TryAdd("ServiceCode", "rds");
            queries.TryAdd("Type", "openAPI");
            queries.TryAdd("id", "cn-hangzhou");
        }

        internal static string ComputeSignature(string stringToSign, string accessKeySecret)
        {
            using (var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(accessKeySecret)))
            {
                var hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
                return Convert.ToBase64String(hashValue);
            }
        }


        internal static string ComposeStringToSign(IDictionary<string, string> queries)
        {
            var tempQueries = new Dictionary<string, string>(queries);

            var sortedDictionary = new SortedDictionary<string, string>(tempQueries, StringComparer.Ordinal);
            var headerQueryString = new StringBuilder();

            foreach (var item in sortedDictionary)
                headerQueryString.Append(SplitStr)
                    .Append(SignatureHelper.ValueEncode(item.Key))
                    .Append("=")
                    .Append(SignatureHelper.ValueEncode(item.Value));

            var stringToSign = new StringBuilder();
            stringToSign.Append("GET");
            stringToSign.Append(SplitStr);
            stringToSign.Append(SignatureHelper.ValueEncode("/"));
            stringToSign.Append(SplitStr);
            stringToSign.Append(SignatureHelper.ValueEncode(headerQueryString.ToString().Substring(1)));

            return stringToSign.ToString();
        }

        private static string FormatIso8601Date(DateTime date)
        {
            var ISO8601_DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:ss'Z'";

            return date.ToUniversalTime()
                .ToString(ISO8601_DATE_FORMAT, CultureInfo.CreateSpecificCulture("en-US"));
        }

        public static string GetAssembleUri()
        {
            RpcQueries(Queries);
            RpcHeaders(Headers);

            var stringToSign = ComposeStringToSign(Queries);
            var signature = ComputeSignature(stringToSign, AccessKeySecret + "&");

            Queries.TryAdd("Signature", signature);

            return ComposeUrl(RdsUrl, Queries);
        }

        public static IDictionary<string, string> GetHeaders()
        {
            return Headers;
        }
    }
}