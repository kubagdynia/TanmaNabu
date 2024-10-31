// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0
using System;
using System.Linq;
using System.Xml.Linq;

namespace TiledSharp;

public class TmxGroup : ITmxLayer
{
    public string Name { get; private set; }

    public double Opacity { get; private set; }
    public bool Visible { get; private set; }
    public double? OffsetX { get; private set; }
    public double? OffsetY { get; private set; }

    public TmxList<ITmxLayer> Layers { get; private set; }

    public TmxList<TmxLayer> TileLayers { get; private set; }
    public TmxList<TmxObjectGroup> ObjectGroups { get; private set; }
    public TmxList<TmxImageLayer> ImageLayers { get; private set; }
    public TmxList<TmxGroup> Groups { get; private set; }
    public PropertyDict Properties { get; private set; }

    public TmxGroup(XElement xGroup, int width, int height, string tmxDirectory)
    {
        Name = (string)xGroup.Attribute("name") ?? String.Empty;
        Opacity = (double?)xGroup.Attribute("opacity") ?? 1.0;
        Visible = (bool?)xGroup.Attribute("visible") ?? true;
        OffsetX = (double?)xGroup.Attribute("offsetx") ?? 0.0;
        OffsetY = (double?)xGroup.Attribute("offsety") ?? 0.0;

        Properties = new PropertyDict(xGroup.Element("properties"));

        Layers = [];
        TileLayers = [];
        ObjectGroups = [];
        ImageLayers = [];
        Groups = [];

        foreach (XElement element in xGroup.Elements().Where(x => x.Name == "layer" || x.Name == "objectgroup" || x.Name == "imagelayer" || x.Name == "group"))
        {
            switch (element.Name.LocalName)
            {
                case "layer":
                    var tileLayer = new TmxLayer(element, width, height);
                    Layers.Add(tileLayer);
                    TileLayers.Add(tileLayer);
                    break;

                case "objectgroup":
                    var objectgroup = new TmxObjectGroup(element);
                    Layers.Add(objectgroup);
                    ObjectGroups.Add(objectgroup);
                    break;

                case "imagelayer":
                    var imagelayer = new TmxImageLayer(element, tmxDirectory);
                    Layers.Add(imagelayer);
                    ImageLayers.Add(imagelayer);
                    break;

                case "group":
                    var group = new TmxGroup(element, width, height, tmxDirectory);
                    Layers.Add(group);
                    Groups.Add(group);
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}