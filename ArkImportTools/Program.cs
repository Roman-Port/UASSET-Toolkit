using ArkImportTools.OutputEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UassetToolkit;

namespace ArkImportTools
{
    class Program
    {
        public const string GAME_ROOT_PATH = @"E:\Programs\ARKEditor\Projects\ShooterGame\Content\";

        static void Main(string[] args)
        {
            DinoImporter.ImportDinos();

            MultipleFileTest();

            UAssetFileBlueprint f = UAssetFileBlueprint.OpenFile(@"E:\Programs\ARKEditor\Projects\ShooterGame\Content\Aberration\Dinos\RockDrake\RockDrake_Character_BP.uasset", false, "RockDrake_Character_BP", @"E:\Programs\ARKEditor\Projects\ShooterGame\Content\");
            UAssetCacheBlock cache = new UAssetCacheBlock();

            ArkDinoEntry e = ArkDinoEntry.Convert(f, cache);
            Console.ReadLine();
        }

        static void MultipleFileTest()
        {
            //SingleFileTestBlueprint(@"Aberration\Dinos\RockDrake\RockDrake_Character_BP.uasset", "RockDrake_Character_BP");
            SingleFileTestBlueprint(@"PrimalEarth\Dinos\Argentavis\Argent_Character_BP.uasset", "Argent_Character_BP");
            //SingleFileTestImage(@"PrimalEarth\UI\Textures\HubInventoryIcon.uasset", "HubInventoryIcon");

            Console.WriteLine("Done ALL");
            Console.ReadLine();
        }

        static void SingleFileTestBlueprint(string path, string classname)
        {
            Console.WriteLine("Testing");
            using (FileStream fs = new FileStream(GAME_ROOT_PATH + path, FileMode.Open, FileAccess.Read))
            {
                UAssetFileBlueprint f = UAssetFileBlueprint.OpenFile(fs, true, classname, GAME_ROOT_PATH);
                UAssetCacheBlock cache = new UAssetCacheBlock();
                List<UProperty> props = f.GetFullProperties(cache);
                foreach (var p in props)
                    Console.WriteLine($"{p.name}, {p.type}, {p.WriteString()}");

                //File.WriteAllText("E:\\rockdrake.json", JsonConvert.SerializeObject(props, Formatting.Indented));
                //ArkDinoEntry e = ArkDinoEntry.Convert(f, cache);
            }
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        static void SingleFileTestImage(string path, string classname)
        {
            Console.WriteLine("Testing");
            using (FileStream fs = new FileStream(GAME_ROOT_PATH + path, FileMode.Open, FileAccess.Read))
            {
                UAssetFileTexture2D f = UAssetFileTexture2D.OpenFile(fs, true, classname, GAME_ROOT_PATH);
                UAssetCacheBlock cache = new UAssetCacheBlock();
                //List<UProperty> props = f.GetFullProperties(cache);
                /*foreach (var p in props)
                    Console.WriteLine($"{p.name}, {p.type}, {p.WriteString()}");*/

                //File.WriteAllText("E:\\rockdrake.json", JsonConvert.SerializeObject(props, Formatting.Indented));
                //ArkDinoEntry e = ArkDinoEntry.Convert(f, cache);
            }
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
