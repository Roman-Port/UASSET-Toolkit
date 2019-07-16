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

            //Skip if this is a ByteProperty
            if(arrayType == "ByteProperty")
            {
                f.Warn("ArrayProperty", "Warning: ArrayProperty is skipping array because ByteProperty arrays are not supported at this time.");
                ms.position += length;
                return;
            }

            long begin = ms.position;

            try
            {
                //Now, read the count. This cuts into the length
                int count = ms.ReadInt();

                //Read items
                f.Debug("Read Array", $"====READ ARRAY BEGIN @ {ms.position} ({arrayType}, {unknownArray})====", ConsoleColor.Yellow);
                for (int i = 0; i < count; i += 1)
                {
                    //Read value
                    props.Add(UProperty.ReadProp(ms, f, arrayType, false));
                }
                f.Debug("Read Array", $"====READ ARRAY END @ {ms.position}====", ConsoleColor.Yellow);
            } catch (Exception ex)
            {
                f.Warn("ArrayProperty", $"Warning: Failed to read array '{name}' with type '{type}' in class '{f.classname}' for '{ex.Message}'. It will now be skipped.");
                ms.position = begin + length;
            }
        }

        public override string WriteString()
        {
            return $"arrayType={arrayType}, unknownArray={unknownArray}, arrayElementCount={props.Count}";
        }
    }
}
