using System;
using System.Collections.Generic;
using System.Text;
using UassetToolkit.UPropertyTypes;

namespace UassetToolkit.UStructTypes
{
    public class IntPointStruct : UStruct
    {
        public int p1;
        public int p2;

        public override void ReadStruct(IOMemoryStream ms, UAssetFile f, StructProperty s)
        {
            p1 = ms.ReadInt();
            p2 = ms.ReadInt();
        }

        public override string WriteString()
        {
            return $"p1={p1}, p2={p2}";
        }
    }
}
