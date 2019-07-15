using System;
using System.Collections.Generic;
using System.Text;

namespace UassetToolkit.UPropertyTypes
{
    public class IntProperty : UProperty
    {
        public IntProperty(IOMemoryStream ms, UAssetFile f) : base(ms, f)
        {

        }

        public Int32 data;

        public override void Read(IOMemoryStream ms, UAssetFile f)
        {
            data = ms.ReadInt();
        }

        public override string WriteString()
        {
            return $"data={data}";
        }
    }
}
