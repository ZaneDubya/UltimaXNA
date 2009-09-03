using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.TileEngine
{
    public interface IWorldRendererasdf
    {
        MapObject MouseOverObject { get; }
        MapObject MouseOverGroundTile { get; }
        PickTypes PickType { get; set; }
        bool DEBUG_DrawTileOver { get; set; }
        
    }
}
