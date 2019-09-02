using ArkImportTools.OutputEntities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UassetToolkit;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics;
using SixLabors.ImageSharp.Formats.Png;

namespace ArkImportTools
{
    public static class ImageTool
    {
        public const string ASSETS_URL = "https://icon-assets.deltamap.net/";
        public const string FORMAT_TYPE = ".png";

        private static List<QueuedImage> queue = new List<QueuedImage>();

        public static ArkImage QueueImage(ClassnamePathnamePair name, ImageModifications mods)
        {
            return QueueImage(name.classname, name.pathname, mods);
        }

        public static string GetAssetsUrl()
        {
            Program.GetOutputDir();
            return ASSETS_URL + Program.revision + "/";
        }

        public static ArkImage QueueImage(string classname, string pathname, ImageModifications mods)
        {
            //Generate IDs for the high res image and thumbnail
            string hiId = GenerateUniqueImageID();
            string loId = GenerateUniqueImageID();

            //Now, create an ArkImage
            ArkImage r = new ArkImage
            {
                image_thumb_url = GetAssetsUrl()+ loId+FORMAT_TYPE,
                image_url = GetAssetsUrl()+ hiId+FORMAT_TYPE
            };

            //Now, create an object and add it to a queue
            queue.Add(new QueuedImage
            {
                classname = classname,
                pathname = pathname,
                hiId = hiId,
                loId = loId,
                mods = mods
            });

            return r;
        }

        public static void ProcessImages(List<string> readErrors)
        {
            //Clean up any old and bad paths
            Console.WriteLine("Cleaning up old image conversions...");
            if(Directory.Exists("./Lib/UModel/in_temp/"))
                Directory.Delete("./Lib/UModel/in_temp/", true);
            if (Directory.Exists("./Lib/UModel/out_temp/"))
                Directory.Delete("./Lib/UModel/out_temp/", true);

            //Make structre
            Directory.CreateDirectory("./Lib/UModel/in_temp/");
            Directory.CreateDirectory("./Lib/UModel/out_temp/");

            //First, we copy all packages to a temporary path with their index
            Console.WriteLine($"Now copying {queue.Count} images...");
            for (int i = 0; i<queue.Count; i++)
            {
                string source = queue[i].pathname;
                File.Copy(source, $"./Lib/UModel/in_temp/{i}.uasset");
            }

            //Now, run the conversion
            Console.WriteLine("Now converting images using UModel...");
            Process p = Process.Start(new ProcessStartInfo
            {
                Arguments = "",
                FileName = "go.bat",
                WorkingDirectory = "Lib\\UModel\\",
                UseShellExecute = true
            });
            p.WaitForExit();

            //Now, load and process these images
            int ok = 0;
            Console.WriteLine($"Now processing {queue.Count} images...");
            for(int i = 0; i<queue.Count; i+=1)
            {
                QueuedImage q = queue[i];

                try
                {
                    //Get the directory. It's a little janky, as files are stored in subdirs
                    string[] results = Directory.GetFiles($"./Lib/UModel/out_temp/{i}/");
                    if (results.Length != 1)
                        throw new Exception("None or too many results found for image.");

                    //Open FileStream on this
                    using (FileStream imgStream = new FileStream(results[0], FileMode.Open, FileAccess.Read))
                    {
                        //Now, begin reading the TGA data https://en.wikipedia.org/wiki/Truevision_TGA
                        IOMemoryStream imgReader = new IOMemoryStream(imgStream, true);
                        imgReader.position += 3 + 5; //Skip intro, it will always be known
                        imgReader.ReadShort(); //Will always be 0
                        imgReader.ReadShort(); //Will aways be 0
                        short width = imgReader.ReadShort();
                        short height = imgReader.ReadShort();
                        byte colorDepth = imgReader.ReadByte();
                        imgReader.ReadByte();

                        //Now, we can begin reading image data
                        //This appears to be bugged for non-square images right now.
                        using (Image<Rgba32> img = new Image<Rgba32>(width, height))
                        {
                            //Read file
                            byte[] channels;
                            for (int y = 0; y < height; y++)
                            {
                                for (int x = 0; x < width; x++)
                                {
                                    if (colorDepth == 32)
                                    {
                                        //Read four channels
                                        channels = imgReader.ReadBytes(4);

                                        //Set pixel
                                        img[x, width - y - 1] = new Rgba32(channels[2], channels[1], channels[0], channels[3]);
                                    }
                                    else if (colorDepth == 24)
                                    {
                                        //Read three channels
                                        channels = imgReader.ReadBytes(3);

                                        //Set pixel
                                        img[x, width - y - 1] = new Rgba32(channels[2], channels[1], channels[0]);
                                    }
                                }
                            }

                            //Apply mods
                            if (q.mods == ImageModifications.White)
                                ApplyWhiteMod(img);

                            //Save original image
                            using (FileStream fs = new FileStream(Program.GetOutputDir() + "assets\\" + q.hiId + FORMAT_TYPE, FileMode.Create))
                                img.SaveAsPng(fs);

                            //Now, downscale
                            img.Mutate(x => x.Resize(64, 64));

                            //Save thumbnail
                            using (FileStream fs = new FileStream(Program.GetOutputDir() + "assets\\" + q.loId + FORMAT_TYPE, FileMode.Create))
                                img.SaveAsPng(fs, new PngEncoder
                                {
                                    CompressionLevel = 9
                                });

                            ok++;
                        }
                    }
                } catch (Exception ex)
                {
                    Console.WriteLine($"Failed to process image {q.classname} with error {ex.Message}");
                    readErrors.Add($"Failed to process image {q.classname} with error {ex.Message} {ex.StackTrace}");
                }
            }

            Console.WriteLine($"Processed {ok}/{queue.Count} images.");

            //Clean up any old and bad paths
            Console.WriteLine("Cleaning up...");
            if (Directory.Exists("./Lib/UModel/in_temp/"))
                Directory.Delete("./Lib/UModel/in_temp/", true);
            if (Directory.Exists("./Lib/UModel/out_temp/"))
                Directory.Delete("./Lib/UModel/out_temp/", true);
        }

        private static List<string> usedIds = new List<string>();
        private static Random rand = new Random();

        private static string GenerateUniqueImageID()
        {
            string id = GenerateUnsafeImageID();
            while (usedIds.Contains(id))
                id = GenerateUnsafeImageID();
            usedIds.Add(id);
            return id;
        }

        private static string GenerateUnsafeImageID()
        {
            //Generate an ID, it will not be guarenteed to be unique
            char[] p = "1234567890abcdef".ToCharArray();
            char[] output = new char[24];
            for (int i = 0; i < 24; i++)
                output[i] = p[rand.Next(0, p.Length)];
            return new string(output);
        }

        public static string GenerateID(int length)
        {
            //Generate an ID, it will not be guarenteed to be unique
            char[] p = "1234567890abcdef".ToCharArray();
            char[] output = new char[length];
            for (int i = 0; i < length; i++)
                output[i] = p[rand.Next(0, p.Length)];
            return new string(output);
        }

        static void ApplyWhiteMod(Image<Rgba32> img)
        {
            //Set all pixels to white, but keep the alpha
            for(int x = 0; x<img.Width; x++)
            {
                for(int y = 0; y<img.Height; y++)
                {
                    Rgba32 v = img[x, y];
                    img[x, y] = new Rgba32(255, 255, 255, v.A);
                }
            }
        }

        class QueuedImage
        {
            public string loId;
            public string hiId;
            public string classname;
            public string pathname;
            public ImageModifications mods;
        }

        public enum ImageModifications
        {
            None,
            White
        }
    }
}
