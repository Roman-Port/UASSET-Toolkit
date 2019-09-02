using ArkImportTools.OutputEntities;
using System;
using System.Collections.Generic;
using System.Text;
using UassetToolkit;

namespace ArkImportTools
{
    public static class ArkStatsRipper
    {
        public static void DoRipStats(PropertyReader reader, ArkDinoEntry entry)
        {
            /*entry.baseLevel = ReadStatsArray("MaxStatusValues", reader, entry, DEFAULT_BASE_LEVEL);
            entry.increasePerWildLevel = ReadStatsArray("AmountMaxGainedPerLevelUpValue", reader, entry, DEFAULT_INCREASE_PER_WILD_LEVEL);
            entry.increasePerTamedLevel = ReadStatsArray("AmountMaxGainedPerLevelUpValueTamed", reader, entry, DEFAULT_INCREASE_PER_TAMED_LEVEL);
            entry.additiveTamingBonus = ReadStatsArray("TamingMaxStatAdditions", reader, entry, DEFAULT_TAMING_MAX_STAT_ADDITIONS);
            entry.multiplicativeTamingBonus = ReadStatsArray("TamingMaxStatMultipliers", reader, entry, DEFAULT_TAMING_MAX_STAT_MULTIPLY);*/

            //Create dicts
            entry.baseLevel = new Dictionary<DinoStatTypeIndex, float>();
            entry.increasePerWildLevel = new Dictionary<DinoStatTypeIndex, float>();
            entry.increasePerTamedLevel = new Dictionary<DinoStatTypeIndex, float>();
            entry.additiveTamingBonus = new Dictionary<DinoStatTypeIndex, float>();
            entry.multiplicativeTamingBonus = new Dictionary<DinoStatTypeIndex, float>();

            //Loop through ARK indexes
            for(int i = 0; i<=11; i++)
            {
                //Convert to our standard stat
                DinoStatTypeIndex stat = (DinoStatTypeIndex)i;

                //Calculate multipliers
                bool can_level = true;// (i == 2) || (reader.GetPropertyByte("CanLevelUpValue", CANLEVELUP_VALUES[i], i) == 1);
                int add_one = IS_PERCENT_STAT[i];
                float zero_mult = can_level ? 1 : 0;
                float ETHM = reader.GetPropertyFloat("ExtraTamedHealthMultiplier", EXTRA_MULTS_VALUES[i], i);

                //Add stat data
                entry.baseLevel.Add(stat, MathF.Round(reader.GetPropertyFloat("MaxStatusValues", BASE_VALUES[i], i) + add_one, ROUND_PERCISION));
                entry.increasePerWildLevel.Add(stat, MathF.Round(reader.GetPropertyFloat("AmountMaxGainedPerLevelUpValue", IW_VALUES[i], i) * zero_mult, ROUND_PERCISION));
                entry.increasePerTamedLevel.Add(stat, MathF.Round(reader.GetPropertyFloat("AmountMaxGainedPerLevelUpValueTamed", 0, i) * ETHM * zero_mult, ROUND_PERCISION));
                entry.additiveTamingBonus.Add(stat, MathF.Round(reader.GetPropertyFloat("TamingMaxStatAdditions", 0, i), ROUND_PERCISION));
                entry.multiplicativeTamingBonus.Add(stat, MathF.Round(reader.GetPropertyFloat("TamingMaxStatMultipliers", 0, i), ROUND_PERCISION));
            }
        }

        public const int ROUND_PERCISION = 6;

        /* New defaults */
        //https://github.com/arkutils/Purlovia/blob/f25dd80a06930f0d34beacd03dafc5f9cecb054e/ark/defaults.py
        public const float FEMALE_MINTIMEBETWEENMATING_DEFAULT = 64800.0f;
        public const float FEMALE_MAXTIMEBETWEENMATING_DEFAULT = 172800.0f;

        public const float BABYGESTATIONSPEED_DEFAULT = 0.000035f;

        public static readonly float[] BASE_VALUES = new float[] { 100, 100, 100, 100, 100, 100, 0, 0, 0, 0, 0, 0 };
        public static readonly float[] IW_VALUES = new float[] {0, 0, 0.06f, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static readonly float[] IMPRINT_VALUES = new float[] {0.2f, 0, 0.2f, 0, 0.2f, 0.2f, 0, 0.2f, 0.2f, 0.2f, 0, 0};
        public static readonly float[] EXTRA_MULTS_VALUES = new float[] {1.35f, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
        public static readonly float[] DONTUSESTAT_VALUES = new float[] {0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1};
        public static readonly byte[] CANLEVELUP_VALUES = new byte[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        public static readonly int[] IS_PERCENT_STAT = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1};

        static Dictionary<DinoStatTypeIndex, float> ReadStatsArray(string name, PropertyReader reader, ArkDinoEntry entry, Dictionary<DinoStatTypeIndex, float> defaults)
        {
            return new Dictionary<DinoStatTypeIndex, float>
            {
                { DinoStatTypeIndex.Health,                 reader.GetPropertyFloat(name, defaults[DinoStatTypeIndex.Health], (int)DinoStatTypeIndex.Health) },
                { DinoStatTypeIndex.Stamina,                reader.GetPropertyFloat(name, defaults[DinoStatTypeIndex.Stamina], (int)DinoStatTypeIndex.Stamina) },
                { DinoStatTypeIndex.Torpidity,              reader.GetPropertyFloat(name, defaults[DinoStatTypeIndex.Torpidity], (int)DinoStatTypeIndex.Torpidity) },
                { DinoStatTypeIndex.Oxygen,                 reader.GetPropertyFloat(name, defaults[DinoStatTypeIndex.Oxygen], (int)DinoStatTypeIndex.Oxygen) },
                { DinoStatTypeIndex.Food,                   reader.GetPropertyFloat(name, defaults[DinoStatTypeIndex.Food], (int)DinoStatTypeIndex.Food) },
                { DinoStatTypeIndex.Water,                  reader.GetPropertyFloat(name, defaults[DinoStatTypeIndex.Water], (int)DinoStatTypeIndex.Water) },
                { DinoStatTypeIndex.Temperature,            reader.GetPropertyFloat(name, defaults[DinoStatTypeIndex.Temperature], (int)DinoStatTypeIndex.Temperature) },
                { DinoStatTypeIndex.Weight,                 reader.GetPropertyFloat(name, defaults[DinoStatTypeIndex.Weight], (int)DinoStatTypeIndex.Weight) },
                { DinoStatTypeIndex.MeleeDamage,            reader.GetPropertyFloat(name, defaults[DinoStatTypeIndex.MeleeDamage], (int)DinoStatTypeIndex.MeleeDamage) },
                { DinoStatTypeIndex.Speed,                  reader.GetPropertyFloat(name, defaults[DinoStatTypeIndex.Speed], (int)DinoStatTypeIndex.Speed) },
                { DinoStatTypeIndex.TemperatureFortitude,   reader.GetPropertyFloat(name, defaults[DinoStatTypeIndex.TemperatureFortitude], (int)DinoStatTypeIndex.TemperatureFortitude) },
                { DinoStatTypeIndex.CraftingSpeed,          reader.GetPropertyFloat(name, defaults[DinoStatTypeIndex.CraftingSpeed], (int)DinoStatTypeIndex.CraftingSpeed) }
            };
        }

        /*
         * DEFAULTS
         */

        public static readonly Dictionary<DinoStatTypeIndex, float> BASE_DEFAULT = new Dictionary<DinoStatTypeIndex, float> {
            { DinoStatTypeIndex.Health, 0f },
            { DinoStatTypeIndex.Stamina, 0f },
            { DinoStatTypeIndex.Torpidity, 0f },
            { DinoStatTypeIndex.Oxygen, 0f },
            { DinoStatTypeIndex.Food, 0f },
            { DinoStatTypeIndex.Water, 0f },
            { DinoStatTypeIndex.Temperature, 0f },
            { DinoStatTypeIndex.Weight, 0f },
            { DinoStatTypeIndex.MeleeDamage, 0f },
            { DinoStatTypeIndex.Speed, 0f },
            { DinoStatTypeIndex.TemperatureFortitude, 0f },
            { DinoStatTypeIndex.CraftingSpeed, 0f }
        };

        public static readonly Dictionary<DinoStatTypeIndex, float> DEFAULT_BASE_LEVEL = new Dictionary<DinoStatTypeIndex, float> {
            { DinoStatTypeIndex.Health, 100f },
            { DinoStatTypeIndex.Stamina, 100f },
            { DinoStatTypeIndex.Torpidity, 100f },
            { DinoStatTypeIndex.Oxygen, 150f },
            { DinoStatTypeIndex.Food, 100f },
            { DinoStatTypeIndex.Water, 100f },
            { DinoStatTypeIndex.Temperature, 0f },
            { DinoStatTypeIndex.Weight, 100f },
            { DinoStatTypeIndex.MeleeDamage, 0f },
            { DinoStatTypeIndex.Speed, 0f },
            { DinoStatTypeIndex.TemperatureFortitude, 0f },
            { DinoStatTypeIndex.CraftingSpeed, 0f }
        };

        public static readonly Dictionary<DinoStatTypeIndex, float> DEFAULT_INCREASE_PER_WILD_LEVEL = new Dictionary<DinoStatTypeIndex, float> {
            { DinoStatTypeIndex.Health, 0.2f },
            { DinoStatTypeIndex.Stamina, 0.1f },
            { DinoStatTypeIndex.Torpidity, 0f },
            { DinoStatTypeIndex.Oxygen, 0.1f },
            { DinoStatTypeIndex.Food, 0.1f },
            { DinoStatTypeIndex.Water, 0.1f },
            { DinoStatTypeIndex.Temperature, 0f },
            { DinoStatTypeIndex.Weight, 0.02f },
            { DinoStatTypeIndex.MeleeDamage, 0.05f },
            { DinoStatTypeIndex.Speed, 0f },
            { DinoStatTypeIndex.TemperatureFortitude, 0f },
            { DinoStatTypeIndex.CraftingSpeed, 0f }
        };

        public static readonly Dictionary<DinoStatTypeIndex, float> DEFAULT_INCREASE_PER_TAMED_LEVEL = new Dictionary<DinoStatTypeIndex, float> {
            { DinoStatTypeIndex.Health, 0.2f },
            { DinoStatTypeIndex.Stamina, 0.1f },
            { DinoStatTypeIndex.Torpidity, 0f },
            { DinoStatTypeIndex.Oxygen, 0.1f },
            { DinoStatTypeIndex.Food, 0.1f },
            { DinoStatTypeIndex.Water, 0.1f },
            { DinoStatTypeIndex.Temperature, 0f },
            { DinoStatTypeIndex.Weight, 0.04f },
            { DinoStatTypeIndex.MeleeDamage, 0.1f },
            { DinoStatTypeIndex.Speed, 0.01f },
            { DinoStatTypeIndex.TemperatureFortitude, 0f },
            { DinoStatTypeIndex.CraftingSpeed, 0f }
        };

        public static readonly Dictionary<DinoStatTypeIndex, float> DEFAULT_TAMING_MAX_STAT_ADDITIONS = new Dictionary<DinoStatTypeIndex, float> {
            { DinoStatTypeIndex.Health, 0.5f },
            { DinoStatTypeIndex.Stamina, 0f },
            { DinoStatTypeIndex.Torpidity, 0.5f },
            { DinoStatTypeIndex.Oxygen, 0f },
            { DinoStatTypeIndex.Food, 0f },
            { DinoStatTypeIndex.Water, 0f },
            { DinoStatTypeIndex.Temperature, 0f },
            { DinoStatTypeIndex.Weight, 0f },
            { DinoStatTypeIndex.MeleeDamage, 0.5f },
            { DinoStatTypeIndex.Speed, 0f },
            { DinoStatTypeIndex.TemperatureFortitude, 0f },
            { DinoStatTypeIndex.CraftingSpeed, 0f }
        };

        public static readonly Dictionary<DinoStatTypeIndex, float> DEFAULT_TAMING_MAX_STAT_MULTIPLY = new Dictionary<DinoStatTypeIndex, float> {
            { DinoStatTypeIndex.Health, 0.0f },
            { DinoStatTypeIndex.Stamina, 0f },
            { DinoStatTypeIndex.Torpidity, 0.0f },
            { DinoStatTypeIndex.Oxygen, 0f },
            { DinoStatTypeIndex.Food, 0f },
            { DinoStatTypeIndex.Water, 0f },
            { DinoStatTypeIndex.Temperature, 0f },
            { DinoStatTypeIndex.Weight, 0f },
            { DinoStatTypeIndex.MeleeDamage, 0.4f },
            { DinoStatTypeIndex.Speed, 0f },
            { DinoStatTypeIndex.TemperatureFortitude, 0f },
            { DinoStatTypeIndex.CraftingSpeed, 0f }
        };
    }
}
