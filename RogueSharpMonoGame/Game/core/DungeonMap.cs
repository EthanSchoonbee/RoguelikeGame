using System;
using System.Collections.Generic;
using RLNET;
using RogueSharp;
using RogueSharp.MapCreation;

namespace Game.core
{
    // custom DungeonMap class which extends the base RogueSharp Map class
    public class DungeonMap : Map
    {
        // list of room rectangles
        public List<Rectangle> Rooms;

        // list of monsters
        private readonly List<Monster> _monsters;

        public DungeonMap()
        {
            // initialize the list of monsters when we create a new DungeonMap
            _monsters = new List<Monster>();
            // initialize the list of rooms when we create a new DungeonMap
            Rooms = new List<Rectangle>();
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

        // method to add monsters to the map
        public void AddMonster(Monster monster)
        {
            _monsters.Add(monster);
            // after adding a monster to the map ensure the cell is not walkable
            SetIsWalkable(monster.X, monster.Y, false );
        }

        // helper method for setting the IsWalkable property on the cell
        public void SetIsWalkable(int x, int y, bool isWalkable)
        {
            Cell cell = (Cell)GetCell(x, y);
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
        }

        // look for a random location in a room tha is walkable
        public Point? GetRandomWalkableLocationInRoom( Rectangle room )
        {
            if ( DoesRoomHaveWalkableSpace( room ) )
            {
                for ( int i = 0; i < 100; i++ )
                {
                    int x = Game.Random.Next( 1, room.Width - 2 ) + room.X;
                    int y = Game.Random.Next( 1, room.Height - 2 ) + room.Y;
                    if ( IsWalkable( x, y ) )
                    {
                        return new Point( x, y );
                    }
                }
            }

            // If we didn't find a walkable location in the room return null
            return null;
        }

        // iterate through each Cell in a room and return true if any are walkable
        public bool DoesRoomHaveWalkableSpace(Rectangle room)
        {
            for (int x = 1; x <= room.Width - 2; x++)
            {
                for (int y = 1; y <= room.Height - 2; y++)
                {
                    if (IsWalkable(x + room.X, y + room.Y))
                    {
                        // there is at lease one walkable cell in the room
                        return true;
                    }
                }
            }
            // there are no walkable cells in the room
            return false;
        }

        // Draw method to eb called each time the map is updated
        // renders all the symbols/colors for each cell to the map sub-console
        public void Draw(RLConsole mapConsole)
        {
            // draw each cell onto the map
            foreach (Cell cell in GetAllCells())
            {
                SetConsoleSymbolForCell( mapConsole, cell ); // set the cell
            }

            // draw each monster onto the map
            foreach (Monster monster in _monsters)
            {
                monster.Draw(mapConsole, this);
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