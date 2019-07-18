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
        public static void Main(string[] args)
        {
            var url = RpcSignature.GetAssembleUri();
            var header = RpcSignature.GetHeaders();

            HttpResponse.GetResponse(url, header);
        }
    }
}