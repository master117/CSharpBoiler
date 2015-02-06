using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace CSharpBoiler
{
    [Serializable]
    public class AdditionalDemoData
    {
        //Key = DemoName, Value = DemoComment
        public SerializableDictionary<string, string> DemoCommentDictionary = new SerializableDictionary<string, string>();

        public void AddComment(string key, string value)
        {
            if (DemoCommentDictionary.ContainsKey(key))
                DemoCommentDictionary.Remove(key);

            DemoCommentDictionary.Add(key, value);
        }

        public bool Contains(string key)
        {
            return DemoCommentDictionary.ContainsKey(key);
        }

        public string Get(string key)
        {
            return DemoCommentDictionary[key];
        }
    }
}
