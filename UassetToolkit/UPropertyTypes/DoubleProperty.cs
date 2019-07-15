using System;
using System.Collections.Generic;
using System.Text;

namespace UassetToolkit.UPropertyTypes
{
    public class DoubleProperty : UProperty
    {
        public DoubleProperty(IOMemoryStream ms, UAssetFile f) : base(ms, f)
        {

        }

        public double data;

        public override void Read(IOMemoryStream ms, UAssetFile f)
        {
            data = ms.ReadDouble();
        }

        public override string WriteString()
        {
            return $"data={data}";
        }
    }
}
