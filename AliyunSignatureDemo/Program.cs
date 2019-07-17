using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using AliyunSignatureDemo.Rpc;

namespace AliyunSignatureDemo
{
    internal class Program
    {
        private const string EcsUrl = "rds.aliyuncs.com";


        private static readonly Dictionary<string, string> Headers = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> Queries = new Dictionary<string, string>();


        private static readonly string _accessKeySecret = Environment.GetEnvironmentVariable("ACCESS_KEY_SECRET");

        private static void Main(string[] args)
        {
            RpcSignature.RpcQueries(Queries);
            RpcSignature.RpcHeaders(Headers);

            var stringToSign = RpcSignature.ComposeStringToSign(Queries);
            var signature = RpcSignature.ComputeSignature(stringToSign, _accessKeySecret + "&");

            Queries.TryAdd("Signature", signature);

            var url = RpcSignature.ComposeUrl(EcsUrl, Queries);

            var request = WebRequest.Create(new Uri(url)) as HttpWebRequest;

            foreach (var item in Headers) request.Headers.Add(item.Key, item.Value);

            var response = request.GetResponse() as HttpWebResponse;

            using (var memoryStream = new MemoryStream())
            {
                var buffer = new byte[1024];
                var stream = response.GetResponseStream();

                while (true)
                {
                    var length = stream.Read(buffer, 0, 1024);
                    if (length == 0) break;

                    memoryStream.Write(buffer, 0, length);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                var bytes = new byte[memoryStream.Length];
                memoryStream.Read(bytes, 0, bytes.Length);

                stream.Close();
                stream.Dispose();

                Console.WriteLine(Encoding.UTF8.GetString(bytes));
            }
        }
    }
}