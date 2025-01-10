using System.Text;
using Game.core;
using RogueSharp.DiceNotation;

namespace Game.systems
{
    // carry out the commands the Player executes
    public class CommandSystem
    {
        // return value of TRUE if the Player was able to move
        // return value of FALSE if the player couldn't move, such as a wall
        public bool MovePlayer(Direction direction)
        {
            int x = Game.Player.X;
            int y = Game.Player.Y;

            // move the Player if possible and return true afterward
            switch (direction)
            {
                case Direction.Up:
                {
                    y = Game.Player.Y - 1;
                    break;
                }
                case Direction.Down:
                {
                    y = Game.Player.Y + 1;
                    break;
                }
                case Direction.Left:
                {
                    x = Game.Player.X - 1;
                    break;
                }
                case Direction.Right:
                {
                    x = Game.Player.X + 1;
                    break;
                }
                default:
                {
                    return false;
                }
            }

            // check if the player cna move to the next location
            if (Game.DungeonMap.SetActorPosition(Game.Player, x, y))
            {
                // if player can be moved
                return true;
            }

            // get a monster at the coordinates the player is moving to (if there is one)
            Monster monster = Game.DungeonMap.GetMonsterAt(x, y);

            // check if there is a monster in the locaiton
            if (monster != null)
            {
                // attack the monster
                Attack(Game.Player, monster);
                return true;
            }

            // if the Player couldn't be moved
            return false;
        }

        public void Attack(Actor attacker, Actor defender)
        {
            // messages to display in the MessageLog
            StringBuilder attackMessage = new StringBuilder();
            StringBuilder defenseMessage = new StringBuilder();

            // get the number of hits from the attacker on the defender and a resultant message
            int hits = ResolveAttack(attacker, defender, attackMessage);

            // based on the number of hits get the number of successful blocks from the defence and a defense message
            int blocks = ResolveDefense(defender, hits, attackMessage, defenseMessage);

            // add the built attack message to the MessageLog
            Game.MessageLog.Add(attackMessage.ToString());

            // check if any defensive blocks were done (a defense message is present)
            if (!string.IsNullOrWhiteSpace(defenseMessage.ToString()))
            {
                // add the defense message to the MessaegLog
                Game.MessageLog.Add(defenseMessage.ToString());
            }

            // work out the damage dealt based on difference in hits and blocks
            int damage = hits - blocks;

            // inflict the damage on the defender
            ResolveDamage(defender, damage);
        }

        // method to get the number of hits an attacker has on a defender base don their Attack and AttackChance
        private static int ResolveAttack(Actor attacker, Actor defender, StringBuilder attackMessage)
        {
            // initialize the hits as 0
            int hits = 0;

            // build an initial attackMessage for the logs
            attackMessage.AppendFormat("{0} attacks {1} and rolls: ", attacker.Name, defender.Name);

            // roll a number of 100-sided dice equal to the Attack value of the attacker
            DiceExpression attackDice = new DiceExpression().Dice(attacker.Attack, 100);
            DiceResult attackResult = attackDice.Roll();

            // look for the face value of each single die that was rolled
            foreach (TermResult termResult in attackResult.Results)
            {
                // append the attack message with the attack result for that roll
                attackMessage.Append(termResult.Value + ", ");

                // compare the value to 100 minus the attack chance and add a hit if it was greater
                if (termResult.Value >= 100 - attacker.AttackChance)
                {
                    hits++;
                }
            }

            // return the number of successful hits
            return hits;
        }

        // method to get the number of blocks a defender has on an attack based on their Defence and DefenceChance
        public static int ResolveDefense(Actor defender, int hits, StringBuilder attackMessage, StringBuilder defenseMessage)
        {
            // initialize the blocksd as 0
            int blocks = 0;

            // check if any hits were dealt by the attacker
            if (hits > 0)
            {
                // append the attackMessage
                attackMessage.AppendFormat("scoring {0} hits.", hits);
                defenseMessage.AppendFormat("  {0} defends and rolls ", defender.Name);

                // roll a number of 100-sided dice equal to the Defense value of the defender
                DiceExpression defenceDice = new DiceExpression().Dice(defender.Defense, 100);
                DiceResult defenseResult = defenceDice.Roll();

                // look at the face value of each single die that was rolled
                foreach (TermResult termResult in defenseResult.Results)
                {
                    defenseMessage.Append(termResult.Value + ", ");

                    // compare the value to 100 minus the defense chance and add a block if its greater
                    if (termResult.Value >= 100 - defender.DefenseChance)
                    {
                        blocks++;
                    }
                }

                // append the defenseMessage to log
                defenseMessage.AppendFormat("resulting in {0} blocks", blocks);
            }
            else
            {
                // if no attack hits were landed on the defender
                attackMessage.Append("and misses completely.");
            }

            // return the number of blocks made
            return blocks;
        }


        // method to resolve any damage that wasnt blocked to the defender
        private static void ResolveDamage(Actor defender, int damage)
        {
            // check if any damage was dealt
            if (damage > 0)
            {
                // subtract the damage from the defenders current Health value
               defender.Health -= damage;

               // log damage done to defender
               Game.MessageLog.Add($"   {defender.Name} was hit for {damage} damage");

               // check if defender has met death conditions
               if (defender.Health <= 0)
               {
                   ResolveDeath(defender);
               }
            }
            else
            {
                Game.MessageLog.Add($"   {defender.Name} blocked all damage");
            }
        }

        // method to resolve the death of the defender and handle if they were the Player or a Monster
        private static void ResolveDeath(Actor defender)
        {
            // check if the defender was the Player
            if (defender is Player)
            {
                // end of game message
                Game.MessageLog.Add($"   {defender.Name} was killed, GAME OVER!!.");

            }
            // if the defender was a Monster
            else if (defender is Monster)
            {
                // remove the monster from the DungeonMap
                Game.DungeonMap.RemoveMonster((Monster)defender);

                // log Monster kill and drops
                Game.MessageLog.Add($"   {defender.Name} died and dropped {defender.Gold} gold");
            }
        }
    }
}