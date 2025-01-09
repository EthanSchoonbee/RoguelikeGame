using Game.interfaces;
using RLNET;
using RogueSharp;

namespace Game.core
{
    public class Actor : IActor, IDrawable
    {
        // IActor
        private int _attack;
        private int _attackChance;
        private int _awareness;
        private int _defense;
        private int _defenseChance;
        private int _gold;
        private int _health;
        private int _maxHealth;
        private string _name;
        private int _speed;

        public int Attack
        {
            get => _attack;
            set => _attack = value;
        }

        public int AttackChance
        {
            get => _attackChance;
            set => _attackChance = value;
        }

        public int Awareness
        {
            get => _awareness;
            set => _awareness = value;
        }

        public int Defense
        {
            get => _defense;
            set => _defense = value;
        }

        public int DefenseChance
        {
            get => _defenseChance;
            set => _defenseChance = value;
        }

        public int Gold
        {
            get => _gold;
            set => _gold = value;
        }

        public int Health
        {
            get => _health;
            set => _health = value;
        }

        public int MaxHealth
        {
            get => _maxHealth;
            set => _maxHealth = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public int Speed
        {
            get => _speed;
            set => _speed = value;
        }

        // IDrawable
        public RLColor Color { get; set; }
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public void Draw(RLConsole console, IMap map)
        {
            // DO NOT draw actors in cells that haven't been explored
            if (!map.GetCell(X, Y).IsExplored)
            {
                return;
            }

            // only draw the actor with the color and symbol when they are in the FOV
            if (map.IsInFov(X, Y))
            {
                console.Set( X, Y, Color, Colors.FloorBackgroundFov, Symbol);
            }
            else
            {
                // when the actor is NOT in FOV draw a normal floor
                console.Set( X, Y, Color, Colors.FloorBackground, '.');
            }
        }

        public void DrawStats(RLConsole statConsole)
        {
            statConsole.Print( 1, 1, $"Name:    {Name}", Colors.Text );
            statConsole.Print( 1, 3, $"Health:  {Health}/{MaxHealth}", Colors.Health );
            statConsole.Print( 1, 5, $"Attack:  {Attack} ({AttackChance}%)", Colors.Text );
            statConsole.Print( 1, 7, $"Defense: {Defense} ({DefenseChance}%)", Colors.Text );
            statConsole.Print( 1, 9, $"Gold:    {Gold}", Colors.Gold );
        }
    }
}