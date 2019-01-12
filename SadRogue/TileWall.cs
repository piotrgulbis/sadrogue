using System;
using Microsoft.Xna.Framework;

namespace SadRogue
{
    public class TileWall : TileBase
    {
        public TileWall(bool blocksMovement = true, bool blocksSight = true) : base(new Color(100, 100, 100), Color.Transparent,
            '#', blocksMovement, blocksSight)
        {
            Name = "Wall";
        }
    }
}