using System;

namespace Vctc_CSharp_Sdk
{
    public class VctcException : Exception
    {
        public string message;
        public string code;
        public string rawResponse;
        public string errorCode;
        public VctcException(string message, string code):base(message)
        {
            this.message = message;
            this.code = code;
        }
        public void setRaw(string raw,string code)
        {
            this.rawResponse = raw; 
            this.errorCode = code;
        }

        public new string ToString()
        {
            return code + ":" + message;
        }
    }
}