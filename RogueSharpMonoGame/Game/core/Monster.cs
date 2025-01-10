using System;
using RLNET;

namespace Game.core
{
    public class Monster : Actor
    {
        public void DrawStats(RLConsole statConsole, int position)
        {
            // start ar Y=13 which is below the player stats
            // multiply the position by 2 to leave a space between each stat
            int yPoisition = 13 + (position * 2);

            // begin the line by printing the symbol of the monster in the appropriate color
            statConsole.Print(1, yPoisition, Symbol.ToString(), Color);

            // figure out the width the health bar by deciding current Health by MaxHealth
            int width = Convert.ToInt32( ( (double)Health / (double)MaxHealth ) * 16.0);
            int remainingHealth = 16 - width;

            // set the background colors of the health bar to show how damaged the monster is
            statConsole.SetBackColor(3, yPoisition, width, 1, Palette.Primary);
            statConsole.SetBackColor(3 + width, yPoisition, remainingHealth, 1, Palette.PrimaryDarkest);

            // print the monsters name over top of the health bar
            statConsole.Print(2, yPoisition, $": {Name}", Palette.DbLight);
        }
    }
}