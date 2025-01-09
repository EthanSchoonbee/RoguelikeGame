using Game.interfaces;
using RLNET;
using RogueSharp;

namespace Game.core
{
    public class Actor : IActor, IDrawable
    {
        // IActor
        public string Name { get; set; }
        public int Awareness { get; set; }

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
    }
}