using ArkImportTools.Entities;
using ArkImportTools.OutputEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UassetToolkit;

namespace ArkImportTools
{
    public static class DinoImporter
    {
        public static void ImportDinos()
        {
            //First, map the game dir
            Dictionary<string, string> map = DirectoryIndexer.MapGameDir();

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
            UAssetCacheBlock cache = new UAssetCacheBlock();
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
                    UAssetFile f = UAssetFile.OpenFile(pathname, false, e.Name, Program.GAME_ROOT_PATH);

                    //Create a dino entry
                    ArkDinoEntry entry = ArkDinoEntry.Convert(f, cache);
                    dinos.Add(entry);
                } catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Failed to import dino {e.Name} with error {ex.Message}!");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            //Now, save
            Console.WriteLine($"Finished reading {dinos.Count}/{dinoClasses.Count} dinos. Saving now...");
            File.WriteAllText("E:\\ark_class_data_dinos.json", JsonConvert.SerializeObject(dinos, Formatting.Indented));

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
