using System;
using System.Collections.Generic;
using System.Text;
using UassetToolkit.UPropertyTypes;

namespace UassetToolkit.UStructTypes
{
    public class UniqueNetIdStruct : UStruct
    {
        public int unk;
        public string netId;

        public override void ReadStruct(IOMemoryStream ms, UAssetFile f, StructProperty s)
        {
            unk = ms.ReadInt();
            netId = ms.ReadUEString();
        }

        public override string WriteString()
        {
            return $"unk={unk}, netId={netId}";
        }
    }
}
