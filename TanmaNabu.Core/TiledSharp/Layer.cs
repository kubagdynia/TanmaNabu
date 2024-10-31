// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;

namespace TiledSharp;

public class TmxLayer : ITmxLayer
{
    public string Name { get; private set; }
    public double Opacity { get; private set; }
    public bool Visible { get; private set; }
    public double? OffsetX { get; private set; }
    public double? OffsetY { get; private set; }

    public Collection<TmxLayerTile> Tiles { get; private set; }
    public PropertyDict Properties { get; private set; }

    public TmxLayer(XElement xLayer, int width, int height)
    {
        Name = (string)xLayer.Attribute("name");
        Opacity = (double?)xLayer.Attribute("opacity") ?? 1.0;
        Visible = (bool?)xLayer.Attribute("visible") ?? true;
        OffsetX = (double?)xLayer.Attribute("offsetx") ?? 0.0;
        OffsetY = (double?)xLayer.Attribute("offsety") ?? 0.0;

        var xData = xLayer.Element("data");
        var encoding = xData is null ? null : (string)xData.Attribute("encoding");

        Tiles = [];

        switch (encoding)
        {
            case "base64":
            {
                var decodedStream = new TmxBase64Data(xData);
                var stream = decodedStream.Data;

                using var br = new BinaryReader(stream);
                for (var j = 0; j < height; j++)
                for (var i = 0; i < width; i++)
                    Tiles.Add(new TmxLayerTile(br.ReadUInt32(), i, j));
                break;
            }
            case "csv":
            {
                var csvData = xData.Value;
                var k = 0;
                foreach (var s in csvData.Split(','))
                {
                    var gid = uint.Parse(s.Trim());
                    var x = k % width;
                    var y = k / width;
                    Tiles.Add(new TmxLayerTile(gid, x, y));
                    k++;
                }

                break;
            }
            case null:
            {
                var titleData = xData?.Elements("tile");
                if (titleData is null) throw new Exception("TmxLayer: Missing data element.");
                
                var k = 0;
                foreach (var e in xData.Elements("tile"))
                {
                    var gid = (uint?)e.Attribute("gid") ?? 0;

                    var x = k % width;
                    var y = k / width;

                    Tiles.Add(new TmxLayerTile(gid, x, y));
                    k++;
                }

                break;
            }
            default:
                throw new Exception("TmxLayer: Unknown encoding.");
        }

        Properties = new PropertyDict(xLayer.Element("properties"));
    }
}

public class TmxLayerTile
{
    // Tile flip bit flags
    const uint FlippedHorizontallyFlag = 0x80000000;
    const uint FlippedVerticallyFlag = 0x40000000;
    const uint FlippedDiagonallyFlag = 0x20000000;

    public int Gid { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public bool HorizontalFlip { get; private set; }
    public bool VerticalFlip { get; private set; }
    public bool DiagonalFlip { get; private set; }
    public TmxLayerTile(uint id, int x, int y)
    {
        uint rawGid = id;
        X = x;
        Y = y;

        // Scan for tile flip bit flags

        var flip = (rawGid & FlippedHorizontallyFlag) != 0;
        HorizontalFlip = flip;

        flip = (rawGid & FlippedVerticallyFlag) != 0;
        VerticalFlip = flip;

        flip = (rawGid & FlippedDiagonallyFlag) != 0;
        DiagonalFlip = flip;

        // Zero the bit flags
        rawGid &= ~(FlippedHorizontallyFlag |
                    FlippedVerticallyFlag |
                    FlippedDiagonallyFlag);

        // Save GID remainder to int
        Gid = (int)rawGid;
    }
}