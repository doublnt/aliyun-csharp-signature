using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace AliyunSignatureDemo
{
    public class HttpResponse
    {
        public static void GetResponse(string url, IDictionary<string, string> headers)
        {
            var request = WebRequest.Create(new Uri(url)) as HttpWebRequest;

            foreach (var item in headers) request.Headers.Add(item.Key, item.Value);

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
