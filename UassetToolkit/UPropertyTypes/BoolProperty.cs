using System;
using System.Collections.Generic;
using System.Text;

namespace UassetToolkit.UPropertyTypes
{
    public class BoolProperty : UProperty
    {
        public bool flag;

        public BoolProperty(IOMemoryStream ms, UAssetFile f) : base(ms, f)
        {

        }

        public override void Read(IOMemoryStream ms, UAssetFile f)
        {
            flag = ms.ReadByte() != 0;
        }

        public override string WriteString()
        {
            return $"flag={flag.ToString()}";
        }
    }
}
