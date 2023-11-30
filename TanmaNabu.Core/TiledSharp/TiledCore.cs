// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace TiledSharp
{
    public class TmxDocument
    {
        public string TmxDirectory { get; private set; }

        public TmxDocument()
        {
            TmxDirectory = string.Empty;
        }

        protected XDocument ReadXml(string filepath)
        {
            XDocument xDoc;

            Assembly asm = Assembly.GetEntryAssembly();
            string[] manifest = new string[0];

            if (asm != null)
            {
                manifest = asm.GetManifestResourceNames();
            }

            string fileResPath = filepath.Replace(Path.DirectorySeparatorChar.ToString(), ".");
            string fileRes = Array.Find(manifest, s => s.EndsWith(fileResPath));

            // If there is a resource in the assembly, load the resource
            // Otherwise, assume filepath is an explicit path
            if (fileRes != null)
            {
                using (Stream xmlStream = asm.GetManifestResourceStream(fileRes))
                {
                    using (XmlReader reader = XmlReader.Create(xmlStream))
                    {
                        xDoc = XDocument.Load(reader);
                    }
                }
                TmxDirectory = string.Empty;
            }
            else
            {
                if (!File.Exists(filepath))
                {
                    throw new FileNotFoundException($"Resource file {Path.GetFileName(filepath)} not found", filepath);
                }

                xDoc = XDocument.Load(filepath);
                TmxDirectory = Path.GetDirectoryName(filepath);
            }

            return xDoc;
        }
    }

    public class TmxList<T> : KeyedCollection<string, T> where T : ITmxElement
    {
        private Dictionary<string, int> nameCount = new Dictionary<string, int>();

        public new void Add(T t)
        {
            string tName = t.Name;

            // Rename duplicate entries by appending a number
            if (Contains(tName))
            {
                nameCount[tName] += 1;
            }
            else
            {
                nameCount.Add(tName, 0);
            }
            base.Add(t);
        }

        protected override string GetKeyForItem(T item)
        {
            string name = item.Name;
            int count = nameCount[name];

            int dupes = 0;

            // For duplicate keys, append a counter
            // For pathological cases, insert underscores to ensure uniqueness
            while (Contains(name))
            {
                name = name + string.Concat(Enumerable.Repeat("_", dupes)) + count.ToString();
                dupes++;
            }

            return name;
        }
    }

    [Serializable]
    public class PropertyDict : Dictionary<string, string>
    {
        public PropertyDict(XContainer xmlProp)
        {
            if (xmlProp == null) return;

            foreach (XElement p in xmlProp.Elements("property"))
            {
                string pval;
                try
                {
                    pval = p.Attribute("value").Value;
                }
                catch (NullReferenceException)
                {
                    // Fallback to element value if no "value"
                    pval = p.Value;
                }

                string pname = p.Attribute("name").Value;
                Add(pname, pval);
            }
        }
    }

    public class TmxImage
    {
        public string Source { get; private set; }
        public string Format { get; private set; }
        public Stream Data { get; private set; }
        public TmxColor Trans { get; private set; }
        public int? Width { get; private set; }
        public int? Height { get; private set; }

        public TmxImage(XElement xImage, string tmxDir = "")
        {
            if (xImage == null) return;

            XAttribute xSource = xImage.Attribute("source");

            if (xSource != null)
            {
                // Append directory if present
                Source = Path.Combine(tmxDir, (string)xSource);
            }                
            else
            {
                Format = (string)xImage.Attribute("format");
                XElement xData = xImage.Element("data");
                TmxBase64Data decodedStream = new TmxBase64Data(xData);
                Data = decodedStream.Data;
            }

            Trans = new TmxColor(xImage.Attribute("trans"));
            Width = (int?)xImage.Attribute("width");
            Height = (int?)xImage.Attribute("height");
        }
    }

    public class TmxColor
    {
        public int R { get; private set; }
        public int G { get; private set; }
        public int B { get; private set; }

        public TmxColor(XAttribute xColor)
        {
            if (xColor == null) return;

            string colorStr = ((string)xColor).TrimStart("#".ToCharArray());

            R = int.Parse(colorStr.Substring(0, 2), NumberStyles.HexNumber);
            G = int.Parse(colorStr.Substring(2, 2), NumberStyles.HexNumber);
            B = int.Parse(colorStr.Substring(4, 2), NumberStyles.HexNumber);
        }
    }

    public class TmxBase64Data
    {
        public Stream Data { get; private set; }

        public TmxBase64Data(XElement xData)
        {
            if ((string)xData.Attribute("encoding") != "base64")
            {
                throw new Exception("TmxBase64Data: Only Base64-encoded data is supported.");
            }

            byte[] rawData = Convert.FromBase64String(xData.Value);
            Data = new MemoryStream(rawData, false);

            string compression = (string)xData.Attribute("compression");
            if (compression == "gzip")
            {
                Data = new GZipStream(Data, CompressionMode.Decompress);
            }
            else if (compression == "zlib")
            {
                // Strip 2-byte header and 4-byte checksum
                // TODO: Validate header here
                int bodyLength = rawData.Length - 6;
                byte[] bodyData = new byte[bodyLength];
                Array.Copy(rawData, 2, bodyData, 0, bodyLength);

                MemoryStream bodyStream = new MemoryStream(bodyData, false);
                Data = new DeflateStream(bodyStream, CompressionMode.Decompress);
            }
            else if (compression != null)
            {
                throw new Exception("TmxBase64Data: Unknown compression.");
            }
        }
    }
}
