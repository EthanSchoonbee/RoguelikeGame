using System;
using Game.core;
using Game.systems;
using RLNET;
using RogueSharp.Random;

namespace Game
{
    public static class Game
    {
        // the screen height and width in number of tiles

        // screen :
        // the root console to contain the game and its sub consoles
        private static readonly int _screenWidth = 100;
        private static readonly int _screenHeight = 70;
        private static RLRootConsole _rootConsole;

        //map :
        // the map takes up most of the screen
        // the map sub-console to contain the game's map
        private static readonly int _mapWidth = 80;
        private static readonly int _mapHeight = 48;
        private static RLConsole _mapConsole;

        //message :
        // the message sub-console to display attack tolls and other info
        // below the map
        private static readonly int _messageWidth = 80;
        private static readonly int _messageHeight = 11;
        private static RLConsole _messageConsole;

        //stat :
        // the stat sub-console diplays player and monster stats
        // to the right of the map
        private static readonly int _statWidth = 20;
        private static readonly int _statHeight = 70;
        private static RLConsole _statConsole;

        //inventory :
        // the inventory sub-console displays player's equipment, abilities, and items
        // above the map
        private static readonly int _inventoryWidth = 80;
        private static readonly int _inventoryHeight = 11;
        private static RLConsole _inventoryConsole;

        // check if a rerender is needs (initialise to true on run)
        private static bool _renderRequired = true;

        // Player
        public static Player Player { get; set; }

        // DungeonMap to generate and render
        public static DungeonMap DungeonMap { get; private set; }

        // MessageLog
        public static MessageLog MessageLog { get; private set; }

        // CommandSystem
        private static CommandSystem CommandSystem { get; set; }

        // singleton of IRandom used throughout rhe game to generate random numbers
        public static IRandom Random { get; private set; }

        static void Main(string[] args)
        {
            // establish a seed for the random number generator from the current time
            int seed = (int)DateTime.UtcNow.Ticks;
            // generate a random number based on seed
            Random = new DotNetRandom(seed);

            // exact name of bitmap font file
            string fontFileName = "assets/terminal8x8.png";

            // title for console window 
            string consoleTitle = $"RogueSharp V3 - Level 1 - Seed {seed}";

            // create a new MessageLog and print the random seed used to generate the level
            MessageLog = new MessageLog();
            MessageLog.Add("The rogue arrives on level 1");
            MessageLog.Add($"Level created with seed {seed}");

            // tell RLNET to use the bitmap font and that each tile is 8x8 pixels
            _rootConsole = new RLRootConsole(fontFileName, _screenWidth, _screenHeight,
                8, 8, 1f, consoleTitle);

            // initialize the sub consoles that will Blit to the root console
            _mapConsole = new RLConsole( _mapWidth, _mapHeight );
            _messageConsole = new RLConsole( _messageWidth, _messageHeight );
            _statConsole = new RLConsole( _statWidth, _statHeight );
            _inventoryConsole = new RLConsole( _inventoryWidth, _inventoryHeight );

            // instantiate a MapGenerator
            MapGenerator mapGenerator = new MapGenerator( _mapWidth, _mapHeight, 20, 13, 7 );

            // use the MapGenerator to create a DungeonMap
            DungeonMap = mapGenerator.CreateMap();

            // update the FOV of the map based on Player awareness
            DungeonMap.UpdatePlayerFieldOfView();

            // instantiate a CommandSystem
            CommandSystem = new CommandSystem();

            // set up a handler for RLNET's Update event
            _rootConsole.Update += OnRootConsoleUpdate;

            // set up a handler for RLNET's Render event
            _rootConsole.Render += OnRootConsoleRender;

            //REMOVE!!!!!!!!!!!!
            _inventoryConsole.SetBackColor( 0, 0, _inventoryWidth, _inventoryHeight, Palette.DbWood );
            _inventoryConsole.Print( 1, 1, "Inventory", Colors.TextHeading );

            // begin RLNET's game loop
            _rootConsole.Run();
        }

        // event handler for RLNET's Update event
        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            // flag for Player movement
            bool didPlayerAct = false;

            // capture key presses within the root console
            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();

            // check is the key press is null and if not determine if it correlates to an Action
            if (keyPress != null)
            {
                if (keyPress.Key == RLKey.Up)
                {
                    // determine if the player was able to move
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Up);
                }
                else if (keyPress.Key == RLKey.Down)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Down);
                }
                else if (keyPress.Key == RLKey.Left)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Left);
                }
                else if (keyPress.Key == RLKey.Right)
                {
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Right);
                }
                // exit the game
                else if (keyPress.Key == RLKey.Escape)
                {
                    _rootConsole.Close();
                }
            }

            if (didPlayerAct)
            {
                _renderRequired = true;
            }
        }

        // event handler for RLNET's Render event
        private static void OnRootConsoleRender ( object sender, UpdateEventArgs e)
        {
            // do not render all consoles if nothing has changed
            if (_renderRequired)
            {
                // clear consoles
                _mapConsole.Clear();
                _statConsole.Clear();
                _messageConsole.Clear();

                // draw the generated DungeonMap onto the map sub-console
                DungeonMap.Draw( _mapConsole, _statConsole );

                // draw the Player and their FOV onto the map sub-console
                Player.Draw( _mapConsole, DungeonMap );

                // draw Player Stats onto the stats sub-console
                Player.DrawStats(_statConsole);

                // draw the MessageLog to the message sub-console
                MessageLog.Draw(_messageConsole);

                // Blit the sub-consoles to the root console in the correct locations
                RLConsole.Blit( _mapConsole, 0, 0, _mapWidth, _mapHeight, _rootConsole , 0, _inventoryHeight );
                RLConsole.Blit( _statConsole, 0, 0, _statWidth, _statHeight, _rootConsole , _mapWidth, 0 );
                RLConsole.Blit( _messageConsole, 0, 0, _messageWidth, _messageHeight, _rootConsole , 0, _screenHeight - _messageHeight );
                RLConsole.Blit( _inventoryConsole, 0, 0, _inventoryWidth, _inventoryHeight, _rootConsole , 0, 0 );

                // tell RLNET to draw the console that we set
                _rootConsole.Draw();

                // reset the render flag
                _renderRequired = false;
            }
        }
    }
}
