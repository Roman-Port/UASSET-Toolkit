using System;
using System.Collections.Generic;
using System.Text;
using UassetToolkit;
using UassetToolkit.UPropertyTypes;
using UassetToolkit.UStructTypes;

namespace ArkImportTools.OutputEntities
{
    public class ArkDinoEntry
    {
        public string screen_name;
        public float colorizationIntensity;
        public float babyGestationSpeed;
        public float extraBabyGestationSpeedMultiplier;
        public float babyAgeSpeed;
        public float extraBabyAgeMultiplier;
        public bool useBabyGestation;

        public ArkDinoEntryStatus statusComponent;

        public List<ArkDinoFood> adultFoods;
        public List<ArkDinoFood> childFoods;

        public string classname;
        public string blueprintPath;

        public ArkImage icon;

        public Dictionary<DinoStatTypeIndex, float> baseLevel;
        public Dictionary<DinoStatTypeIndex, float> increasePerWildLevel;
        public Dictionary<DinoStatTypeIndex, float> increasePerTamedLevel;
        public Dictionary<DinoStatTypeIndex, float> additiveTamingBonus; //Taming effectiveness
        public Dictionary<DinoStatTypeIndex, float> multiplicativeTamingBonus; //Taming effectiveness

        public int captureTime;

        public static ArkDinoEntry Convert(UAssetFileBlueprint f, UAssetCacheBlock cache, Dictionary<string, PropertyReader> dinoEntries)
        {
            //Open reader
            PropertyReader reader = new PropertyReader(f.GetFullProperties(cache));

            //Get the dino settings
            UAssetFileBlueprint settingsFileAdult = ArkDinoFood.GetAdultFile(f, cache);
            UAssetFileBlueprint settingsFileBaby = ArkDinoFood.GetBabyFile(f, cache);

            //Get time
            int time = (int)Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);

            //Get status component
            UAssetFileBlueprint statusComponent = ArkDinoEntryStatus.GetFile(f, cache);
            PropertyReader statusReader = new PropertyReader(statusComponent.GetFullProperties(cache));

            //Use name tag to find entry
            string tag = reader.GetPropertyStringOrName("DinoNameTag");
            if (!dinoEntries.ContainsKey(tag))
                throw new Exception($"Could not find dino entry for '{f.classname}' (tag '{tag}')");
            PropertyReader entry = dinoEntries[tag];

            //Now, load the material used for the dino image
            UAssetFileMaterial entryMaterial = entry.GetProperty<ObjectProperty>("DinoMaterial").GetReferencedFileMaterial();
            UAssetFileMaterial.TextureParameterValue entryMaterialTexture = entryMaterial.textureParameters[0];
            ClassnamePathnamePair entryTexture = entryMaterialTexture.prop.GetReferencedFile();

            //Read
            ArkDinoEntry e = new ArkDinoEntry
            {
                screen_name = reader.GetPropertyString("DescriptiveName", null),
                colorizationIntensity = reader.GetPropertyFloat("ColorizationIntensity", 1),
                babyGestationSpeed = reader.GetPropertyFloat("BabyGestationSpeed", -1),
                extraBabyGestationSpeedMultiplier = reader.GetPropertyFloat("ExtraBabyGestationSpeedMultiplier", -1),
                babyAgeSpeed = reader.GetPropertyFloat("BabyAgeSpeed", null),
                extraBabyAgeMultiplier = reader.GetPropertyFloat("ExtraBabyAgeSpeedMultiplier", -1),
                useBabyGestation = reader.GetPropertyBool("bUseBabyGestation", false),
                statusComponent = ArkDinoEntryStatus.Convert(statusComponent, statusReader),
                adultFoods = ArkDinoFood.Convert(settingsFileAdult, cache),
                childFoods = ArkDinoFood.Convert(settingsFileBaby, cache),
                classname = f.classname,
                blueprintPath = "N/A",
                captureTime = time,
                icon = ImageTool.QueueImage(entryTexture)
            };

            //Finally, read stats
            ArkStatsRipper.DoRipStats(statusReader, e);

            return e;
        }
    }

    public class ArkDinoEntryStatus
    {
        public float baseFoodConsumptionRate;
        public float babyDinoConsumingFoodRateMultiplier;
        public float extraBabyDinoConsumingFoodRateMultiplier;
        public float foodConsumptionMultiplier;
        public float tamedBaseHealthMultiplier;

        public static UAssetFileBlueprint GetFile(UAssetFileBlueprint f, UAssetCacheBlock cache)
        {
            //Search for this by name
            GameObjectTableHead hr = null;
            UAssetFileBlueprint workingFile = f;
            while(hr == null && workingFile != null)
            {
                //Search
                foreach (var h in workingFile.gameObjectReferences)
                {
                    if (h.name.StartsWith("DinoCharacterStatusComponent_BP_"))
                        hr = h;
                }

                //Try to get the parent file
                workingFile = workingFile.GetParentBlueprint(cache);
            }
            if (hr == null)
                throw new Exception("Could not find dino status component!");

            //Open file
            string pathname = f.GetReferencedUAssetPathname(hr);
            return f.GetReferencedUAssetBlueprintFromPathname(pathname);
        }

        public static ArkDinoEntryStatus Convert(UAssetFileBlueprint f, PropertyReader reader)
        {
            return new ArkDinoEntryStatus
            {
                baseFoodConsumptionRate = reader.GetPropertyFloat("BaseFoodConsumptionRate", null),
                babyDinoConsumingFoodRateMultiplier = reader.GetPropertyFloat("BabyDinoConsumingFoodRateMultiplier", 25.5f),
                extraBabyDinoConsumingFoodRateMultiplier = reader.GetPropertyFloat("ExtraBabyDinoConsumingFoodRateMultiplier", 20),
                foodConsumptionMultiplier = reader.GetPropertyFloat("FoodConsumptionMultiplier", 1),
                tamedBaseHealthMultiplier = reader.GetPropertyFloat("TamedBaseHealthMultiplier", 1)
            };
        }
    }

    public class ArkDinoFood
    {
        public string classname;
        public float foodEffectivenessMultiplier;
        public float affinityOverride;
        public float affinityEffectivenessMultiplier;
        public int foodCategory;
        public float priority;

        public static UAssetFileBlueprint GetAdultFile(UAssetFileBlueprint f, UAssetCacheBlock cache)
        {
            //First, try to see if it's a property
            PropertyReader r = new PropertyReader(f.GetFullProperties(cache));
            ObjectProperty p = r.GetProperty<ObjectProperty>("AdultDinoSettings");
            if(p != null)
            {
                //Get file
                return p.GetReferencedFileBlueprint();
            }

            //Get the base DinoSettingsClass property
            p = r.GetProperty<ObjectProperty>("DinoSettingsClass");
            if(p != null)
            {
                //Get file
                return p.GetReferencedFileBlueprint();
            }

            //Throw error
            throw new Exception("Dino settings class was not found.");
        }

        public static UAssetFileBlueprint GetBabyFile(UAssetFileBlueprint f, UAssetCacheBlock cache)
        {
            //First, try to see if it's a property
            PropertyReader r = new PropertyReader(f.GetFullProperties(cache));
            ObjectProperty p = r.GetProperty<ObjectProperty>("BabyDinoSettings");
            if (p != null)
            {
                //Get file
                return p.GetReferencedFileBlueprint();
            }

            //Fallback to adult settings
            return GetAdultFile(f, cache);
        }

        public static List<ArkDinoFood> Convert(UAssetFileBlueprint f, UAssetCacheBlock cache)
        {
            //Open reader
            PropertyReader reader = new PropertyReader(f.GetFullProperties(cache));
            List<ArkDinoFood> output = new List<ArkDinoFood>();

            //Get each
            ArrayProperty mBase = reader.GetProperty<ArrayProperty>("FoodEffectivenessMultipliers");
            ArrayProperty mExtra = reader.GetProperty<ArrayProperty>("ExtraFoodEffectivenessMultipliers");

            //Convert
            if (mBase != null)
                output.AddRange(ConvertMultiplier(f, cache, mBase));
            if (mExtra != null)
                output.AddRange(ConvertMultiplier(f, cache, mExtra));

            return output;
        }

        private static List<ArkDinoFood> ConvertMultiplier(UAssetFileBlueprint f, UAssetCacheBlock cache, ArrayProperty p)
        {
            //Convert each entry
            List<ArkDinoFood> output = new List<ArkDinoFood>();
            foreach (var s in p.props)
            {
                StructProperty data = (StructProperty)s;
                PropListStruct sdata = (PropListStruct)data.data;
                PropertyReader reader = new PropertyReader(sdata.propsList);
                UAssetFileBlueprint foodClass = reader.GetProperty<ObjectProperty>("FoodItemParent").GetReferencedFileBlueprint();
                ArkDinoFood food = new ArkDinoFood
                {
                    classname = foodClass.classname,
                    foodEffectivenessMultiplier = reader.GetPropertyFloat("FoodEffectivenessMultiplier", null),
                    affinityOverride = reader.GetPropertyFloat("AffinityOverride", null),
                    affinityEffectivenessMultiplier = reader.GetPropertyFloat("AffinityEffectivenessMultiplier", null),
                    foodCategory = reader.GetPropertyInt("FoodItemCategory", null),
                    priority = reader.GetPropertyFloat("UntamedFoodConsumptionPriority", null)
                };
                output.Add(food);
            }
            return output;
        }
    }

    public enum DinoStatTypeIndex
    {
        Health = 0,
        Stamina = 1,
        Torpidity = 2,
        Oxygen = 3,
        Food = 4,
        Water = 5,
        Temperature = 6,
        Weight = 7,
        MeleeDamage = 8,
        Speed = 9,
        TemperatureFortitude = 10,
        CraftingSpeed = 11
    }
}
