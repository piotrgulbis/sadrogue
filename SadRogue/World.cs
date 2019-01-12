using System;
using GoRogue;
using GoRogue.MapViews;
using Microsoft.Xna.Framework;
using SadRogue.Entities;
using SharpDX.Mathematics.Interop;

namespace SadRogue
{
    // All game state data is stored in World.
    // Creates and processes generators for map creation.
    public class World
    {
        private int _mapWidth = 300;
        private int _mapHeight = 100;
        //private TileBase[] _mapTiles;
        private int _maxRooms = 100;
        private int _minRoomSize = 4;
        private int _maxRoomSize = 15;
        public Map CurrentMap { get; set; }
        public Player Player { get; set; }

        public World()
        {
            CreateMap();
            CreatePlayer();
            CreateNPC();
            CreateItems();
        }

        private void CreateMap()
        {
            //_mapTiles = new TileBase[_mapWidth * _mapHeight];
            CurrentMap = new Map(_mapWidth, _mapHeight);
            var mapGen = new MapGenerator();
            CurrentMap = mapGen.GenerateMap2(_mapWidth, _mapHeight);

            //CurrentMap = mapGen.GenerateMap(_mapWidth, _mapHeight, _maxRooms, _minRoomSize, _maxRoomSize);
        }

        private void CreatePlayer()
        {
            Player = new Player(Color.Yellow, Color.Transparent);

            // Place the player on the first non-movement-blocking tile on the map
            for (var i = 0; i < CurrentMap.Tiles.Length; i++)
            {
                if (!CurrentMap.Tiles[i].IsBlockingMove)
                {
                    // Set the player's position to the index of the current map position
                    Player.Position = SadConsole.Helpers.GetPointFromIndex(i, CurrentMap.Width);
                }
            }

            GameLoop.EntityManager.Entities.Add(Player);
        }

        private void CreateNPC()
        {
            var numNPCs = 10;
            var randNum = new Random();

            // Create several NPCs and place them on a random tile.
            // Check if the tile is blocking (e.g. a wall) and if so then try a new tile.
            for (var i = 0; i < numNPCs; i++)
            {
                var npcPosition = 0;
                NPC newNPC = new NPC(Color.Orange, Color.Transparent);

                while (CurrentMap.Tiles[npcPosition].IsBlockingMove)
                {
                    npcPosition = randNum.Next(0, CurrentMap.Width * CurrentMap.Height);
                }

                newNPC.Defense = randNum.Next(0, 10);
                newNPC.DefenseChance = randNum.Next(0, 50);
                newNPC.Attack = randNum.Next(0, 10);
                newNPC.AttackChance = randNum.Next(0, 50);
                var nameGen = new NameGenerator();
                newNPC.Name = nameGen.GenerateName();
                newNPC.Gold = randNum.Next(0, 100);

                newNPC.Position = new Point(npcPosition % CurrentMap.Width, npcPosition / CurrentMap.Width);
                GameLoop.EntityManager.Entities.Add(newNPC);
            }
        }

        private void CreateItems()
        {
            var numItems = 20;
            var randNum = new Random();

            for (var i = 0; i < numItems; i++)
            {
                var itemPosition = 0;
                var newItem = new Item(Color.LimeGreen, Color.Transparent, "Item", 'I', 2);
                while (CurrentMap.Tiles[itemPosition].IsBlockingMove)
                {
                    itemPosition = randNum.Next(0, CurrentMap.Width * CurrentMap.Height);
                }

                newItem.Position = new Point(itemPosition % CurrentMap.Width, itemPosition / CurrentMap.Width);
                GameLoop.EntityManager.Entities.Add(newItem);
            }
        }
    }
}