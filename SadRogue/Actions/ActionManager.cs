using System;
using System.Net;
using System.Text;
using Microsoft.Xna.Framework;
using SadRogue.Entities;
using GoRogue.DiceNotation;

namespace SadRogue.Actions
{
    //Contains all generic actions performed on entities and tiles e.g. combat, movement, etc.
    public class ActionManager
    {
        // Empty Constructor

        // Stores the actor's last move action
        private Point _lastMoveActorPoint;
        private Actor _lastMoveActor;

        public bool MoveActorBy(Actor actor, Point position)
        {
            _lastMoveActor = actor;
            _lastMoveActorPoint = position;

            return actor.MoveBy(position);
        }

        public bool RedoMoveActorBy()
        {
            // Make sure there is an actor available to redo first!
            if (_lastMoveActor != null)
            {
                return _lastMoveActor.MoveBy(_lastMoveActorPoint);
            }
            else
            {
                return false;
            }
        }

        public bool UndoMoveActorBy()
        {
            // Make sure there is an actor available to redo first!
            if (_lastMoveActor != null)
            {
                // Reverse the last move
                _lastMoveActorPoint = new Point(-_lastMoveActorPoint.X, -_lastMoveActorPoint.Y);

                if (_lastMoveActor.MoveBy(_lastMoveActorPoint))
                {
                    _lastMoveActorPoint = new Point(0, 0);
                    return true;
                }
                else
                {
                    _lastMoveActorPoint = new Point(0, 0);
                    return false;
                }
            }

            return false;
        }

        // Executes an attack from an attacking actor on a defending actor, and then describes the outcome of the attack in the Message Log
        public void Attack(Actor attacker, Actor defender)
        {
            var attackMessage = new StringBuilder();
            var defenseMessage = new StringBuilder();

            var hits = ResolveAttack(attacker, defender, attackMessage);
            var blocks = ResolveDefense(defender, hits, attackMessage, defenseMessage);

            GameLoop.UIManager.MessageLog.Add(attackMessage.ToString());
            if (!string.IsNullOrWhiteSpace(defenseMessage.ToString()))
            {
                GameLoop.UIManager.MessageLog.Add(defenseMessage.ToString());
            }

            var damage = hits - blocks;

            ResolveDamage(defender, damage);
        }

        // Calculates the outcome of an attacker's attempt at scoring a hit on a defender, using the attacker's AttackChance and a random d100 roll as the basis.
        // Modifies a StringBuilder message that will be displayed in the MessageLog.
        private static int ResolveAttack(Actor attacker, Actor defender, StringBuilder attackMessage)
        {
            var hits = 0;
            attackMessage.AppendFormat("{0} attacks {1}", attacker.Name, defender.Name);

            for (var dice = 0; dice < attacker.Attack; dice++)
            {
                var diceOutcome = Dice.Roll("1d100");

                if (diceOutcome >= 100 - attacker.AttackChance)
                    hits++;
            }

            return hits;
        }

        // Calculates the outcome of the defender's attempt to block incoming hits.
        // Modifies a StringBuilder message that will be displayed in the MessageLog.
        private static int ResolveDefense(Actor defender, int hits, StringBuilder attackMessage,
            StringBuilder defenseMessage)
        {
            var blocks = 0;
            if (hits > 0)
            {
                attackMessage.AppendFormat(", scoring {0} hits.", hits);
                defenseMessage.AppendFormat("{0} defends and rolls: ", defender.Name);

                for (var dice = 0; dice < defender.Defense; dice++)
                {
                    var diceOutcome = Dice.Roll("1d100");

                    if (diceOutcome >= 100 - defender.DefenseChance)
                        blocks++;
                }

                defenseMessage.AppendFormat("resulting in {0} blocks.", blocks);
            }
            else
            {
                attackMessage.Append("and misses completely!");
            }

            return blocks;
        }

        // Calculates the damage a defender takes after asuccessful hit and subtracts it from its health.
        // Displays the outcome in the MessageLog.
        private static void ResolveDamage(Actor defender, int damage)
        {
            if (damage > 0)
            {
                defender.Health = defender.Health - damage;
                GameLoop.UIManager.MessageLog.Add($"{defender.Name} was hit for {damage} damage");
                if (defender.Health <= 0)
                {
                    ResolveDeath(defender);
                }
            }
            else
            {
                GameLoop.UIManager.MessageLog.Add($"{defender.Name} blocked all damage!");
            }
        }

        // Removes an Actor that has its health reduced to 0 and displays a message showing the amount of Gold dropped.
        private static void ResolveDeath(Actor defender)
        {
            // Dump the dead actor's inventory on the tile where it died
            foreach (var item in defender.Inventory)
            {
                item.Position = defender.Position;
                GameLoop.EntityManager.Entities.Add(item);
            }

            GameLoop.EntityManager.Entities.Remove(defender);

            if (defender is Player)
            {
                GameLoop.UIManager.MessageLog.Add($"{defender.Name} has died!");
            }
            else if (defender is NPC)
            {
                GameLoop.UIManager.MessageLog.Add($"{defender.Name} died and dropped {defender.Gold} gold coins.");
            }
        }

        // Tries to pickup an item and add it to the player's inventory.
        public void Pickup(Actor actor, Item item)
        {
            actor.Inventory.Add(item);
            GameLoop.UIManager.MessageLog.Add($"{actor.Name} picked up {item.Name}.");
            item.Destroy();
        }
    }
}