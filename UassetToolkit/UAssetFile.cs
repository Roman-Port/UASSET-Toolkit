using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UassetToolkit.UPropertyTypes;

namespace UassetToolkit
{
    public class UAssetFile
    {
        //Package headers
        public int headerUnknown1;
        public int headerUnknown2;
        public int headerUnknown3;
        public int headerUnknown4;
        public int headerUnknown5;
        public int headerUnknown6;
        public int headerUnknown7;
        public string headerUnknown9;
        public int headerUnknown10;
        public int nameTableLength; //How many elements are in the name table
        public int nameTableOffset; //Where the name table is, starting at the beginning of the file
        public int embeddedGameObjectCount; //Number of objects inside of this
        public int headerUnknown11;
        public int refGameObjectCount; //Number of refrenced game objects
        public int headerUnknown12;
        public int binaryIdTableOffset; //Offset to the "binary id" table, st arting from the beginning of the file
        public int headerUnknown13;
        public int thumbnailOffset; //Offset to the thumbnail, starting at the beginning of the file
        public int packagePropertyDictOffset; //Offset to the package property dict
        
        //Package data
        public string[] name_table; //Maps IDs to classnames
        public GameObjectTableHead[] gameObjectReferences;
        public EmbeddedGameObjectTableHead[] gameObjectEmbeds;
        public string classname;
        public List<UProperty> properties;
        public Dictionary<string, string> metadata;
        public string parentPath;
        public string parentClassname;
        public bool hasParentUObject;

        //Others
        public IOMemoryStream stream;
        public bool isDebugModeEnabled;
        public string rootPath;


        /// <summary>
        /// Opens a UAssetFile
        /// </summary>
        /// <returns></returns>
        public static UAssetFile OpenFile(string pathname, bool isDebugEnabled, string classname, string rootPath)
        {
            using (FileStream fs = new FileStream(pathname, FileMode.Open, FileAccess.Read))
            {
                return OpenFile(fs, isDebugEnabled, classname, rootPath);
            }
        }

        /// <summary>
        /// Opens a UAssetFile
        /// </summary>
        /// <returns></returns>
        public static UAssetFile OpenFile(System.IO.Stream s, bool isDebugEnabled, string classname, string rootPath)
        {
            //Create the object
            UAssetFile f = new UAssetFile
            {
                isDebugModeEnabled = isDebugEnabled,
                classname = classname,
                rootPath = rootPath
            };

            //Create a stream
            f.stream = new IOMemoryStream(s, true);

            //Read header data
            f.ReadHeaderData();

            //Read name table
            f.ReadNameTable();

            //Read GameObject headers
            f.ReadGameObjectReferences();

            //Now, read embedded GameObject headers
            f.ReadEmbeddedGameObjectReferences();

            //Read metadata
            f.ReadPackageMetadata();

            //Get parent classname
            f.hasParentUObject = f.TryGetFullPackagePath(f.metadata["ParentClassPackage"], out f.parentPath);
            f.parentClassname = GetPackageClassnameFromPath(f.metadata["ParentClassPackage"]);

            //Now, read properties
            f.ReadDefaultProperties();

            return f;
        }

        public void Debug(string topic, string msg, ConsoleColor color)
        {
            if (!isDebugModeEnabled)
                return;
            Console.ForegroundColor = color;
            Console.WriteLine($"[UAssetFile -> {topic}] " + msg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Warn(string topic, string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[UAssetFile WARNING -> {topic}] " + msg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void DebugDump(string name, ConsoleColor color, params string[] data)
        {
            if (!isDebugModeEnabled)
                return;
            //Build
            string msg = $"";
            bool wasLastLabel = false;
            foreach (string s in data)
            {
                if (!wasLastLabel)
                    msg += s + "=";
                else
                    msg += s + ", ";
                wasLastLabel = !wasLastLabel;
            }
                
            Debug(name, msg, color);
        }

        public bool TryGetFullPackagePath(string path, out string output)
        {
            bool ok = path.StartsWith("/Game/");
            if (ok)
                output = path.Replace("/Game/", rootPath) + ".uasset";
            else
                output = path;
            return ok;
        }

        public static string GetPackageClassnameFromPath(string pathname)
        {
            string[] splitClassname = pathname.Split('/');
            return splitClassname[splitClassname.Length - 1];
        }

        void ReadHeaderData()
        {
            stream.position = 0;

            //Start reading header data
            headerUnknown1 = stream.ReadInt();
            headerUnknown2 = stream.ReadInt();
            headerUnknown3 = stream.ReadInt();
            headerUnknown4 = stream.ReadInt();
            headerUnknown5 = stream.ReadInt();
            headerUnknown6 = stream.ReadInt();
            headerUnknown7 = stream.ReadInt();
            headerUnknown9 = stream.ReadUEString();
            headerUnknown10 = stream.ReadInt();
            nameTableLength = stream.ReadInt();
            nameTableOffset = stream.ReadInt();
            embeddedGameObjectCount = stream.ReadInt();
            headerUnknown11 = stream.ReadInt();
            refGameObjectCount = stream.ReadInt();
            headerUnknown12 = stream.ReadInt();
            binaryIdTableOffset = stream.ReadInt();
            headerUnknown13 = stream.ReadInt();
            thumbnailOffset = stream.ReadInt();
            packagePropertyDictOffset = stream.ReadInt();

            //Dump
            DebugDump("Header Data", ConsoleColor.Cyan, "headerUnknown1", headerUnknown1.ToString(), "headerUnknown2", headerUnknown2.ToString(), "headerUnknown3", headerUnknown3.ToString(),
                "headerUnknown4", headerUnknown4.ToString(), "headerUnknown5", headerUnknown5.ToString(), "headerUnknown6", headerUnknown6.ToString(), "headerUnknown7", headerUnknown7.ToString(),
                "headerUnknown9", headerUnknown9.ToString(), "headerUnknown10", headerUnknown10.ToString() );
            DebugDump("Header Data", ConsoleColor.Cyan, "nameTableLength", nameTableLength.ToString(), "nameTableOffset", nameTableOffset.ToString(), "embeddedGameObjectCount", embeddedGameObjectCount.ToString(), "headerUnknown11", headerUnknown11.ToString(),
                "refGameObjectCount", refGameObjectCount.ToString(), "headerUnknown12", headerUnknown12.ToString(), "binaryIdTableOffset", binaryIdTableOffset.ToString(), "headerUnknown13", headerUnknown13.ToString(),
                "binaryIdTableOffset", binaryIdTableOffset.ToString(), "headerUnknown13", headerUnknown13.ToString(), "thumbnailOffset", thumbnailOffset.ToString(), "packagePropertyDictOffset", packagePropertyDictOffset.ToString());
        }

        void ReadNameTable()
        {
            //Jump to the beginning of the name table
            stream.position = nameTableOffset;

            //Read array
            name_table = stream.ReadStringArray(nameTableLength);

            //Dump
            //Debug("Name Table Length", name_table.Length.ToString(), ConsoleColor.Yellow);
            for (int i = 0; i < nameTableLength; i++)
                Debug($"Name Table Entry {i}", name_table[i], ConsoleColor.Yellow);
        }

        void ReadGameObjectReferences()
        {
            //Starts directly after the name table. Assume we're already there
            gameObjectReferences = new GameObjectTableHead[refGameObjectCount];
            for(int i = 0; i<refGameObjectCount; i++)
            {
                GameObjectTableHead h = GameObjectTableHead.ReadEntry(stream, this);
                gameObjectReferences[i] = h;
                DebugDump($"Game Object Reference {i} @ {h.startPos}", ConsoleColor.Blue, "cType", h.coreType, "u1", h.unknown1.ToString(), "oType", h.objectType, "u2", h.unknown2.ToString(), "i", h.index.ToString(), "name", h.name, "u4", h.unknown4.ToString());
            }
        }

        void ReadEmbeddedGameObjectReferences()
        {
            //Starts directly after the referenced GameObject table. Assume we're already there
            gameObjectEmbeds = new EmbeddedGameObjectTableHead[embeddedGameObjectCount];
            for (int i = 0; i < embeddedGameObjectCount; i++)
            {
                EmbeddedGameObjectTableHead h = EmbeddedGameObjectTableHead.ReadEntry(stream, this);
                gameObjectEmbeds[i] = h;
                DebugDump($"Game Object Embed {i} @ {h.entryLocation}", ConsoleColor.Magenta, "id", h.id.ToString(), "u2", h.unknown2.ToString(), "u3", h.unknown3.ToString(), "type", h.type, "u4", h.unknown4.ToString(), "u5", h.unknown5.ToString(),
                    "length", h.dataLength.ToString(), "location", h.dataLocation.ToString(), "u6", h.unknown6.ToString(), "u7", h.unknown7.ToString(), "u8", h.unknown8.ToString(), "u9", h.unknown9.ToString(), "u10", h.unknown10.ToString(), "u11", h.unknown11.ToString(),
                    "u12", h.unknown12.ToString(), "u13", h.unknown13.ToString(), "u14", h.unknown14.ToString());
            }
        }

        void ReadDefaultProperties()
        {
            //Find the embedded game object with the data
            EmbeddedGameObjectTableHead result = FindEmbeddedObjectByType($"Default__{classname}_C");

            //Go to
            stream.position = result.dataLocation;

            //Read
            properties = UProperty.ReadProperties(stream, this, null, false);
        }

        void ReadPackageMetadata()
        {
            stream.position = packagePropertyDictOffset;

            stream.ReadInt();
            stream.ReadUEString();
            stream.ReadUEString();
            stream.ReadInt();
            stream.ReadInt(); //1
            stream.ReadUEString(); //argent_character_bp
            stream.ReadUEString(); //Blueprint

            int len = stream.ReadInt();
            string lastKey = null;
            metadata = new Dictionary<string, string>();
            for(int i = 0; i<len*2; i++)
            {
                if ((i % 2) == 0)
                    lastKey = stream.ReadUEString();
                else
                    metadata.Add(lastKey, stream.ReadUEString());
            }
        }

        //Tools
        public List<EmbeddedGameObjectTableHead> FindEmbeddedObjectsByType(string type)
        {
            List<EmbeddedGameObjectTableHead> output = new List<EmbeddedGameObjectTableHead>();
            foreach(var e in gameObjectEmbeds)
            {
                if (e.type == type)
                    output.Add(e);
            }
            return output;
        }

        public EmbeddedGameObjectTableHead FindEmbeddedObjectByType(string type)
        {
            List<EmbeddedGameObjectTableHead> results = FindEmbeddedObjectsByType(type);
            if (results.Count == 0)
                throw new Exception($"Failed to find embedded game object with type {type}: No results were found.");
            if (results.Count > 1)
                throw new Exception($"Failed to find embedded game object with type {type}: More than one result was found.");
            return results[0];
        }

        public UAssetFile OpenUAssetWithSameSettings(string path, string classname)
        {
            UAssetFile f;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                f = UAssetFile.OpenFile(fs, isDebugModeEnabled, classname, rootPath);
            return f;
        }

        public UAssetFile GetReferencedUAsset(ObjectProperty h)
        {
            //Make sure htis is the correct type
            if (h.objectRefType != ObjectProperty.ObjectPropertyType.TypeID)
                throw new Exception("Only TypeID ObjectProperties are supported for finding a referenced file.");

            //Find using the ID
            return GetReferencedUAsset(h.objectIndex);
        }

        public UAssetFile GetReferencedUAsset(int index)
        {
            if(index < 0)
            {
                GameObjectTableHead hr = gameObjectReferences[-index - 1];
                return GetReferencedUAsset(hr);
            } else
            {
                GameObjectTableHead hr = gameObjectReferences[index];
                return GetReferencedUAsset(hr);
            }
        }

        public UAssetFile GetReferencedUAsset(GameObjectTableHead h)
        {
            //Check if we're actually looking for something else
            if(h.index < 0)
            {
                GameObjectTableHead hr = gameObjectReferences[-h.index - 1];
                return GetReferencedUAsset(hr);
            }
            
            //Get the full path
            bool ok = TryGetFullPackagePath(h.name, out string pathname);

            //Stop if not ok
            if (!ok)
                return null;

            //Get the pathname
            string classname = GetPackageClassnameFromPath(h.name);

            //Open file
            return OpenUAssetWithSameSettings(pathname, classname);
        }

        /// <summary>
        /// Gets properties from parents
        /// </summary>
        /// <returns></returns>
        public List<UProperty> GetFullProperties(UAssetCacheBlock cache)
        {
            //First, create a list of properties. This list is in the opposite order it should be
            List<List<UProperty>> props = new List<List<UProperty>>();
            props.Add(properties);
            
            //Loop through
            if(hasParentUObject)
            {
                string next = parentPath;
                string nextClassname = parentClassname;
                while (true)
                {
                    UAssetFile f;

                    //Get
                    if (cache.files.ContainsKey(nextClassname))
                        f = cache.files[nextClassname];
                    else
                        using (FileStream fs = new FileStream(next, FileMode.Open, FileAccess.Read))
                            f = UAssetFile.OpenFile(fs, isDebugModeEnabled, nextClassname, rootPath);

                    //Add file to cache
                    if (cache.files.ContainsKey(nextClassname))
                        cache.files.Add(nextClassname, f);

                    //Use
                    props.Add(f.properties);
                    next = f.parentPath;
                    nextClassname = f.parentClassname;
                    if (!f.hasParentUObject)
                        break;
                }
            }

            //Now, work up the list and add properties
            List<UProperty> output = new List<UProperty>();
            for(int i = 0; i<props.Count; i+=1)
            {
                //Loop through properties. If it doesn't exist in the output, add it
                foreach(UProperty prop in props[i])
                {
                    if (output.Where(x => x.name == prop.name && x.index == prop.index).Count() == 0)
                        output.Add(prop);
                }
            }

            return output;
        }
    }
}
