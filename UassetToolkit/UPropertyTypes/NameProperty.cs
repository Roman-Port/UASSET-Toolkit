using System;
using System.Collections.Generic;
using System.Text;

namespace UassetToolkit.UPropertyTypes
{
    public class NameProperty : UProperty
    {
        public NameProperty(IOMemoryStream ms, UAssetFile f) : base(ms, f)
        {

        }

        public string data;
        public int unknown;

        public override void Read(IOMemoryStream ms, UAssetFile f)
        {
            if(length == 0 || length == 8)
            {
                data = ms.ReadNameTableEntry(f);
                unknown = ms.ReadInt();
            } else
            {
                data = ms.ReadNameTableEntry(f);
            }
        }

        public override string WriteString()
        {
            return $"data={data}, unknown={unknown}";
        }
    }
}
