using System;

namespace VctcNet.Sdk
{
    public class VctcException : Exception
    {
        public string message;
        public string code;
        public string rawResponse;
        public string errorCode;
        public VctcException(string message, string code):base(code+":"+message)
        {
            this.message = message;
            this.code = code;
        }
        public void setRaw(string raw,string code)
        {
            this.rawResponse = raw; 
            this.errorCode = code;
        }

        public override string ToString()
        {
            return code + ":" + message;
        }
    }
}