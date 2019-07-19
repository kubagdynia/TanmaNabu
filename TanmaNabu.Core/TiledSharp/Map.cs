// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TiledSharp
{
    public class TmxMap : TmxDocument
    {
        public string Version {get; private set;}
        public string TiledVersion { get; private set; }
        public int Width {get; private set;}
        public int Height {get; private set;}
        public int TileWidth {get; private set;}
        public int TileHeight {get; private set;}
        public int? HexSideLength {get; private set;}
        public OrientationType Orientation {get; private set;}
        public StaggerAxisType StaggerAxis {get; private set;}
        public StaggerIndexType StaggerIndex {get; private set;}
        public RenderOrderType RenderOrder {get; private set;}
        public TmxColor BackgroundColor {get; private set;}
        public int? NextObjectID {get; private set;}

        public TmxList<TmxTileset> Tilesets {get; private set;}
        public TmxList<TmxLayer> TileLayers {get; private set;}
        public TmxList<TmxObjectGroup> ObjectGroups {get; private set;}
        public TmxList<TmxImageLayer> ImageLayers {get; private set;}
        public TmxList<TmxGroup> Groups { get; private set; }
        public PropertyDict Properties {get; private set;}

        public TmxList<ITmxLayer> Layers { get; private set; }

        public TmxMap(string filename) => Load(ReadXml(filename));

        public TmxMap(Stream inputStream) => Load(XDocument.Load(inputStream));

        public TmxMap(XDocument xDoc) => Load(xDoc);

        private void Load(XDocument xDoc)
        {
            XElement xMap = xDoc.Element("map");
            Version = (string) xMap.Attribute("version");
            TiledVersion = (string)xMap.Attribute("tiledversion");

            Width = (int) xMap.Attribute("width");
            Height = (int) xMap.Attribute("height");
            TileWidth = (int) xMap.Attribute("tilewidth");
            TileHeight = (int) xMap.Attribute("tileheight");
            HexSideLength = (int?) xMap.Attribute("hexsidelength");

            // Map orientation type
            Dictionary<string, OrientationType> orientDict = new Dictionary<string, OrientationType>
            {
                {"unknown", OrientationType.Unknown},
                {"orthogonal", OrientationType.Orthogonal},
                {"isometric", OrientationType.Isometric},
                {"staggered", OrientationType.Staggered},
                {"hexagonal", OrientationType.Hexagonal},
            };

            string orientValue = (string) xMap.Attribute("orientation");
            if (orientValue != null)
            {
                Orientation = orientDict[orientValue];
            }

            // Hexagonal stagger axis
            Dictionary<string, StaggerAxisType> staggerAxisDict = new Dictionary<string, StaggerAxisType>
            {
                {"x", StaggerAxisType.X},
                {"y", StaggerAxisType.Y},
            };

            string staggerAxisValue = (string) xMap.Attribute("staggeraxis");
            if (staggerAxisValue != null)
            {
                StaggerAxis = staggerAxisDict[staggerAxisValue];
            }

            // Hexagonal stagger index
            Dictionary<string, StaggerIndexType> staggerIndexDict = new Dictionary<string, StaggerIndexType>
            {
                {"odd", StaggerIndexType.Odd},
                {"even", StaggerIndexType.Even},
            };

            string staggerIndexValue = (string) xMap.Attribute("staggerindex");
            if (staggerIndexValue != null)
            {
                StaggerIndex = staggerIndexDict[staggerIndexValue];
            }

            // Tile render order
            Dictionary<string, RenderOrderType> renderDict = new Dictionary<string, RenderOrderType>
            {
                {"right-down", RenderOrderType.RightDown},
                {"right-up", RenderOrderType.RightUp},
                {"left-down", RenderOrderType.LeftDown},
                {"left-up", RenderOrderType.LeftUp}
            };

            string renderValue = (string) xMap.Attribute("renderorder");
            if (renderValue != null)
            {
                RenderOrder = renderDict[renderValue];
            }

            NextObjectID = (int?)xMap.Attribute("nextobjectid");
            BackgroundColor = new TmxColor(xMap.Attribute("backgroundcolor"));

            Properties = new PropertyDict(xMap.Element("properties"));

            Tilesets = new TmxList<TmxTileset>();
            foreach (XElement element in xMap.Elements("tileset"))
            {
                Tilesets.Add(new TmxTileset(element, TmxDirectory));
            }

            Layers = new TmxList<ITmxLayer>();
            TileLayers = new TmxList<TmxLayer>();
            ObjectGroups = new TmxList<TmxObjectGroup>();
            ImageLayers = new TmxList<TmxImageLayer>();
            Groups = new TmxList<TmxGroup>();

            foreach (XElement element in xMap.Elements().Where(x => x.Name == "layer" || x.Name == "objectgroup" || x.Name == "imagelayer" || x.Name == "group"))
            {
                switch (element.Name.LocalName)
                {
                    case "layer":
                        TmxLayer tileLayer = new TmxLayer(element, Width, Height);
                        Layers.Add(tileLayer);
                        TileLayers.Add(tileLayer);
                        break;

                    case "objectgroup":
                        TmxObjectGroup objectgroup = new TmxObjectGroup(element);
                        Layers.Add(objectgroup);
                        ObjectGroups.Add(objectgroup);
                        break;

                    case "imagelayer":
                        TmxImageLayer imagelayer = new TmxImageLayer(element, TmxDirectory);
                        Layers.Add(imagelayer);
                        ImageLayers.Add(imagelayer);
                        break;

                    case "group":
                        TmxGroup group = new TmxGroup(element, Width, Height, TmxDirectory);
                        Layers.Add(group);
                        Groups.Add(group);
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }
    }

    public enum OrientationType
    {
        Unknown,
        Orthogonal,
        Isometric,
        Staggered,
        Hexagonal
    }

    public enum StaggerAxisType
    {
        X,
        Y
    }

    public enum StaggerIndexType
    {
        Odd,
        Even
    }

    public enum RenderOrderType
    {
        RightDown,
        RightUp,
        LeftDown,
        LeftUp
    }
}
