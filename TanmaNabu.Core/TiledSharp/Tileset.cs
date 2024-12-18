﻿// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;

namespace TiledSharp;

public class TmxTileset : TmxDocument, ITmxElement
{
    /// <summary>
    /// The first global tile ID of this tileset (this global ID maps to the first tile in this tileset).
    /// </summary>
    public int FirstGid { get; private set; }

    /// <summary>
    /// If this tileset is stored in an external TSX (Tile Set XML) file, this attribute refers to that file.
    /// That TSX file has the same structure as the <tileset> element described here.
    /// (There is the firstgid attribute missing and this source attribute is also not there.
    /// These two attributes are kept in the TMX map, since they are map specific.)
    /// </summary>
    public string Source { get; private set; }

    /// <summary>
    /// The name of this tileset.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// The (maximum) width of the tiles in this tileset.
    /// </summary>
    public int TileWidth { get; private set; }

    /// <summary>
    /// The (maximum) height of the tiles in this tileset.
    /// </summary>
    public int TileHeight { get; private set; }

    /// <summary>
    /// The spacing in pixels between the tiles in this tileset (applies to the tileset image).
    /// </summary>
    public int Spacing { get; private set; }

    /// <summary>
    /// The margin around the tiles in this tileset (applies to the tileset image).
    /// </summary>
    public int Margin { get; private set; }

    /// <summary>
    /// The number of tile columns in the tileset. For image collection tilesets it is editable
    /// and is used when displaying the tileset. 
    /// </summary>
    public int? Columns { get; private set; }

    /// <summary>
    /// The number of tiles in this tileset
    /// </summary>
    public int? TileCount { get; private set; }

    public Collection<TmxTilesetTile> Tiles { get; private set; }
    public TmxTileOffset TileOffset { get; private set; }
    public PropertyDict Properties { get; private set; }
    public TmxImage Image { get; private set; }
    public TmxList<TmxTerrain> Terrains { get; private set; }

    // TSX file constructor
    public TmxTileset(string filename)
    {
        var xDoc = ReadXml(filename);
        Load(xDoc.Element("tileset"));
    }

    // TSX file constructor
    public TmxTileset(XContainer xDoc, string tmxDir) => Load(xDoc.Element("tileset"), tmxDir);

    // TMX tileset element constructor
    public TmxTileset(XElement xTileset, string tmxDir = "") => Load(xTileset, tmxDir);

    public void Load(XElement xTileset, string tmxDir = "")
    {
        var xFirstGid = xTileset.Attribute("firstgid");
        var source = (string)xTileset.Attribute("source");

        if (source != null)
        {
            // Prepend the parent TMX directory if necessary
            source = Path.Combine(tmxDir, source);

            // source is always preceded by firstgid
            FirstGid = (int)xFirstGid;

            // Everything else is in the TSX file
            var xDocTileset = ReadXml(source);
            var ts = new TmxTileset(xDocTileset, TmxDirectory);
            Source = ts.Source;
            Name = ts.Name;
            TileWidth = ts.TileWidth;
            TileHeight = ts.TileHeight;
            Spacing = ts.Spacing;
            Margin = ts.Margin;
            Columns = ts.Columns;
            TileCount = ts.TileCount;
            TileOffset = ts.TileOffset;
            Image = ts.Image;
            Terrains = ts.Terrains;
            Tiles = ts.Tiles;
            Properties = ts.Properties;
        }
        else
        {
            // firstgid is always in TMX, but not TSX
            if (xFirstGid != null)
            {
                FirstGid = (int)xFirstGid;
            }

            Source = (string)xTileset.Attribute("source");
            Name = (string)xTileset.Attribute("name");
            TileWidth = (int)xTileset.Attribute("tilewidth");
            TileHeight = (int)xTileset.Attribute("tileheight");
            Spacing = (int?)xTileset.Attribute("spacing") ?? 0;
            Margin = (int?)xTileset.Attribute("margin") ?? 0;
            Columns = (int?)xTileset.Attribute("columns");
            TileCount = (int?)xTileset.Attribute("tilecount");
            TileOffset = new TmxTileOffset(xTileset.Element("tileoffset"));
            Image = new TmxImage(xTileset.Element("image"), tmxDir);

            Terrains = [];
            var xTerrainType = xTileset.Element("terraintypes");
            if (xTerrainType != null)
            {
                foreach (var element in xTerrainType.Elements("terrain"))
                {
                    Terrains.Add(new TmxTerrain(element));
                }
            }

            Tiles = [];
            foreach (var xTile in xTileset.Elements("tile"))
            {
                var tile = new TmxTilesetTile(xTile, Terrains, tmxDir);
                Tiles.Add(tile);
            }

            Properties = new PropertyDict(xTileset.Element("properties"));
        }
    }
}

public class TmxTileOffset
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public TmxTileOffset(XElement xTileOffset)
    {
        if (xTileOffset == null)
        {
            X = 0;
            Y = 0;
        }
        else
        {
            X = (int)xTileOffset.Attribute("x");
            Y = (int)xTileOffset.Attribute("y");
        }
    }
}

public class TmxTerrain : ITmxElement
{
    public string Name { get; private set; }
    public int Tile { get; private set; }

    public PropertyDict Properties { get; private set; }

    public TmxTerrain(XElement xTerrain)
    {
        Name = (string)xTerrain.Attribute("name");
        Tile = (int)xTerrain.Attribute("tile");
        Properties = new PropertyDict(xTerrain.Element("properties"));
    }
}

public class TmxTilesetTile
{
    public int Id { get; private set; }
    public Collection<TmxTerrain> TerrainEdges { get; private set; } = [];
    public double Probability { get; private set; }
    public string Type { get; private set; }

    public PropertyDict Properties { get; private set; }
    public TmxImage Image { get; private set; }
    public TmxList<TmxObjectGroup> ObjectGroups { get; private set; }
    public Collection<TmxAnimationFrame> AnimationFrames { get; private set; }

    // Human-readable aliases to the Terrain markers
    public TmxTerrain TopLeft => TerrainEdges[0];

    public TmxTerrain TopRight => TerrainEdges[1];

    public TmxTerrain BottomLeft => TerrainEdges[2];
    public TmxTerrain BottomRight => TerrainEdges[3];

    public TmxTilesetTile(XElement xTile, TmxList<TmxTerrain> terrains, string tmxDir = "")
    {
        Id = (int)xTile.Attribute("id");

        var strTerrain = (string)xTile.Attribute("terrain") ?? ",,,";
        foreach (var v in strTerrain.Split(','))
        {
            var success = int.TryParse(v, out var result);
            var edge = success ? terrains[result] : null;
            TerrainEdges.Add(edge);

            // TODO: Assert that TerrainEdges length is 4
        }

        Probability = (double?)xTile.Attribute("probability") ?? 1.0;
        Type = (string)xTile.Attribute("type");
        Image = new TmxImage(xTile.Element("image"), tmxDir);

        ObjectGroups = [];
        foreach (var element in xTile.Elements("objectgroup"))
        {
            ObjectGroups.Add(new TmxObjectGroup(element));
        }

        AnimationFrames = [];
        var xTileElement = xTile.Element("animation");
        if (xTileElement != null)
        {
            foreach (var element in xTileElement.Elements("frame"))
            {
                AnimationFrames.Add(new TmxAnimationFrame(element));
            }
        }

        Properties = new PropertyDict(xTile.Element("properties"));
    }
}

public class TmxAnimationFrame(XElement xFrame)
{
    public int Id { get; private set; } = (int)xFrame.Attribute("tileid");
    public int Duration { get; private set; } = (int)xFrame.Attribute("duration");
}