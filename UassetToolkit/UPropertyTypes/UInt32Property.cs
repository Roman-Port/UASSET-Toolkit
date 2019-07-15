using System;
using System.Collections.Generic;
using System.Text;

namespace UassetToolkit.UPropertyTypes
{
    public class UInt32Property : UProperty
    {
        public UInt32Property(IOMemoryStream ms, UAssetFile f) : base(ms, f)
        {

        }

        public UInt32 data;

        public override void Read(IOMemoryStream ms, UAssetFile f)
        {
            data = ms.ReadUInt();
        }

        public override string WriteString()
        {
            return $"data={data}";
        }
    }
}
