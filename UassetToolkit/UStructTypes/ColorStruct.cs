using System;
using System.Collections.Generic;
using System.Text;
using UassetToolkit.UPropertyTypes;

namespace UassetToolkit.UStructTypes
{
    public class ColorStruct : UStruct
    {
        public byte b;
        public byte g;
        public byte r;
        public byte a;

        public override void ReadStruct(IOMemoryStream ms, UAssetFile f, StructProperty s)
        {
            b = ms.ReadByte();
            g = ms.ReadByte();
            r = ms.ReadByte();
            a = ms.ReadByte();
        }

        public override string WriteString()
        {
            return $"b={b}, g={g}, r={r}, a={a}";
        }
    }
}
