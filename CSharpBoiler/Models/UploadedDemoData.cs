using System;
using System.Runtime.Serialization;

namespace CSharpBoiler.Models
{
    public class UploadedDemoData : ISerializable
    {
        public string demoUrl { get; set; }
        public string matchDate { get; set; }
        public string eventString { get; set; }

        public bool isSend { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
