using Newtonsoft.Json;
using System;
using System.IO;
using UassetToolkit;

namespace ArkImportTools
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing");
            using(FileStream fs = new FileStream(@"E:\Programs\ARKEditor\Projects\ShooterGame\Content\PrimalEarth\Dinos\Argentavis\Argent_Character_BP.uasset", FileMode.Open, FileAccess.Read))
            {
                UAssetFile f = UAssetFile.OpenFile(fs, true, "Argent_Character_BP");
                File.WriteAllText("E:\\argentavis.json", JsonConvert.SerializeObject(f.properties, Formatting.Indented));
            }
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
