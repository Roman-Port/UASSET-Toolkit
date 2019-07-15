using System;
using System.Collections.Generic;
using System.Text;

namespace UassetToolkit.UPropertyTypes
{
    public class FloatProperty : UProperty
    {
        public FloatProperty(IOMemoryStream ms, UAssetFile f) : base(ms, f)
        {

        }

        public float data;

        public override void Read(IOMemoryStream ms, UAssetFile f)
        {
            data = ms.ReadFloat();
        }

        public override string WriteString()
        {
            return $"data={data}";
        }
    }
}
