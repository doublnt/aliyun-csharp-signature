using System;
using System.Collections.Generic;
using System.Text;

using AliyunSignatureDemo.Signer;
using AliyunSignatureDemo.Utils;

namespace AliyunSignatureDemo.Signature.Implement
{
    public class RoaSignature : Signature
    {
        public RoaSignature()
        {
            UrlPattern = "/version";
            AcceptType = "RAM";
            Version = "2015-12-15";
            Endpoint = "cs.aliyuncs.com";
        }

        public override string GetComposedUrl()
        {
            AssemblyHeaders(Headers);

            var tempHeader = SignRequestUrl(Headers);
            var stringToSign = ComposeStringToSign(tempHeader);
            var signature = HmacSha1Signer.ComputeSignature(stringToSign, AccessKeySecret);

            tempHeader.TryAdd("Authorization", "acs " + AccessKeyId + ":" + signature);

            Headers = tempHeader;

            return ComposeUrl(Endpoint, Queries);
        }

        private void AssemblyHeaders(IDictionary<string, string> headers)
        {
            ComposeHeader(headers);

            headers.TryAdd("x-acs-version", Version);
            headers.TryAdd("Accept", AcceptType);
        }

        private static IDictionary<string, string> SignRequestUrl(IDictionary<string, string> headers)
        {
            var imutableMap = new Dictionary<string, string>(headers);

            imutableMap.TryAdd("Date", SignatureHelper.GetRfc2616Date(DateTime.Now));
            imutableMap.TryAdd("Accept", SignatureHelper.FormatTypeToString("Json"));
            imutableMap.TryAdd("x-acs-signature-method", HmacSha1Signer.GetSignatureMethod());
            imutableMap.TryAdd("x-acs-signature-version", HmacSha1Signer.GetSignatureVersion());

            return imutableMap;
        }

        public override string ComposeStringToSign(IDictionary<string, string> headers)
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
            var uri = ReplaceOccupiedParameters(UrlPattern, Paths);
            sb.Append(BuildCanonicalHeaders(headers, "x-acs-", HeaderSeparator));
            sb.Append(BuildQuerystring(uri, Queries, QuerySeparator));

            return sb.ToString();
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

        public string ComposeUrl(string endpoint, IDictionary<string, string> queries)
        {
            var mapQueries = queries;
            var urlBuilder = new StringBuilder("");
            urlBuilder.Append("http");
            urlBuilder.Append("://").Append(endpoint);
            if (null != UrlPattern) urlBuilder.Append(ReplaceOccupiedParameters(UrlPattern, Paths));

            if (-1 == urlBuilder.ToString().IndexOf('?'))
                urlBuilder.Append("?");
            else if (!urlBuilder.ToString().EndsWith("?")) urlBuilder.Append("&");

            var query = SignatureHelper.ConcatQueryString(mapQueries);
            var url = urlBuilder.Append(query).ToString();
            if (url.EndsWith("?") || url.EndsWith("&")) url = url.Substring(0, url.Length - 1);

            return url;
        }

        public static string BuildCanonicalHeaders(IDictionary<string, string> headers, string headerBegin,
            string headerSeparator)
        {
            var sortMap = new Dictionary<string, string>();
            foreach (var e in headers)
            {
                var key = e.Key.ToLower();
                var val = e.Value;
                if (key.StartsWith(headerBegin)) sortMap.Add(key, val);
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

        public static string BuildQuerystring(string uri, IDictionary<string, string> queries, string querySeparator)
        {
            var uriParts = SignatureHelper.SplitSubResource(uri);
            var sortMap = new Dictionary<string, string>(queries);
            if (null != uriParts[1]) sortMap.Add(uriParts[1], null);

            var queryBuilder = new StringBuilder(uriParts[0]);
            var sortedDictionary = new SortedDictionary<string, string>(sortMap);
            if (0 < sortedDictionary.Count) queryBuilder.Append("?");

            foreach (var e in sortedDictionary)
            {
                queryBuilder.Append(e.Key);
                if (null != e.Value) queryBuilder.Append("=").Append(e.Value);

                queryBuilder.Append(querySeparator);
            }

            var querystring = queryBuilder.ToString();
            if (querystring.EndsWith(querySeparator)) querystring = querystring.Substring(0, querystring.Length - 1);

            return querystring;
        }
    }
}