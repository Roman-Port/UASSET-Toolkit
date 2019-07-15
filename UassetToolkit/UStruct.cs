using System;
using System.Collections.Generic;
using System.Text;
using UassetToolkit.UPropertyTypes;

namespace UassetToolkit
{
    public abstract class UStruct
    {
        public abstract void ReadStruct(IOMemoryStream ms, UAssetFile f, StructProperty s);
        public abstract string WriteString();
    }
}
