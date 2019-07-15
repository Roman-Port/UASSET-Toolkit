using System;
using System.Collections.Generic;
using System.Text;
using UassetToolkit.UPropertyTypes;

namespace UassetToolkit.UStructTypes
{
    public class Vector2Struct : UStruct
    {
        public float x;
        public float y;

        public override void ReadStruct(IOMemoryStream ms, UAssetFile f, StructProperty s)
        {
            x = ms.ReadFloat();
            y = ms.ReadFloat();
        }

        public override string WriteString()
        {
            return $"x={x}, y={y}";
        }
    }
}
