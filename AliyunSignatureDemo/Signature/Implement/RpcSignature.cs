using System;
using System.Collections.Generic;
using System.Text;

using AliyunSignatureDemo.Signer;
using AliyunSignatureDemo.Utils;

namespace AliyunSignatureDemo.Signature.Implement
{
    public class RpcSignature : Signature
    {
        public RpcSignature()
        {
            Version = "2014-08-15";
            Action = "DescribeRegions";
            Endpoint = "rds.aliyuncs.com";
        }

        private void ComposeRpcQueries(IDictionary<string, string> queries)
        {
            var timeStamp = SignatureHelper.FormatIso8601Date(DateTime.Now);

            queries.TryAdd("Action", Action);
            queries.TryAdd("Version", Version);
            queries.TryAdd("Format", Format);
            queries.TryAdd("RegionId", RegionId);
            queries.TryAdd("Timestamp", timeStamp);
            queries.TryAdd("SignatureMethod", HmacSha1Signer.GetSignatureMethod());
            queries.TryAdd("SignatureVersion", HmacSha1Signer.GetSignatureVersion());
            queries.TryAdd("SignatureNonce", HmacSha1Signer.GetSignatureNonce());
            queries.TryAdd("AccessKeyId", AccessKeyId);
        }

        public override string ComposeStringToSign(IDictionary<string, string> queries)
        {
            var tempQueries = new Dictionary<string, string>(queries);

            var sortedDictionary = new SortedDictionary<string, string>(tempQueries, StringComparer.Ordinal);
            var headerQueryString = new StringBuilder();

            foreach (var item in sortedDictionary)
                headerQueryString.Append(HeaderSeparator)
                    .Append(SignatureHelper.ValueEncode(item.Key))
                    .Append("=")
                    .Append(SignatureHelper.ValueEncode(item.Value));

            var stringToSign = new StringBuilder();
            stringToSign.Append(MethodType);
            stringToSign.Append(HeaderSeparator);
            stringToSign.Append(SignatureHelper.ValueEncode("/"));
            stringToSign.Append(HeaderSeparator);
            stringToSign.Append(SignatureHelper.ValueEncode(headerQueryString.ToString().Substring(1)));

            return stringToSign.ToString();
        }

        public override string GetComposedUrl()
        {
            ComposeHeader(Headers);
            ComposeRpcQueries(Queries);

            var stringToSign = ComposeStringToSign(Queries);
            var signature = HmacSha1Signer.ComputeSignature(stringToSign, AccessKeySecret + "&");

            Queries.TryAdd("Signature", signature);

            return ComposeUrl(Endpoint, Queries);
        }

        private string ComposeUrl(string endpoint, IDictionary<string, string> queries)
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