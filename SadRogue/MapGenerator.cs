using System;
using System.Collections.Generic;
using System.Linq;
using GoRogue;
using GoRogue.MapViews;
using Microsoft.Xna.Framework;
using SadRogue.Entities;
using Troschuetz.Random;
using Troschuetz.Random.Generators;

namespace SadRogue
{
    public class MapGenerator
    {
        // Empty Constructor

        private Map _map;

        //public Map GenerateMap(int mapWidth, int mapHeight, int maxRooms, int minRoomSize, int maxRoomSize)
        //{
        //    _map = new Map(mapWidth, mapHeight);
        //    var randNum = new Random();

        //    List<Rectangle> rooms = new List<Rectangle>();

        //    for (var i = 0; i < maxRooms; i++)
        //    {
        //        var newRoomWidth = randNum.Next(minRoomSize, maxRoomSize);
        //        var newRoomHeight = randNum.Next(minRoomSize, maxRoomSize);

        //        var newRoomX = randNum.Next(0, mapWidth - newRoomWidth - 1);
        //        var newRoomY = randNum.Next(0, mapHeight - newRoomHeight - 1);

        //        var newRoom = new Rectangle(newRoomX, newRoomY, newRoomWidth, newRoomHeight);

        //        var newRoomIntersects = rooms.Any(room => newRoom.Intersects(room));

        //        if (!newRoomIntersects)
        //            rooms.Add(newRoom);
        //    }

        //    FloodWalls();

        //    foreach (var room in rooms)
        //    {
        //        CreateRoom(room);
        //    }

        //    for (var r = 1; r < rooms.Count; r++)
        //    {
        //        var previousRoomCenter = rooms[r - 1].Center;
        //        var currentRoomCenter = rooms[r].Center;

        //        if (randNum.Next(1, 2) == 1)
        //        {
        //            CreateHorizontalTunnel(previousRoomCenter.X, currentRoomCenter.X, previousRoomCenter.Y);
        //            CreateVerticalTunnel(previousRoomCenter.Y, currentRoomCenter.Y, currentRoomCenter.X);
        //        }
        //        else
        //        {
        //            CreateVerticalTunnel(previousRoomCenter.Y, currentRoomCenter.Y, previousRoomCenter.X);
        //            CreateHorizontalTunnel(previousRoomCenter.X, currentRoomCenter.X, currentRoomCenter.Y);
        //        }
        //    }

        //    return _map;
        //}

        public Map GenerateMap2(int mapWidth, int mapHeight)
        {
            _map = new Map(mapWidth, mapHeight);

            var myMap = new ArrayMap<bool>(mapWidth, mapHeight);

            //GoRogue.MapGeneration.QuickGenerators.GenerateRectangleMap(myMap);
            GoRogue.MapGeneration.QuickGenerators.GenerateRandomRoomsMap(myMap, 100, 4, 15);
            //GoRogue.MapGeneration.QuickGenerators.GenerateCellularAutomataMap(myMap);
            //IGenerator iGen = new XorShift128Generator();
            //GoRogue.MapGeneration.QuickGenerators.GenerateDungeonMazeMap(myMap,iGen.Next(100), 100, 4, 25);

            foreach (var pos in myMap.Positions()) // Iterates through all positions in the map
            {
                if (myMap[pos])
                    _map.Tiles[pos.ToIndex(myMap.Width)] = new TileFloor(); // Use Coord's ToIndex function to convert from 2d Position to 1D index, and assign to the tile array
                else
                    _map.Tiles[pos.ToIndex(myMap.Width)] = new TileWall();
            }

            _map.GenerateMapView();

            return _map;
        }

        private void CreateRoom(Microsoft.Xna.Framework.Rectangle room)
        {
            for (var x = room.Left + 1; x < room.Right - 1; x++)
            {
                for (var y = room.Top + 1; y < room.Bottom - 1; y++)
                {
                    CreateFloor(new Point(x, y));
                }
            }

            List<Point> perimeter = GetBorderCellLocations(room);
            foreach (var location in perimeter)
            {
                CreateWall(location);
            }
        }

        private void CreateFloor(Point location)
        {
            _map.Tiles[location.ToIndex(_map.Width)] = new TileFloor();
        }

        private void CreateWall(Point location)
        {
            _map.Tiles[location.ToIndex(_map.Width)] = new TileWall();
        }

        private void FloodWalls()
        {
            for (var i = 0; i < _map.Tiles.Length; i++)
            {
                _map.Tiles[i] = new TileWall();
            }
        }

        private List<Point> GetBorderCellLocations(Microsoft.Xna.Framework.Rectangle room)
        {
            var xMin = room.Left;
            var xMax = room.Right;
            var yMin = room.Top;
            var yMax = room.Bottom;

            var borderCells = new List<Point>();
            borderCells.AddRange(GetTileLocationsAlongLine(xMin, yMin, xMax, yMin));
            borderCells.AddRange(GetTileLocationsAlongLine(xMin, yMin, xMin, yMax));
            borderCells.AddRange(GetTileLocationsAlongLine(xMin, yMax, xMax, yMax));
            borderCells.AddRange(GetTileLocationsAlongLine(xMax, yMin, xMax, yMax));

            return borderCells;
        }

        public IEnumerable<Point> GetTileLocationsAlongLine(int xOrigin, int yOrigin, int xDestination, int yDestination)
        {
            xOrigin = ClampX(xOrigin);
            yOrigin = ClampY(yOrigin);
            xDestination = ClampX(xDestination);
            yDestination = ClampY(yDestination);

            var dx = Math.Abs(xDestination - xOrigin);
            var dy = Math.Abs(yDestination - yOrigin);

            var sx = xOrigin < xDestination ? 1 : -1;
            var sy = yOrigin < yDestination ? 1 : -1;
            var err = dx - dy;

            while (true)
            {
                yield return new Point(xOrigin, yOrigin);
                if (xOrigin == xDestination && yOrigin == yDestination)
                {
                    break;
                }

                var e2 = 2 * err;

                if (e2 > -dy)
                {
                    err = err - dy;
                    xOrigin = xOrigin + sx;
                }

                if (e2 < dx)
                {
                    err = err + dx;
                    yOrigin = yOrigin + sy;
                }
            }
        }

        private int ClampX(int x)
        {
            if (x < 0)
                x = 0;
            else if (x > _map.Width - 1)
                x = _map.Width - 1;
            return x;
            // OR using ternary conditional operators: return(x<0)?0:(x>_map.Width-1)?_map.Width-1:x;
        }

        private int ClampY(int y)
        {
            if (y < 0)
                y = 0;
            else if (y > _map.Height - 1)
                y = _map.Height - 1;
            return y;
            //OR using ternary conditional operators: return(y<0)?0:(y>_map.Height-1)?_map.Height-1:y;
        }

        private void CreateHorizontalTunnel(int xStart, int xEnd, int yPosition)
        {
            for (var x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
            {
                CreateFloor(new Point(x, yPosition));
            }
        }

        private void CreateVerticalTunnel(int yStart, int yEnd, int xPosition)
        {
            for (var y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
            {
                CreateFloor(new Point(xPosition, y));
            }
        }
    }
}