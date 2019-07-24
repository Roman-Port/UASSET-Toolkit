using System;
using System.Collections.Generic;
using System.Text;
using UassetToolkit.UPropertyTypes;

namespace UassetToolkit.UStructTypes
{
    public class StringAssetReferenceStruct : UStruct
    {
        public string name;

        public override void ReadStruct(IOMemoryStream ms, UAssetFile f, StructProperty s)
        {
            name = ms.ReadUEString();
        }

        public override string WriteString()
        {
            return $"name={name}";
        }
    }
}
