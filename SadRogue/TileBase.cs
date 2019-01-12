using System;
using Microsoft.Xna.Framework;

namespace SadRogue
{
//TileBase is an abstract base class representing the most basic form of all tiles
    public abstract class TileBase : SadConsole.Cell
    {
        public bool IsBlockingMove;
        public bool IsBlockingSight;
        public bool Transparent;

        protected string Name;

        public TileBase(Color foreground, Color background, int glyph, bool blockingMove = false,
            bool blockingSight = false, string name = "") : base(foreground, background, glyph)
        {
            IsBlockingMove = blockingMove;
            IsBlockingSight = blockingSight;
            Transparent = !blockingSight;
            Name = name;
        }
    }
}