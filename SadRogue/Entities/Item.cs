using System;
using Microsoft.Xna.Framework;
using SharpDX.MediaFoundation.DirectX;
using Color = Microsoft.Xna.Framework.Color;

namespace SadRogue.Entities
{
    // Describes objects that can be picked up, used by, or destroyed by actors.
    public class Item : SadConsole.Entities.Entity
    {
        private int _condition;

        public int Weight { get; set; }

        // Physical condition of the item: 100=undamaged, 0=destroyed
        public int Condition
        {
            get { return _condition; }
            set
            {
                _condition += value;
                if (_condition <= 0)
                    Destroy();
            }
        }

        public Item(Color foreground, Color background, string name, char glyph, int weight = 1, int condition = 100,
            int width = 1, int height = 1) : base(width, height)
        {
            Animation.CurrentFrame[0].Foreground = foreground;
            Animation.CurrentFrame[0].Background = background;
            Animation.CurrentFrame[0].Glyph = glyph;
            Weight = weight;
            Condition = condition;
            Name = name;
        }

        // Destroy this object by removing it from the EntityManager's list of entities for garbage collection.
        public void Destroy()
        {
            GameLoop.EntityManager.Entities.Remove(this);
        }
    }
}