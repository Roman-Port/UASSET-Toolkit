using System;
using System.Collections.Generic;
using System.Text;
using UassetToolkit;
using UassetToolkit.UPropertyTypes;
using UassetToolkit.UStructTypes;

namespace ArkImportTools.OutputEntities
{
    public class ArkItemEntry
    {
        public string classname;
        public string blueprintPath;

        public ArkImage icon;
        public ArkImage broken_icon;

        public int captureTime;

        public bool hideFromInventoryDisplay { get; set; }
        public bool useItemDurability { get; set; }
        public bool isTekItem { get; set; }
        public bool allowUseWhileRiding { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public float spoilingTime { get; set; } //0 if not spoiling
        public float baseItemWeight { get; set; }
        public float useCooldownTime { get; set; }
        public float baseCraftingXP { get; set; }
        public float baseRepairingXP { get; set; }
        public int maxItemQuantity { get; set; }

        //Consumables
        public Dictionary<string, ArkItemEntry_ConsumableAddStatusValue> addStatusValues;

        public static ArkItemEntry ConvertEntry(UAssetFileBlueprint bp, UAssetCacheBlock cache)
        {
            //Open reader
            PropertyReader reader = new PropertyReader(bp.GetFullProperties(cache));

            //Get primary icon
            ArkImage icon;
            if (reader.GetProperty<ObjectProperty>("ItemIcon") != null)
                icon = ImageTool.QueueImage(reader.GetProperty<ObjectProperty>("ItemIcon").GetReferencedFile(), ImageTool.ImageModifications.None);
            else
                icon = ArkImage.MISSING_ICON;

            //Get broken icon
            ArkImage brokenIcon;
            if (reader.GetProperty<ObjectProperty>("BrokenImage") != null)
                brokenIcon = ImageTool.QueueImage(reader.GetProperty<ObjectProperty>("BrokenImage").GetReferencedFile(), ImageTool.ImageModifications.None);
            else
                brokenIcon = ArkImage.MISSING_ICON;

            //Get the array of UseItemAddCharacterStatusValues
            ArrayProperty statusValuesArray = reader.GetProperty<ArrayProperty>("UseItemAddCharacterStatusValues");
            Dictionary<string, ArkItemEntry_ConsumableAddStatusValue> statusValues = new Dictionary<string, ArkItemEntry_ConsumableAddStatusValue>();
            if (statusValuesArray != null)
            {
                foreach(var i in statusValuesArray.props)
                {
                    StructProperty sv = (StructProperty)i;
                    var svp = ((PropListStruct)sv.data).propsList;
                    var svpr = new PropertyReader(svp);
                    string type = svpr.GetProperty<ByteProperty>("StatusValueType").enumValue;
                    ArkItemEntry_ConsumableAddStatusValue sve = ArkItemEntry_ConsumableAddStatusValue.Convert(svpr, type);
                    statusValues.Add(type, sve);
                }
            }

            //Get time
            int time = (int)Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);

            //Create
            ArkItemEntry e = new ArkItemEntry
            {
                hideFromInventoryDisplay = reader.GetPropertyBool("bHideFromInventoryDisplay", false),
                useItemDurability = reader.GetPropertyBool("bUseItemDurability", false),
                isTekItem = reader.GetPropertyBool("bTekItem", false),
                allowUseWhileRiding = reader.GetPropertyBool("bAllowUseWhileRiding", false),
                name = reader.GetPropertyString("DescriptiveNameBase", null),
                description = reader.GetPropertyString("ItemDescription", null),
                spoilingTime = reader.GetPropertyFloat("SpolingTime", 0),
                baseItemWeight = reader.GetPropertyFloat("BaseItemWeight", 0),
                useCooldownTime = reader.GetPropertyFloat("MinimumUseInterval", 0),
                baseCraftingXP = reader.GetPropertyFloat("BaseCraftingXP", 0),
                baseRepairingXP = reader.GetPropertyFloat("BaseRepairingXP", 0),
                maxItemQuantity = reader.GetPropertyInt("MaxItemQuantity", 0),
                classname = bp.classname+"_C",
                blueprintPath = "N/A",
                icon = icon,
                broken_icon = brokenIcon,
                addStatusValues = statusValues,
                captureTime = time
            };
            return e;
        }
    }

    public class ArkItemEntry_ConsumableAddStatusValue
    {
        public float baseAmountToAdd { get; set; }
        public bool percentOfMaxStatusValue { get; set; }
        public bool percentOfCurrentStatusValue { get; set; }
        public bool useItemQuality { get; set; }
        public bool addOverTime { get; set; }
        public bool setValue { get; set; }
        public bool setAdditionalValue { get; set; }
        public float addOverTimeSpeed { get; set; }
        public float itemQualityAddValueMultiplier { get; set; }

        public string statusValueType; //Enum


        public static ArkItemEntry_ConsumableAddStatusValue Convert(PropertyReader reader, string type)
        {
            return new ArkItemEntry_ConsumableAddStatusValue
            {
                baseAmountToAdd = reader.GetPropertyFloat("BaseAmountToAdd", null),
                percentOfMaxStatusValue = reader.GetPropertyBool("bPercentOfMaxStatusValue", null),
                percentOfCurrentStatusValue = reader.GetPropertyBool("bPercentOfCurrentStatusValue", null),
                useItemQuality = reader.GetPropertyBool("bUseItemQuality", null),
                addOverTime = reader.GetPropertyBool("bAddOverTime", null),
                setValue = reader.GetPropertyBool("bSetValue", null),
                setAdditionalValue = reader.GetPropertyBool("bSetAdditionalValue", null),
                addOverTimeSpeed = reader.GetPropertyFloat("AddOverTimeSpeed", null),
                itemQualityAddValueMultiplier = reader.GetPropertyFloat("ItemQualityAddValueMultiplier", null),
                statusValueType = type
            };
        }
    }
}
