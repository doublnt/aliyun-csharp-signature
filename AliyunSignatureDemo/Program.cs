using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

using AliyunSignatureDemo.Roa;
using AliyunSignatureDemo.Rpc;

namespace AliyunSignatureDemo
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            RoaDemo();
        }

        private static void RpcDemo()
        {
            var url = RpcSignature.GetAssembleUri();
            var header = RpcSignature.GetHeaders();

            HttpResponse.GetResponse(url, header);
        }

        private static void RoaDemo()
        {
            var url = RoaSignature.GetFinalUrl();
            var header = RoaSignature.GetHeader();

            HttpResponse.GetResponse(url, header);
        }
    }
}