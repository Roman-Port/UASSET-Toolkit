using System;
using System.Collections.Generic;
using System.Text;

namespace UassetToolkit.UPropertyTypes
{
    public class Int8Property : UProperty
    {
        public Int8Property(IOMemoryStream ms, UAssetFile f) : base(ms, f)
        {

        }

        public byte data;

        public override void Read(IOMemoryStream ms, UAssetFile f)
        {
            data = ms.ReadByte();
        }

        public override string WriteString()
        {
            return $"data={data}";
        }
    }
}
