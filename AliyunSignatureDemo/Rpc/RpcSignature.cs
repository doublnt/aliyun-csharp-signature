using System;
using System.Collections.Generic;
using System.Text;

using AliyunSignatureDemo.Signer;
using AliyunSignatureDemo.Utils;

namespace AliyunSignatureDemo.Rpc
{
    public class RpcSignature
    {
        private const string SplitStr = "&";
        private const string RdsUrl = "rds.aliyuncs.com";
        private static readonly string AccessKeyId = Environment.GetEnvironmentVariable("ACCESS_KEY_ID");

        private static readonly string AccessKeySecret = Environment.GetEnvironmentVariable("ACCESS_KEY_SECRET");
        private static readonly IDictionary<string, string> Headers = new Dictionary<string, string>();
        private static readonly IDictionary<string, string> Queries = new Dictionary<string, string>();

        private static void RpcHeaders(IDictionary<string, string> headers)
        {
            headers.Add("x-sdk-client", "Net/2.0.0");
            headers.Add("x-sdk-invoke-type", "normal");
            headers.Add("User-Agent",
                "User-Agent, Alibaba Cloud (Microsoft Windows 10.0.18362 ) netcoreapp/2.0.9 Core/1.5.1.0");
        }

        private static void RpcQueries(IDictionary<string, string> queries)
        {
            var version = "2014-08-15";
            var action = "DescribeRegions";

            queries.TryAdd("Action", action);
            queries.TryAdd("Version", version);
            queries.TryAdd("Format", "JSON");
            var timeStamp = SignatureHelper.FormatIso8601Date(DateTime.Now);
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

        private static string ComposeStringToSign(IDictionary<string, string> queries)
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


        public static string GetAssembleUri()
        {
            RpcQueries(Queries);
            RpcHeaders(Headers);

            var stringToSign = ComposeStringToSign(Queries);
            var signature = HmacSha1Signer.ComputeSignature(stringToSign, AccessKeySecret + "&");

            Queries.TryAdd("Signature", signature);

            return SignatureHelper.ComposeUrl(RdsUrl, Queries);
        }

        public static IDictionary<string, string> GetHeaders()
        {
            return Headers;
        }
    }
}