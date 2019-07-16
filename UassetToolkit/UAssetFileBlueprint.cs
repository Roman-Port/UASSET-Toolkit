using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UassetToolkit.UPropertyTypes;

namespace UassetToolkit
{
    public class UAssetFileBlueprint : UAssetFile
    {
        //Package data
        public List<UProperty> properties;

        public static UAssetFileBlueprint OpenFile(string pathname, bool isDebugEnabled, string classname, string rootPath)
        {
            using (FileStream fs = new FileStream(pathname, FileMode.Open, FileAccess.Read))
            {
                return OpenFile(fs, isDebugEnabled, classname, rootPath);
            }
        }

        public static UAssetFileBlueprint OpenFile(FileStream fs, bool isDebugEnabled, string classname, string rootPath)
        {
            UAssetFileBlueprint f = new UAssetFileBlueprint();
            f.ReadFile(fs, isDebugEnabled, classname, rootPath);
            return f;
        }

        public UAssetFileBlueprint OpenUAssetWithSameSettings(string path, string classname)
        {
            UAssetFileBlueprint f = UAssetFileBlueprint.OpenFile(path, isDebugModeEnabled, classname, rootPath);
            return f;
        }

        public override void ReadFile(Stream s, bool isDebugEnabled, string classname, string rootPath)
        {
            //Do initial init
            base.ReadFile(s, isDebugEnabled, classname, rootPath);

            //Now, read properties
            ReadDefaultProperties();
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

        public UAssetFileBlueprint GetReferencedUAsset(ObjectProperty h)
        {
            //Make sure htis is the correct type
            if (h.objectRefType != ObjectProperty.ObjectPropertyType.TypeID)
                throw new Exception("Only TypeID ObjectProperties are supported for finding a referenced file.");

            //Find using the ID
            return GetReferencedUAsset(h.objectIndex);
        }

        public UAssetFileBlueprint GetReferencedUAsset(int index)
        {
            if (index < 0)
            {
                GameObjectTableHead hr = gameObjectReferences[-index - 1];
                return GetReferencedUAsset(hr);
            }
            else
            {
                GameObjectTableHead hr = gameObjectReferences[index];
                return GetReferencedUAsset(hr);
            }
        }

        public UAssetFileBlueprint GetReferencedUAsset(GameObjectTableHead h)
        {
            //Check if we're actually looking for something else
            if (h.index < 0)
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
            if (hasParentUObject)
            {
                string next = parentPath;
                string nextClassname = parentClassname;
                while (true)
                {
                    UAssetFileBlueprint f;

                    //Get
                    if (cache.files.ContainsKey(nextClassname))
                        f = cache.files[nextClassname];
                    else
                        f = OpenUAssetWithSameSettings(next, nextClassname);

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
            for (int i = 0; i < props.Count; i += 1)
            {
                //Loop through properties. If it doesn't exist in the output, add it
                foreach (UProperty prop in props[i])
                {
                    if (output.Where(x => x.name == prop.name && x.index == prop.index).Count() == 0)
                        output.Add(prop);
                }
            }

            return output;
        }
    }
}
