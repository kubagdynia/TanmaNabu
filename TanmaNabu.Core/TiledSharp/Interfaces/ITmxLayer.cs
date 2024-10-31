// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0
namespace TiledSharp;

public interface ITmxLayer : ITmxElement
{
    double? OffsetX { get; }
    double? OffsetY { get; }
    double Opacity { get; }
    PropertyDict Properties { get; }
    bool Visible { get; }
}