using ArkImportTools.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArkImportTools
{
    public class ClassListBinder
    {
        public ClassListEntry ClassData;

        public static ClassListBinder OpenBinder(string pathname)
        {
            //Log
            Console.WriteLine("Opening class list binder...");
            
            //Load
            string data = System.IO.File.ReadAllText(pathname);

            //Deserialize
            ClassListBinder b = JsonConvert.DeserializeObject<ClassListBinder>(data);

            //Log
            Console.WriteLine($"Opened class list binder, got {b.ClassData.GetAllChildrenCount()} classes.");
            return b;
        }
    }
}
