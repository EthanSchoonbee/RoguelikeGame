using RLNET;
using RogueSharp;

namespace Game.interfaces
{
    // interface to define the functionality and related behaviors of actors
    public interface IDrawable
    {
        RLColor Color { get; set; }
        char Symbol { get; set; }
        int X { get; set; }
        int Y { get; set; }

        void Draw( RLConsole console, IMap map );
    }
}