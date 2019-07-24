using System;
using System.Collections.Generic;
using System.Text;
using UassetToolkit.UPropertyTypes;

namespace UassetToolkit.UStructTypes
{
    /// <summary>
    /// Reads structs into a list
    /// </summary>
    public class PropListStruct : UStruct
    {
        public Dictionary<string, UProperty> props; //Could be unreliable
        public List<UProperty> propsList;
        public int count;
        public int u1;
        public int u2;

        public override void ReadStruct(IOMemoryStream ms, UAssetFile f, StructProperty s)
        {
            //Read into array
            List<UProperty> sprops = UProperty.ReadProperties(ms, f, null, true);
            count = sprops.Count;
            propsList = sprops;
            props = new Dictionary<string, UProperty>();

            //Convert to dict
            foreach (UProperty p in sprops)
            {
                if(!props.ContainsKey(p.name))
                    props.Add(p.name, p);
            }
                

            //Read two unknown ints
            u1 = ms.ReadInt();
        }

        public override string WriteString()
        {
            return $"propCount={count}";
        }
    }
}
