using AliyunSignatureDemo.Signature;
using AliyunSignatureDemo.Signature.Implement;

namespace AliyunSignatureDemo
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            RoaDemo();
            //RpcDemo();
        }

        private static void RpcDemo()
        {
            var signature = new RpcSignature();

            var url = signature.GetComposedUrl();
            var header = signature.GetHeader();

            HttpResponse.GetResponse(url, header);
        }

        private static void RoaDemo()
        {
            var signature = new RoaSignature();

            var url = signature.GetComposedUrl();
            var header = signature.GetHeader();

            HttpResponse.GetResponse(url, header);
        }
    }
}