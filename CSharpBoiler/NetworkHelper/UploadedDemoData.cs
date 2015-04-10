using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBoiler.NetworkHelper
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
