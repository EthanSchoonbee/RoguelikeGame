using Game.core;
using Game.systems;
using RLNET;

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

        // DungeonMap to generate and render
        public static DungeonMap DungeonMap { get; private set; }

        static void Main(string[] args)
        {
            // exact name of bitmap font file
            string fontFileName = "assets/terminal8x8.png";
            // title for console window 
            string consoleTitle = "RogueSharp V3 - Level 1";

            // tell RLNET to use the bitmap font and that each tile is 8x8 pixels
            _rootConsole = new RLRootConsole(fontFileName, _screenWidth, _screenHeight,
                8, 8, 1f, consoleTitle);

            // initialize the sub consoles that will Blit to the root console
            _mapConsole = new RLConsole( _mapWidth, _mapHeight );
            _messageConsole = new RLConsole( _messageWidth, _messageHeight );
            _statConsole = new RLConsole( _statWidth, _statHeight );
            _inventoryConsole = new RLConsole( _inventoryWidth, _inventoryHeight );

            // initialise a MapGenerator
            MapGenerator mapGenerator = new MapGenerator( _mapWidth, _mapHeight );

            // use the MapGenerator to create a DungeonMap
            DungeonMap = mapGenerator.CreateMap();

            // set up a handler for RLNET's Update event
            _rootConsole.Update += OnRootConsoleUpdate;

            // set up a handler for RLNET's Render event
            _rootConsole.Render += OnRootConsoleRender;

            // begin RLNET's game loop
            _rootConsole.Run();
        }

        // event handler for RLNET's Update event
        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            // set background color and text for each console
            // this is to verify positioning
            _mapConsole.SetBackColor( 0, 0, _mapWidth, _mapHeight, Colors.FloorBackground );
            _mapConsole.Print( 1, 1, "Map", Colors.TextHeading );

            _messageConsole.SetBackColor( 0, 0, _messageWidth, _messageHeight, Palette.DbDeepWater );
            _messageConsole.Print( 1, 1, "Messages", Colors.TextHeading );

            _statConsole.SetBackColor( 0, 0, _statWidth, _statHeight, Palette.DbOldStone );
            _statConsole.Print( 1, 1, "Stats", Colors.TextHeading );

            _inventoryConsole.SetBackColor( 0, 0, _inventoryWidth, _inventoryHeight, Palette.DbWood );
            _inventoryConsole.Print( 1, 1, "Inventory", Colors.TextHeading );
        }

        // event handler for RLNET's Render event
        private static void OnRootConsoleRender ( object sender, UpdateEventArgs e)
        {
            // Blit the sub-consoles to the root console in the correct locations
            RLConsole.Blit( _mapConsole, 0, 0, _mapWidth, _mapHeight,
                _rootConsole , 0, _inventoryHeight );
            RLConsole.Blit( _statConsole, 0, 0, _statWidth, _statHeight,
                _rootConsole , _mapWidth, 0 );
            RLConsole.Blit( _messageConsole, 0, 0, _messageWidth, _messageHeight,
                _rootConsole , 0, _screenHeight - _messageHeight );
            RLConsole.Blit( _inventoryConsole, 0, 0, _inventoryWidth, _inventoryHeight,
                _rootConsole , 0, 0 );

            // draw the generated DungeonMap to the map sub-console
            DungeonMap.Draw( _mapConsole );

            // tell RLNET to draw the console that we set
            _rootConsole.Draw();
        }
    }
}
