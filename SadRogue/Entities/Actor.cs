using System;
using System.Collections.Generic;
using GoRogue;
using GoRogue.MapViews;
using Microsoft.Xna.Framework;

namespace SadRogue.Entities
{
    public abstract class Actor : SadConsole.Entities.Entity
    {
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Attack { get; set; }
        public int AttackChance { get; set; }
        public int Defense { get; set; }
        public int DefenseChance { get; set; }
        public int Gold { get; set; }
        private FOV FOVSight;
        private int VisibilityDistance = 20;

        public List<Item> Inventory = new List<Item>();

        public Actor(Color foreground, Color background, int glyph, int width = 1, int height = 1) : base(width, height)
        {
            Animation.CurrentFrame[0].Foreground = foreground;
            Animation.CurrentFrame[0].Background = background;
            Animation.CurrentFrame[0].Glyph = glyph;

            FOVSight = new FOV(GameLoop.World.CurrentMap.MapView);
        }

        public bool MoveBy(Point positionChange)
        {
            if (GameLoop.World.CurrentMap.IsTileWalkable(Position + positionChange))
            {
                // If tile is occupied by an NPC then attack.
                var npc = GameLoop.EntityManager.GetEntityAt<NPC>(Position + positionChange);
                var item = GameLoop.EntityManager.GetEntityAt<Item>(Position + positionChange);

                if (npc != null)
                {
                    GameLoop.ActionManager.Attack(this, npc);
                    return true;
                }

                if (item != null)
                {
                    GameLoop.ActionManager.Pickup(this, item);
                    return true;
                }

                Position += positionChange;
                this.UpdateFOV();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool MoveTo(Point newPosition)
        {
            Position = newPosition;
            return true;
        }

        public void UpdateFOV()
        {
            //FOVSight.Calculate(Coord.Get(Position.X, Position.Y), VisibilityDistance);
        }
    }
}