using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SadRogue.Entities
{
    public class EntityManager : SadConsole.Entities.EntityManager
    {
        // Empty Constructor

        // Checking whether a certain type of entity exists at a specified location in the manager's list of entities and if so then return it.
        public T GetEntityAt<T>(Point location) where T : SadConsole.Entities.Entity
        {
            return (T) Entities.FirstOrDefault(entity => entity is T && entity.Position == location);
        }
    }
}