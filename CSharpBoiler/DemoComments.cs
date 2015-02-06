using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CSharpBoiler
{
    [Serializable]
    public class DemoComments
    {
        //Key = DemoName, Value = DemoComment

        private SerializableDictionary<string, string> DemoCommentDictionary = new SerializableDictionary<string, string>();

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
