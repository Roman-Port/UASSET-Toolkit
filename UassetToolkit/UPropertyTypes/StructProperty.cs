using System;
using System.Collections.Generic;
using System.Text;
using UassetToolkit.UStructTypes;

namespace UassetToolkit.UPropertyTypes
{
    public class StructProperty : UProperty
    {
        public StructProperty(IOMemoryStream ms, UAssetFile f) : base(ms, f)
        {

        }

        public string structType;
        public int unknown;
        public UStruct data;

        public override void Read(IOMemoryStream ms, UAssetFile f)
        {
            //Is we're in an array, this will **ALWAYS** be a prop list struct.
            UStruct st;
            if (isArray)
            {
                structType = "(array)";
                unknown = 0;
                st = new PropListStruct();
                f.Debug("Read Array", $"====READ STRUCT BEGIN @ {ms.position} ({structType}, {unknown})====", ConsoleColor.Yellow);
            } else
            {
                structType = ms.ReadNameTableEntry(f);
                unknown = ms.ReadInt();
                f.Debug("Read Array", $"====READ STRUCT BEGIN @ {ms.position} ({structType}, {unknown})====", ConsoleColor.Yellow);

                //Find the struct type
                if (structType == "ItemNetID" || structType == "ItemNetInfo" || structType == "Transform" || structType == "PrimalPlayerDataStruct" || structType == "PrimalPlayerCharacterConfigStruct" || structType == "PrimalPersistentCharacterStatsStruct" || structType == "TribeData" || structType == "TribeGovernment" || structType == "TerrainInfo" || structType == "ArkInventoryData" || structType == "DinoOrderGroup" || structType == "ARKDinoData")
                {
                    //Open this as a struct property list.
                    st = new PropListStruct();
                }
                else if (structType == "Vector" || structType == "Rotator")
                {
                    //3d vector or rotor 
                    st = new Vector3Struct();
                }
                else if (structType == "Vector2D")
                {
                    //2d vector
                    st = new Vector2Struct();
                }
                else if (structType == "Quat")
                {
                    //Quat
                    st = new QuatStruct();
                }
                else if (structType == "Color")
                {
                    //Color
                    st = new ColorStruct();
                }
                else if (structType == "LinearColor")
                {
                    //Linear color
                    st = new LinearColorStruct();
                }
                else if (structType == "UniqueNetIdRepl")
                {
                    //Some net stuff
                    st = new UniqueNetIdStruct();
                }
                else if (structType == "Guid")
                {
                    //Some net stuff
                    st = new GuidStruct();
                }
                else if (structType == "IntPoint")
                {
                    //Some net stuff
                    st = new IntPointStruct();
                }
                else
                {
                    //Interpet this as a struct property list. Maybe raise a warning later?
                    f.Debug("Struct Warning", $"Unknown type '{structType}'. Interpeting as a struct property list...", ConsoleColor.Red);
                    st = new PropListStruct();
                }
            }

            //Read
            st.ReadStruct(ms, f, this);
            data = st;

            f.Debug("Read Struct", $"====READ STRUCT END @ {ms.position}====", ConsoleColor.Yellow);
        }

        public override string WriteString()
        {
            return $"structType={structType}, u={unknown}, "+data.WriteString();
        }
    }
}
