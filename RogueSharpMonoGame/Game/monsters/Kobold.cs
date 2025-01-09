using System.Net.Mime;
using Game.core;
using RogueSharp.DiceNotation;

namespace Game.monsters
{
    public class Kobold : Monster
    {
        // create a new Kobold given a level of the dungeon
        public static Kobold Create(int level)
        {
            // get random health stat
            int health = Dice.Roll("2D5");

            // build the Kobold and return it
            return new Kobold
            {
                Attack = Dice.Roll("1D3") + level / 3,
                AttackChance = Dice.Roll("25D3"),
                Awareness = 10,
                Color = Colors.KoboldColor,
                Defense = Dice.Roll("1D3") + level / 3,
                DefenseChance = Dice.Roll("10D4"),
                Gold = Dice.Roll("5D5"),
                Health = health,
                MaxHealth = health,
                Name = $"Kobold {level}",
                Speed = 14,
                Symbol = 'k'
            };
        }
    }
}