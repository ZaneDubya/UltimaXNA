using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.GameObjects
{
    public class PropertyList
    {
        public int Hash = 0;
        private List<string> mPropertyList = new List<string>();

        public bool HasProperties
        {
            get
            {
                if (mPropertyList.Count == 0)
                    return false;
                else
                    return true;
            }
        }

        public string Properties
        {
            get
            {
                string iPropertyConcat = string.Empty;
                for (int i = 0; i < mPropertyList.Count; i++)
                {
                    iPropertyConcat += mPropertyList[i];
                    if (i < mPropertyList.Count - 1)
                    {
                        iPropertyConcat += Environment.NewLine;
                    }
                }
                return iPropertyConcat;
            }
        }

        public void Clear()
        {
            mPropertyList.Clear();
        }

        public void AddProperty(string nProperty)
        {
            mPropertyList.Add(nProperty);
        }
    }
}
