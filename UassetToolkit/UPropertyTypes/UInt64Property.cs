using System;
using System.Collections.Generic;
using System.Text;

namespace UassetToolkit.UPropertyTypes
{
    public class UInt64Property : UProperty
    {
        public UInt64Property(IOMemoryStream ms, UAssetFile f) : base(ms, f)
        {

        }

        public UInt64 data;

        public override void Read(IOMemoryStream ms, UAssetFile f)
        {
            data = ms.ReadULong();
        }

        public override string WriteString()
        {
            return $"data={data}";
        }
    }
}
