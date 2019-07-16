using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ArkImportTools
{
    public static class DirectoryIndexer
    {
        public static Dictionary<string, string> MapGameDir()
        {
            Console.WriteLine("Please wait, indexing and mapping the game directory...");
            Dictionary<string, string> data = new Dictionary<string, string>();
            IndexDir(data, Program.GAME_ROOT_PATH);
            Console.WriteLine("Found " + data.Keys.Count + " game classes.");
            return data;
        }

        private static void IndexDir(Dictionary<string, string> data, string pathname)
        {
            string[] files = Directory.GetFiles(pathname);
            string[] dirs = Directory.GetDirectories(pathname);

            //Add files
            foreach(string s in files)
            {
                if(s.EndsWith(".uasset"))
                {
                    //Get the classname
                    string[] split = s.Split('\\');
                    string name = split[split.Length - 1];
                    name = name.Substring(0, name.Length - ".uasset".Length);

                    //Write
                    if (!data.ContainsKey(name))
                        data.Add(name, s);
                }
            }

            //Add dirs
            foreach (string d in dirs)
                IndexDir(data, d);
        }
    }
}
