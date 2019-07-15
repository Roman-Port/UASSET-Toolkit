using System;
using System.Collections.Generic;
using System.Text;

namespace UassetToolkit.UPropertyTypes
{
    public class TextProperty : UProperty
    {
        public TextProperty(IOMemoryStream ms, UAssetFile f) : base(ms, f)
        {

        }

        public string data;

        public override void Read(IOMemoryStream ms, UAssetFile f)
        {
            //Not really sure how this works....this is just what my original program written 6 months ago does
            data = Convert.ToBase64String(ms.ReadBytes(length));
        }

        public override string WriteString()
        {
            return $"data={data}";
        }
    }
}
