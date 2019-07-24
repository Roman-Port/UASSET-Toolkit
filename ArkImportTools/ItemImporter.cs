using ArkImportTools.OutputEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UassetToolkit;
using UassetToolkit.UPropertyTypes;

namespace ArkImportTools
{
    public static class ItemImporter
    {
        public static void ImportItems(UAssetCacheBlock cache, Dictionary<string, string> map, List<string> readErrors)
        {
            //Find items to import
            List<ArkItemEntry> items = new List<ArkItemEntry>();
            foreach(var mapEntry in map)
            {
                //Check if this is an item
                if (!mapEntry.Key.StartsWith("PrimalItem"))
                    continue;

                //Open 
                UAssetFileBlueprint bp;
                try
                {
                    bp = UAssetFileBlueprint.OpenFile(mapEntry.Value, false, mapEntry.Key, Program.GAME_ROOT_PATH);
                } catch (Exception ex)
                {
                    Console.WriteLine($"FAILED TO READ ITEM {mapEntry.Key} with error {ex.Message}");
                    readErrors.Add($"FAILED TO READ ITEM {mapEntry.Key} with error {ex.Message} {ex.StackTrace}");
                    continue;
                }

                //Decode
                try
                {
                    ArkItemEntry entry = ArkItemEntry.ConvertEntry(bp, cache);
                    items.Add(entry);
                } catch (Exception ex)
                {
                    Console.WriteLine("FAILED TO IMPORT ITEM " + bp.classname);
                    readErrors.Add($"FAILED TO IMPORT ITEM {bp.classname} {ex.Message} {ex.StackTrace}");
                    continue;
                }
            }

            //OK
            Console.WriteLine($"Imported {items.Count} items.");

            //Save
            File.WriteAllText(Program.GetOutputDir() + "items.json", JsonConvert.SerializeObject(items, Formatting.Indented));
            Console.WriteLine("Saved.");
        }
    }
}
