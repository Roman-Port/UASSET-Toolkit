using System;
using System.Collections.Generic;
using System.Text;
using UassetToolkit.UPropertyTypes;

namespace UassetToolkit.UStructTypes
{
    public class LinearColorStruct : UStruct
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public override void ReadStruct(IOMemoryStream ms, UAssetFile f, StructProperty s)
        {
            r = ms.ReadFloat();
            g = ms.ReadFloat();
            b = ms.ReadFloat();
            a = ms.ReadFloat();
        }

        public override string WriteString()
        {
            return $"r={r}, g={g}, b={b}, a={a}";
        }
    }
}
