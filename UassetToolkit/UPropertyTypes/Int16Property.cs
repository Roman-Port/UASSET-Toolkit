using System;
using System.Collections.Generic;
using System.Text;

namespace UassetToolkit.UPropertyTypes
{
    public class Int16Property : UProperty
    {
        public Int16Property(IOMemoryStream ms, UAssetFile f) : base(ms, f)
        {

        }

        public Int16 data;

        public override void Read(IOMemoryStream ms, UAssetFile f)
        {
            data = ms.ReadShort();
        }

        public override string WriteString()
        {
            return $"data={data}";
        }
    }
}
