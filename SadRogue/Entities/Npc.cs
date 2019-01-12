using System;
using Microsoft.Xna.Framework;

namespace SadRogue.Entities
{
    // A Generic NPC capable of interaction and combat and yields treasure upon death.
    public class NPC : Actor
    {
        public NPC(Color foreground, Color background) : base(foreground, background, 'N')
        {
            var randNum = new Random();

            int itemNum = randNum.Next(1, 4);

            for (var i = 0; i < itemNum; i++)
            {
                var newItem = new Item(Color.HotPink, Color.Transparent, "NPC Item", 'L', 2);
                Inventory.Add(newItem);
            }
        }
    }
}
