using System;
using System.Collections.Generic;
using System.Text;
using UassetToolkit.UPropertyTypes;

namespace UassetToolkit.UStructTypes
{
    public class GuidStruct : UStruct
    {
        public override void ReadStruct(IOMemoryStream ms, UAssetFile f, StructProperty s)
        {
            ms.ReadInt();
            ms.ReadInt();
            ms.ReadInt();
            ms.ReadInt();
        }

        public override string WriteString()
        {
            return "";
        }
    }
}
