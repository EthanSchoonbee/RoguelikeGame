using System;
using RLNET;
using RogueSharp;

namespace Game.core
{
    // custom DungeonMap class which extends the base RogueSharp Map class
    public class DungeonMap : Map
    {
        // Draw method to eb called each time the map is updated
        // renders all the symbols/colors for each cell to the map sub-console
        public void Draw(RLConsole mapConsole)
        {
            mapConsole.Clear();
            foreach (Cell cell in GetAllCells())
            {
                SetConsoleSymbolForCell( mapConsole, cell ); // set the cell
            }
        }

        private void SetConsoleSymbolForCell(RLConsole console, Cell cell)
        {
            // when the cell has not been explored yet, dont draw anything
            if (!cell.IsExplored)
            {
                return;
            }

            // if cell is currently in the FOV it should be drawn in lighter colors
            if (IsInFov(cell.X, cell.Y))
            {
                // choose the symbol to draw based on of the cell is walkable or not
                // "." for floor and "#" for wall
                if (cell.IsWalkable) // is a floor in FOV
                {
                    console.Set(cell.X, cell.Y, Colors.FloorFov, Colors.FloorBackgroundFov, '.');
                }
                else // is a wall in FOV
                {
                    console.Set(cell.X, cell.Y, Colors.WallFov, Colors.WallBackgroundFov, '#');
                }
            }
            // a cell is outside the FOV draw it in darker colors
            else
            {
                if (cell.IsWalkable) // is a floor outside FOV
                {
                    console.Set(cell.X, cell.Y, Colors.Floor, Colors.FloorBackground, '.');
                }
                else // is a wall outside FOV
                {
                    console.Set(cell.X, cell.Y, Colors.Wall, Colors.WallBackground, '#');
                }
            }
        }
    }
}