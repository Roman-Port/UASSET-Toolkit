using System;
using System.Collections.Generic;
using System.Text;

namespace UassetToolkit.UPropertyTypes
{
    public class UInt16Property : UProperty
    {
        public UInt16Property(IOMemoryStream ms, UAssetFile f) : base(ms, f)
        {

        }

        public UInt16 data;

        public override void Read(IOMemoryStream ms, UAssetFile f)
        {
            data = ms.ReadUShort();
        }

        public override string WriteString()
        {
            return $"data={data}";
        }
    }
}
