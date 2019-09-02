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
        //(...)
        public int headerPayloadBegin; //Value that seems to be important to reading images. At position 147 in header
        
        //Package data
        public string[] name_table; //Maps IDs to classnames
        public GameObjectTableHead[] gameObjectReferences;
        public EmbeddedGameObjectTableHead[] gameObjectEmbeds;
        public string classname;
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
        public virtual void ReadFile(System.IO.Stream s, bool isDebugEnabled, string classname, string rootPath)
        {
            this.isDebugModeEnabled = isDebugEnabled;
            this.classname = classname;
            this.rootPath = rootPath;

            //Create a stream
            stream = new IOMemoryStream(s, true);

            //Read header data
            ReadHeaderData();

            //Read name table
            ReadNameTable();

            //Read GameObject headers
            ReadGameObjectReferences();

            //Now, read embedded GameObject headers
            ReadEmbeddedGameObjectReferences();

            //Read metadata
            ReadPackageMetadata();

            //Get parent classname
            /*hasParentUObject = false;
            if(metadata.ContainsKey("ParentClassPackage"))
            {
                hasParentUObject = TryGetFullPackagePath(metadata["ParentClassPackage"], out parentPath);
                parentClassname = GetPackageClassnameFromPath(metadata["ParentClassPackage"]);
            }*/
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
            string name = splitClassname[splitClassname.Length - 1];
            if (name.EndsWith(".uasset"))
                name = name.Substring(0, name.Length - ".uasset".Length);
            return name;
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

            //There's some data that we don't know how to get to that seems to sit at 16 bytes before the name table. Read it
            stream.position = nameTableOffset - 16;
            headerPayloadBegin = stream.ReadInt();

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

        void ReadPackageMetadata()
        {
            //Some files seem to have no package metadata. They might be corrupted?
            if(packagePropertyDictOffset == 0)
            {
                metadata = new Dictionary<string, string>();
                return;
            }

            stream.position = packagePropertyDictOffset;

            int u1 = stream.ReadInt();
            string u2 = stream.ReadUEString();
            string u3 = stream.ReadUEString();
            int u4 = stream.ReadInt();
            int u5 = stream.ReadInt(); //1
            string u6 = stream.ReadUEString(); //argent_character_bp
            string u7 = stream.ReadUEString(); //Blueprint

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

        public Stream CopyPayload()
        {
            //Check payload position
            if (headerPayloadBegin > stream.ms.Length)
                throw new Exception("Payload beginning is after the end of file!");

            //Open stream and copy
            MemoryStream ms = new MemoryStream();
            byte[] buffer = new byte[2048];
            int bytesRemaining = (int)stream.ms.Length - headerPayloadBegin;
            stream.position = headerPayloadBegin;
            while(bytesRemaining > 0)
            {
                int size = Math.Min(2048, bytesRemaining);
                stream.ms.Read(buffer, 0, size);
                ms.Write(buffer, 0, size);
                bytesRemaining -= size;
            }

            //Rewind and return
            ms.Position = 0;
            return ms;
        }

        //Refs
        public string GetReferencedUAssetPathname(ObjectProperty h)
        {
            //Make sure htis is the correct type
            if (h.objectRefType != ObjectProperty.ObjectPropertyType.TypeID)
                throw new Exception("Only TypeID ObjectProperties are supported for finding a referenced file.");

            //Find using the ID
            return GetReferencedUAssetPathname(h.objectIndex);
        }

        public string GetReferencedUAssetPathname(int index)
        {
            if (index < 0)
            {
                GameObjectTableHead hr = gameObjectReferences[-index - 1];
                return GetReferencedUAssetPathname(hr);
            }
            else
            {
                GameObjectTableHead hr = gameObjectReferences[index];
                return GetReferencedUAssetPathname(hr);
            }
        }

        public string GetReferencedUAssetPathname(GameObjectTableHead h)
        {
            //Check if we're actually looking for something else
            if (h.index < 0)
            {
                GameObjectTableHead hr = gameObjectReferences[-h.index - 1];
                return GetReferencedUAssetPathname(hr);
            }

            //Get the full path
            bool ok = TryGetFullPackagePath(h.name, out string pathname);

            //Stop if not ok
            if (!ok)
                return null;

            return pathname;
        }

        //Refs type
        void BaseGetReferencedUAssetBaseFromPathname(UAssetFile bp, string pathname)
        {
            //Get the pathname
            string classname = GetPackageClassnameFromPath(pathname);

            //Open file
            using (FileStream fs = new FileStream(pathname, FileMode.Open, FileAccess.Read))
                bp.ReadFile(fs, isDebugModeEnabled, classname, rootPath);
        }

        public UAssetFileBlueprint GetReferencedUAssetBlueprintFromPathname(string pathname)
        {
            UAssetFileBlueprint bp = new UAssetFileBlueprint();
            BaseGetReferencedUAssetBaseFromPathname(bp, pathname);
            return bp;
        }

        public UAssetFileMaterial GetReferencedUAssetMaterialFromPathname(string pathname)
        {
            UAssetFileMaterial bp = new UAssetFileMaterial();
            BaseGetReferencedUAssetBaseFromPathname(bp, pathname);
            return bp;
        }

        public UAssetFileTexture2D GetReferencedUAssetTexture2DFromPathname(string pathname)
        {
            UAssetFileTexture2D bp = new UAssetFileTexture2D();
            BaseGetReferencedUAssetBaseFromPathname(bp, pathname);
            return bp;
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
    }
}
