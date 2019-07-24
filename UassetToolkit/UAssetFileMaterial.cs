using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UassetToolkit.UPropertyTypes;
using UassetToolkit.UStructTypes;

namespace UassetToolkit
{
    public class UAssetFileMaterial : UAssetFile
    {
        //Package data
        public List<UProperty> properties;

        //Material data
        public List<TextureParameterValue> textureParameters;

        public static UAssetFileMaterial OpenFile(string pathname, bool isDebugEnabled, string classname, string rootPath)
        {
            using (FileStream fs = new FileStream(pathname, FileMode.Open, FileAccess.Read))
            {
                return OpenFile(fs, isDebugEnabled, classname, rootPath);
            }
        }

        public static UAssetFileMaterial OpenFile(FileStream fs, bool isDebugEnabled, string classname, string rootPath)
        {
            UAssetFileMaterial f = new UAssetFileMaterial();
            f.ReadFile(fs, isDebugEnabled, classname, rootPath);
            return f;
        }

        public UAssetFileMaterial OpenUAssetWithSameSettings(string path, string classname)
        {
            UAssetFileMaterial f = UAssetFileMaterial.OpenFile(path, isDebugModeEnabled, classname, rootPath);
            return f;
        }

        public override void ReadFile(Stream s, bool isDebugEnabled, string classname, string rootPath)
        {
            //Do initial init
            base.ReadFile(s, isDebugEnabled, classname, rootPath);

            //Now, read properties
            ReadProperties();

            //Get the texture parameter values
            PropertyReader reader = new PropertyReader(properties);
            ConvertTextureParameters(reader);
        }

        void ReadProperties()
        {
            //Find the embedded game object with the data
            EmbeddedGameObjectTableHead result = null;
            foreach (var r in gameObjectEmbeds)
            {
                if (r.type == classname)
                    result = r;
            }

            //Crash if not correct
            if (result == null)
                throw new Exception("Could not find property data.");

            //Go to
            stream.position = result.dataLocation;

            //Read
            properties = UProperty.ReadProperties(stream, this, null, false);
        }

        void ConvertTextureParameters(PropertyReader reader)
        {
            ArrayProperty p = reader.GetProperty<ArrayProperty>("TextureParameterValues");
            textureParameters = new List<TextureParameterValue>();
            foreach(var e in p.props)
            {
                StructProperty sp = (StructProperty)e;
                PropListStruct lp = (PropListStruct)sp.data;
                PropertyReader lr = new PropertyReader(lp.propsList);
                textureParameters.Add(new TextureParameterValue
                {
                    name = lr.GetPropertyStringOrName("ParameterName"),
                    prop = lr.GetProperty<ObjectProperty>("ParameterValue")
                });
            }
        }

        public class TextureParameterValue
        {
            public string name;
            public ObjectProperty prop;
        }
    }
}
