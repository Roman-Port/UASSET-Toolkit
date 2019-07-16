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
            entry.baseLevel = ReadStatsArray("MaxStatusValues", reader, entry, DEFAULT_BASE_LEVEL);
            entry.increasePerWildLevel = ReadStatsArray("AmountMaxGainedPerLevelUpValue", reader, entry, DEFAULT_INCREASE_PER_WILD_LEVEL);
            entry.increasePerTamedLevel = ReadStatsArray("AmountMaxGainedPerLevelUpValueTamed", reader, entry, DEFAULT_INCREASE_PER_TAMED_LEVEL);
            entry.additiveTamingBonus = ReadStatsArray("TamingMaxStatAdditions", reader, entry, DEFAULT_TAMING_MAX_STAT_ADDITIONS);
            entry.multiplicativeTamingBonus = ReadStatsArray("TamingMaxStatMultipliers", reader, entry, DEFAULT_TAMING_MAX_STAT_MULTIPLY);
        }

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
