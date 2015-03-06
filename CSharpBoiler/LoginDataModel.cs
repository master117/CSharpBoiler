using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBoiler
{
    [Serializable]
    public class LoginDataModel
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
