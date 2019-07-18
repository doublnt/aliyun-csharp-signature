using System;
using System.Collections.Generic;

using AliyunSignatureDemo.Signature.Interface;

namespace AliyunSignatureDemo.Signature.Implement
{
    public abstract class Signature : ISignature
    {
        public string UrlPattern { get; set; }

        public string HeaderSeparator { get; set; }

        public string QuerySeparator { get; set; } = "&";

        public string Version { get; set; }

        public string MethodType { get; set; } = "GET";

        public string AcceptType { get; set; }

        public string Endpoint { get; set; }

        public string RegionId { get; set; } = "cn-hangzhou";

        public string Action { get; set; }

        public string Format { get; set; } = "JSON";

        public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        public IDictionary<string, string> Queries { get; set; } = new Dictionary<string, string>();

        public IDictionary<string, string> Paths { get; set; } = new Dictionary<string, string>();

        public IDictionary<string, string> Bodies { get; set; } = new Dictionary<string, string>();

        public string AccessKeyId { get; set; } = Environment.GetEnvironmentVariable("ACCESS_KEY_ID");

        public string AccessKeySecret { get; set; } = Environment.GetEnvironmentVariable("ACCESS_KEY_SECRET");


        public abstract string GetComposedUrl();

        public virtual IDictionary<string, string> GetHeader()
        {
            return Headers;
        }

        public void ComposeHeader(IDictionary<string, string> header)
        {
            header.Add("x-sdk-client", "Net/2.0.0");
            header.Add("x-sdk-invoke-type", "normal");
            header.Add("User-Agent",
                "User-Agent, Alibaba Cloud (Microsoft Windows 10.0.18362 ) netcoreapp/2.0.9 Core/1.5.1.0");

        }

        public abstract string ComposeStringToSign(IDictionary<string, string> dic);
    }
}