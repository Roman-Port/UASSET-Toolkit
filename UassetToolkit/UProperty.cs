using System;
using System.Collections.Generic;
using System.Text;
using UassetToolkit.UPropertyTypes;

namespace UassetToolkit
{
    public abstract class UProperty
    {
        public string name;
        public int unknown1;
        public string type;
        public int unknown2;
        public int length;
        public int index;

        public bool isArray;

        public long start;
        public long payloadStart;

        public abstract void Read(IOMemoryStream ms, UAssetFile f);
        public abstract string WriteString();

        public UProperty(IOMemoryStream ms, UAssetFile f)
        {

        }

        public static List<UProperty> ReadProperties(IOMemoryStream ms, UAssetFile f, string arrayType, bool isStruct)
        {
            //Read until none
            List<UProperty> output = new List<UProperty>();
            while(true)
            {
                UProperty p = ReadProp(ms, f, arrayType, isStruct);
                if (p == null)
                    break;
                output.Add(p);
            }
            return output;
        }

        public static UProperty ReadProp(IOMemoryStream ms, UAssetFile f, string arrayType, bool isStruct)
        {
            //Read the name
            long start = ms.position;

            //Read the remainder of the properties
            string name;
            int u1;
            string type;
            int u2;
            int length;
            int index;
            if (arrayType == null)
            {
                //Not an array
                name = ms.ReadNameTableEntry(f);

                //Return null if this is "None". That means we're done reading
                if (name == "None")
                    return null;

                u1 = ms.ReadInt();
                type = ms.ReadNameTableEntry(f);
                u2 = ms.ReadInt();
                length = ms.ReadInt();
                index = ms.ReadInt();

            } else
            {
                name = null;
                u1 = 0;
                type = arrayType;
                u2 = 0;
                length = 0;
                index = 0;
            }
            long payloadStart = ms.position;

            //Create the object
            UProperty u;
            switch (type)
            {
                case "ArrayProperty": u = new ArrayProperty(ms, f); break;
                case "BoolProperty": u = new BoolProperty(ms, f); break;
                case "ByteProperty": u = new ByteProperty(ms, f); break;
                case "DoubleProperty": u = new DoubleProperty(ms, f); break;
                case "FloatProperty": u = new FloatProperty(ms, f); break;
                case "Int16Property": u = new Int16Property(ms, f); break;
                case "Int8Property": u = new Int8Property(ms, f); break;
                case "IntProperty": u = new IntProperty(ms, f); break;
                case "NameProperty": u = new NameProperty(ms, f); break;
                case "ObjectProperty": u = new ObjectProperty(ms, f); break;
                case "StrProperty": u = new StrProperty(ms, f); break;
                case "StructProperty": u = new StructProperty(ms, f); break;
                case "TextProperty": u = new TextProperty(ms, f); break;
                case "UInt16Property": u = new UInt16Property(ms, f); break;
                case "UInt32Property": u = new UInt32Property(ms, f); break;
                case "UInt64Property": u = new UInt64Property(ms, f); break;
                default:
                    throw new Exception($"FAILED TO READ UPROPERTY: Type {type} was not a valid type. Name={name}, u1={u1}, u2={u2}, length={length}, index={index}, position={start}");
            }

            //Set attributes
            u.name = name;
            u.unknown1 = u1;
            u.type = type;
            u.unknown2 = u2;
            u.length = length;
            u.index = index;
            u.start = start;
            u.payloadStart = payloadStart;
            u.isArray = arrayType != null;

            //Dump
            f.DebugDump("UProperty Reading", ConsoleColor.Green, "name", name, "u1", u1.ToString(), "type", type, "u2", u2.ToString(), "length", length.ToString(), "index", index.ToString(), "start", start.ToString(), "payloadStart", payloadStart.ToString());

            //Read
            u.Read(ms, f);

            //Log
            string msg = u.WriteString();
            f.Debug("UProperty Read "+type, msg, ConsoleColor.DarkGreen);

            return u;
        }
    }
}
