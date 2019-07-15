using System;
using System.Collections.Generic;
using System.Text;

namespace UassetToolkit.UPropertyTypes
{
    public class StrProperty : UProperty
    {
        public StrProperty(IOMemoryStream ms, UAssetFile f) : base(ms, f)
        {

        }

        public string data;

        public override void Read(IOMemoryStream ms, UAssetFile f)
        {
            data = ms.ReadUEString();
        }

        public override string WriteString()
        {
            return $"data={data}";
        }
    }
}
