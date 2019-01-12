using System;
using Microsoft.Xna.Framework;

namespace SadRogue
{
    public class TileFloor : TileBase
    {
        public TileFloor(bool blocksMovement = false, bool blocksSight = false) : base(Color.DarkGray, Color.Transparent,
            '.', blocksMovement, blocksSight)
        {
            Name = "Floor";
        }
    }
}