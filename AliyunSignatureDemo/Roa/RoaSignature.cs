using System;
using System.Collections.Generic;
using System.Text;

using AliyunSignatureDemo.Signer;
using AliyunSignatureDemo.Utils;

namespace AliyunSignatureDemo.Roa
{
    public class RoaSignature
    {
        private const string UriPattern = "/version";
        private const string Version = "2015-12-15";
        private const string Action = "DescribeApiVersion";
        private const string OpenApiType = "openAPI";
        private const string MethodType = "GET";
        private const string AcceptType = "RAM";
        private const string Endpoint = "cs.aliyuncs.com";

        private const string QuerySeparator = "&";
        private const string HeaderSeparator = "\n";

        private static IDictionary<string, string> Headers = new Dictionary<string, string>();
        private static readonly IDictionary<string, string> Paths = new Dictionary<string, string>();
        private static readonly IDictionary<string, string> Queries = new Dictionary<string, string>();

        public static string GetFinalUrl()
        {
            AssemblyHeaders(Headers);
            var tempHeader = SignRequestUrl(Headers);
            AddSignatureToHeader(tempHeader, Paths, Queries);

            Headers = tempHeader;
            var url = ComposeUrl(Endpoint, Queries);

            return url;
        }

        public static IDictionary<string, string> GetHeader()
        {
            return Headers;
        }

        private static void AssemblyHeaders(IDictionary<string, string> headers)
        {
            headers.TryAdd("x-acs-version", Version);
            headers.Add("x-sdk-client", "Net/2.0.0");
            headers.Add("x-sdk-invoke-type", "normal");
            headers.Add("User-Agent",
                "User-Agent, Alibaba Cloud (Microsoft Windows 10.0.18362 ) netcoreapp/2.0.9 Core/1.5.1.0");
            headers.TryAdd("Accept", AcceptType);
        }

        private static IDictionary<string, string> SignRequestUrl(IDictionary<string, string> headers)
        {
            var imutableMap = new Dictionary<string, string>(headers);

            imutableMap.TryAdd("Date", SignatureHelper.GetRFC2616Date(DateTime.Now));
            imutableMap.TryAdd("Accept", SignatureHelper.FormatTypeToString("Json"));
            imutableMap.TryAdd("x-acs-signature-method", "HMAC-SHA1");
            imutableMap.TryAdd("x-acs-signature-version", "1.0");

            return imutableMap;
        }

        private static string ComputeStringToSign(IDictionary<string, string> headers,
            IDictionary<string, string> paths, IDictionary<string, string> queries)
        {
            var sb = new StringBuilder();
            sb.Append(MethodType).Append(HeaderSeparator);
            if (headers.ContainsKey("Accept")) sb.Append(headers["Accept"]);

            sb.Append(HeaderSeparator);
            if (headers.ContainsKey("Content-MD5")) sb.Append(headers["Content-MD5"]);

            sb.Append(HeaderSeparator);
            if (headers.ContainsKey("Content-Type")) sb.Append(headers["Content-Type"]);

            sb.Append(HeaderSeparator);
            if (headers.ContainsKey("Date")) sb.Append(headers["Date"]);

            sb.Append(HeaderSeparator);
            var uri = ReplaceOccupiedParameters(UriPattern, paths);
            sb.Append(SignatureHelper.BuildCanonicalHeaders(headers, "x-acs-", HeaderSeparator));
            sb.Append(SignatureHelper.BuildQuerystring(uri, queries, QuerySeparator));

            return sb.ToString();
        }

        private static void AddSignatureToHeader(IDictionary<string, string> headers, IDictionary<string, string> paths, IDictionary<string, string> queries)
        {
            string accessKeyId = Environment.GetEnvironmentVariable("ACCESS_KEY_ID");
            string accessKeySecret = Environment.GetEnvironmentVariable("ACCESS_KEY_SECRET");

            var stringToSign = ComputeStringToSign(headers, paths, queries);

            var signature =
                HmacSha1Signer.ComputeSignature(stringToSign, accessKeySecret);

            headers.TryAdd("Authorization", "acs " + accessKeyId + ":" + signature);
        }

        public static string ReplaceOccupiedParameters(string url, IDictionary<string, string> paths)
        {
            var result = url;
            foreach (var entry in paths)
            {
                var key = entry.Key;
                var value = entry.Value;
                var target = "[" + key + "]";
                result = result.Replace(target, value);
            }

            return result;
        }

        public static string ComposeUrl(string endpoint, IDictionary<string, string> queries)
        {
            var mapQueries = queries;
            var urlBuilder = new StringBuilder("");
            urlBuilder.Append("http");
            urlBuilder.Append("://").Append(endpoint);
            if (null != UriPattern)
            {
                urlBuilder.Append(ReplaceOccupiedParameters(UriPattern, Paths));
            }

            if (-1 == urlBuilder.ToString().IndexOf('?'))
            {
                urlBuilder.Append("?");
            }
            else if (!urlBuilder.ToString().EndsWith("?"))
            {
                urlBuilder.Append("&");
            }

            var query = SignatureHelper.ConcatQueryString(mapQueries);
            var url = urlBuilder.Append(query).ToString();
            if (url.EndsWith("?") || url.EndsWith("&"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            return url;
        }
    }
}