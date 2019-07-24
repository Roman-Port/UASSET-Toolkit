using ArkImportTools.OutputEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UassetToolkit;

namespace ArkImportTools
{
    class Program
    {
        public const string GAME_ROOT_PATH = @"E:\Programs\ARKEditor\Projects\ShooterGame\Content\";
        public const string OUTPUT_PATH = @"E:\ArkExportV2\";

        public static string revision = null;

        static void Main(string[] args)
        {
            DoRun();

            //MultipleFileTest();
        }

        public static void DoRun()
        {
            //First, map the game dir
            Dictionary<string, string> map = DirectoryIndexer.MapGameDir();

            //Create cache and import
            UAssetCacheBlock cache = new UAssetCacheBlock();
            List<string> readErrors = new List<string>();
            DinoImporter.ImportDinos(cache, map, readErrors);
            //ItemImporter.ImportItems(cache, map, readErrors);

            //Now, save imges
            ImageTool.ProcessImages(readErrors);

            //Finish
            File.WriteAllLines(Program.GetOutputDir() + "errors.log", readErrors.ToArray());
            Console.WriteLine("Done.");
            Console.ReadLine();
        }

        public static string GetOutputDir()
        {
            if(revision == null)
            {
                revision = ImageTool.GenerateID(8);
                Directory.CreateDirectory(OUTPUT_PATH + revision);
                Directory.CreateDirectory(OUTPUT_PATH + revision+"\\assets\\");
            }
            return OUTPUT_PATH + revision + "\\";
        }

        static void MultipleFileTest()
        {
            //SingleFileTestBlueprint(@"Aberration\Dinos\RockDrake\RockDrake_Character_BP.uasset", "RockDrake_Character_BP");
            //SingleFileTestBlueprint(@"PrimalEarth\Dinos\Argentavis\Argent_Character_BP.uasset", "Argent_Character_BP");
            //SingleFileTestImage(@"PrimalEarth/UI/Empty_SpinoHead_Icon.uasset", "Empty_SpinoHead_Icon");
            //SingleFileTestImage(@"ScorchedEarth\Icons\Dinos\DinoEntryIcon_Camelsaurus.uasset", "DinoEntryIcon_Camelsaurus");

            UAssetFileBlueprint primalGameData = UAssetFileBlueprint.OpenFile(Program.GAME_ROOT_PATH + @"PrimalEarth\CoreBlueprints\TestGameMode.uasset", false, "TestGameMode", Program.GAME_ROOT_PATH);
            PropertyReader primalGameDataReader = new PropertyReader(primalGameData.GetFullProperties(new UAssetCacheBlock()));

            List<UProperty> p = primalGameDataReader.props.Where(x => x.name.StartsWith("PerLevelStatsMultiplier_DinoTamed")).ToList();

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
                //UAssetCacheBlock cache = new UAssetCacheBlock();
                //List<UProperty> props = f.GetFullProperties(cache);
                /*foreach (var p in props)
                    Console.WriteLine($"{p.name}, {p.type}, {p.WriteString()}");*/

                //File.WriteAllText("E:\\rockdrake.json", JsonConvert.SerializeObject(props, Formatting.Indented));
                //ArkDinoEntry e = ArkDinoEntry.Convert(f, cache);

                Console.WriteLine(f.payload.Length);
            }
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
