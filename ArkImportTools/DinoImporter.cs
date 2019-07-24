using ArkImportTools.Entities;
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
    public static class DinoImporter
    {
        public static void ImportDinos(UAssetCacheBlock cache, Dictionary<string, string> map, List<string> readErrors)
        {
            //Open PrimalGameData
            Console.WriteLine("Opening PrimalGameData...");
            UAssetFileBlueprint primalGameData = UAssetFileBlueprint.OpenFile(Program.GAME_ROOT_PATH + @"PrimalEarth\CoreBlueprints\PrimalGameData_BP.uasset", false, "PrimalGameData_BP", Program.GAME_ROOT_PATH);
            PropertyReader primalGameDataReader = new PropertyReader(primalGameData.GetFullProperties(cache));
            Console.WriteLine("PrimalGameData opened.");

            //Now, open all dino entries
            Console.WriteLine("Opening dino entries...");
            Dictionary<string, PropertyReader> dinoEntries = new Dictionary<string, PropertyReader>();
            ArrayProperty entriesArray = primalGameDataReader.GetProperty<ArrayProperty>("DinoEntries");
            int entriesCount = 0;
            foreach (var en in entriesArray.props)
            {
                //Open
                UAssetFileBlueprint bp = ((ObjectProperty)en).GetReferencedFileBlueprint();
                PropertyReader bpReader = new PropertyReader(bp.GetFullProperties(cache));
                string name = bpReader.GetPropertyStringOrName("DinoNameTag");
                if (dinoEntries.ContainsKey(name))
                    dinoEntries.Remove(name);
                dinoEntries.Add(name, bpReader);
                entriesCount++;
            }
            Console.WriteLine($"Finished opening dino entries. Found {entriesCount} entries.");

            //Now, open binder
            ClassListBinder binder = ClassListBinder.OpenBinder("classes.json");

            //Find base dino
            ClassListEntry dinoBase = binder.ClassData.SearchForClass("Dino_Character_BP");
            if (dinoBase == null)
                throw new Exception("Failed to find dino base class.");

            //Get children
            List<ClassListEntry> dinoClasses = dinoBase.GetAllChildren();
            Console.WriteLine($"Found {dinoClasses.Count} dino classes to import.");

            //Loop classes and start reading
            List<ArkDinoEntry> dinos = new List<ArkDinoEntry>();
            foreach(ClassListEntry e in dinoClasses)
            {
                try
                {
                    //Find this dino file in the map
                    if (!map.ContainsKey(e.Name))
                        throw new Exception($"Failed to find dino {e.Name} in uasset map.");
                    string pathname = map[e.Name];

                    //Now, open the UASSET file
                    UAssetFileBlueprint f = UAssetFileBlueprint.OpenFile(pathname, false, e.Name, Program.GAME_ROOT_PATH);

                    //Create a dino entry
                    ArkDinoEntry entry = ArkDinoEntry.Convert(f, cache, dinoEntries);
                    dinos.Add(entry);
                } catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Failed to import dino {e.Name} with error {ex.Message}!");
                    readErrors.Add($"Failed to import dino {e.Name} with error {ex.Message}{ex.StackTrace}!");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            //Now, save
            Console.WriteLine($"Finished reading {dinos.Count}/{dinoClasses.Count} dinos. Saving now...");
            File.WriteAllText(Program.GetOutputDir()+"dinos.json", JsonConvert.SerializeObject(dinos, Formatting.Indented));

            Console.WriteLine("Done importing dinos.");
        }
    }
}
