using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using GoRogue.GameFramework;
using GoRogue.MapViews;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Themes;
using Console = System.Console;

namespace SadRogue
{
    public class Map
    {
        private TileBase[] _tiles;
        private int _width;
        private int _height;

        public TileBase[] Tiles
        {
            get { return _tiles; }
            set { _tiles = value; }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public LambdaMapView<bool> MapView;

        public Map(int width, int height)
        {
            _width = width;
            _height = height;
            Tiles = new TileBase[width * height];
        }

        public bool IsTileWalkable(Point location)
        {
            if (location.X < 0 || location.Y < 0 || location.X >= Width || location.Y >= Height)
                return false;
            return !_tiles[location.Y * Width + location.X].IsBlockingMove;
        }

        public void GenerateMapView()
        {
            MapView = new LambdaMapView<bool>(Width, Height, pos => _tiles[pos.ToIndex(Width)].Transparent);
        }
    }
}