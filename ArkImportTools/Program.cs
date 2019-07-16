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

            UAssetFile f = UAssetFile.OpenFile(@"E:\Programs\ARKEditor\Projects\ShooterGame\Content\Aberration\Dinos\RockDrake\RockDrake_Character_BP.uasset", false, "RockDrake_Character_BP", @"E:\Programs\ARKEditor\Projects\ShooterGame\Content\");
            UAssetCacheBlock cache = new UAssetCacheBlock();

            ArkDinoEntry e = ArkDinoEntry.Convert(f, cache);
            Console.ReadLine();
        }

        static void MultipleFileTest()
        {
            //SingleFileTest(@"Aberration\Dinos\RockDrake\RockDrake_Character_BP.uasset", "RockDrake_Character_BP");
            SingleFileTest(@"PrimalEarth\Dinos\Argentavis\Argent_Character_BP.uasset", "Argent_Character_BP");

            Console.WriteLine("Done ALL");
            Console.ReadLine();
        }

        static void SingleFileTest(string path, string classname)
        {
            Console.WriteLine("Testing");

            UAssetFile f = UAssetFile.OpenFile(GAME_ROOT_PATH + path, false, classname, GAME_ROOT_PATH);
            UAssetCacheBlock cache = new UAssetCacheBlock();
            List<UProperty> props = f.GetFullProperties(cache);
            foreach (var p in props)
                Console.WriteLine($"{p.name}, {p.type}, {p.WriteString()}");

            //File.WriteAllText("E:\\rockdrake.json", JsonConvert.SerializeObject(props, Formatting.Indented));
            ArkDinoEntry e = ArkDinoEntry.Convert(f, cache);
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
