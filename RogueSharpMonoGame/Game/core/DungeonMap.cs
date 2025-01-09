using System;
using System.Collections.Generic;
using RLNET;
using RogueSharp;

namespace Game.core
{
    // custom DungeonMap class which extends the base RogueSharp Map class
    public class DungeonMap : Map
    {
        // list of room rectangles
        public List<Rectangle> Rooms;

        public DungeonMap()
        {
            //Initialize the list of rooms when we create a new DungeonMap
            Rooms = new List<Rectangle>();
        }

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

        // method is called any time the player is moved in order to update their FOV
        public void UpdatePlayerFieldOfView()
        {
            // initialise a references to the player object
            Player player = Game.Player;

            // compute the FOV base don the player's location and awareness
            ComputeFov(player.X, player.Y, player.Awareness, true);

            // mark all cells in the players FOV has having been explored
            foreach (Cell cell in GetAllCells())
            {
                // check if the cell is in the players FOV
                if (IsInFov(cell.X, cell.Y))
                {
                    SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                }
            }
        }

        // return true when able to place the Actor on the cell or false when not
        public bool SetActorPosition(Actor actor, int x, int y)
        {
            // only allow Actors to be places in walkable cells
            if (GetCell(x, y).IsWalkable)
            {
                // the cell the Actor was previously on is now walkable
                SetIsWalkable( actor.X, actor.Y, true );

                // update the actor's position
                actor.X = x;
                actor.Y = y;

                // the new cell the Actor is on is now not walkable
                SetIsWalkable(actor.X, actor.Y, false );

                // update the FOV if we just repositioned the Player
                if (actor is Player) // check if the actor is a Player
                {
                    // update the DungeonMap with new FOV
                    UpdatePlayerFieldOfView();
                }

                // able to place Actor on cell (could be moved)
                return true;
            }

            // unable to place Actor on cell (could not be moved)
            return false;
        }

        // helper method for setting the IsWalkable property on the cell
        public void SetIsWalkable(int x, int y, bool isWalkable)
        {
            Cell cell = (Cell)GetCell(x, y);
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
        }

        // method to update the Players position and FOV on the DungeonMap
        public void AddPlayer(Player player)
        {
            // get the player
            Game.Player = player;
            // set the player position to not walkable
            SetIsWalkable(player.X, player.Y, false );
            // update the DungeonMap Player's FOV
            UpdatePlayerFieldOfView();
        }
    }
}