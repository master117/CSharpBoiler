/*
Boiler
Copyright (C) 2015  Johannes Gocke

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

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
        public SerializableDictionary<string, int> K3Dictionary = new SerializableDictionary<string, int>();
        public SerializableDictionary<string, int> K4Dictionary = new SerializableDictionary<string, int>();
        public SerializableDictionary<string, int> K5Dictionary = new SerializableDictionary<string, int>();
        public SerializableDictionary<string, int> HSDictionary = new SerializableDictionary<string, int>();

        #region generated Getter Setter
        public void AddComment(string key, string value)
        {
            if (DemoCommentDictionary.ContainsKey(key))
                DemoCommentDictionary.Remove(key);

            DemoCommentDictionary.Add(key, value);
        }

        public bool ContainsComment(string key)
        {
            return DemoCommentDictionary.ContainsKey(key);
        }

        public string GetComment(string key)
        {
            return DemoCommentDictionary[key];
        }

        public void AddK3(string key, int value)
        {
            if (K3Dictionary.ContainsKey(key))
                K3Dictionary.Remove(key);

            K3Dictionary.Add(key, value);
        }

        public bool ContainsK3(string key)
        {
            return K3Dictionary.ContainsKey(key);
        }

        public int GetK3(string key)
        {
            return K3Dictionary[key];
        }

        public void AddK4(string key, int value)
        {
            if (K4Dictionary.ContainsKey(key))
                K4Dictionary.Remove(key);

            K4Dictionary.Add(key, value);
        }

        public bool ContainsK4(string key)
        {
            return K4Dictionary.ContainsKey(key);
        }

        public int GetK4(string key)
        {
            return K4Dictionary[key];
        }

        public void AddK5(string key, int value)
        {
            if (K5Dictionary.ContainsKey(key))
                K5Dictionary.Remove(key);

            K5Dictionary.Add(key, value);
        }

        public bool ContainsK5(string key)
        {
            return K5Dictionary.ContainsKey(key);
        }

        public int GetK5(string key)
        {
            return K5Dictionary[key];
        }

        public void AddHS(string key, int value)
        {
            if (HSDictionary.ContainsKey(key))
                HSDictionary.Remove(key);

            HSDictionary.Add(key, value);
        }

        public bool ContainsHS(string key)
        {
            return HSDictionary.ContainsKey(key);
        }

        public int GetHS(string key)
        {
            return HSDictionary[key];
        }
        #endregion
    }
}
