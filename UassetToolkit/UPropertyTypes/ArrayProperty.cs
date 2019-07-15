using System;
using System.Collections.Generic;
using System.Text;

namespace UassetToolkit.UPropertyTypes
{
    public class ArrayProperty : UProperty
    {
        public ArrayProperty(IOMemoryStream ms, UAssetFile f) : base(ms, f)
        {

        }

        public string arrayType;
        public int unknownArray;
        public List<UProperty> props;

        public override void Read(IOMemoryStream ms, UAssetFile f)
        {
            //Read the array type
            arrayType = ms.ReadNameTableEntry(f);
            unknownArray = ms.ReadInt();
            props = new List<UProperty>();

            /*//Skip for now
            ms.position += length;
            return;*/

            //Now, read the count. This cuts into the length
            int count = ms.ReadInt();

            //Read items
            Console.WriteLine($"====READ ARRAY BEGIN @ {ms.position} ({arrayType}, {unknownArray})====");
            for (int i = 0; i<count; i+=1)
            {
                //Read value
                props.Add(UProperty.ReadProp(ms, f, arrayType, false));

                //Read unknown flag
                /*int flag = ms.ReadInt();
                if (flag != 0)
                    Console.WriteLine("Flag is != 0");*/
            }
            Console.WriteLine($"====READ ARRAY END @ {ms.position}====");
        }

        public override string WriteString()
        {
            return $"arrayType={arrayType}, unknownArray={unknownArray}, arrayElementCount={props.Count}";
        }
    }
}
