using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBoiler.Models
{
    public class VacWatch_nlUploadedData : ISerializable
    {
        public long steamId { get; set; }
        public bool isSend { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
