using System;
using System.Collections.Generic;
using System.Text;

namespace AliyunSignatureDemo.Signature.Interface
{
    interface ISignature
    {
        string GetComposedUrl();

        IDictionary<string, string> GetHeader();

        string ComposeStringToSign(IDictionary<string, string> dic);

        void ComposeHeader(IDictionary<string, string> dic);
    }
}
