using Game.core;
using RogueSharp;

namespace Game.systems
{
    public class MapGenerator
    {
        // dimensions of map
        private readonly int _width;
        private readonly int _height;

        // custom map class (blueprint)
        private readonly DungeonMap _map;

        // pass the dimensions of the maps to create to the generator
        public MapGenerator(int width, int height)
        {
            _width = width;
            _height = height;
            _map = new DungeonMap();
        }

        // generate a map with a simple open floor and walls on the outside
        public DungeonMap CreateMap()
        {
            // initialise every cell in the map by setting walkable, transparency, and explored to true
            _map.Initialize(_width, _height);
            foreach (Cell cell in _map.GetAllCells())
            {
                _map.SetCellProperties(cell.X, cell.Y, true, true, true);
            }

            // set the first and last rows in the map to NOT be transparent or walkable
            foreach (Cell cell in _map.GetCellsInRows(0, _height - 1))
            {
                _map.SetCellProperties(cell.X, cell.Y, false, false, true);
            }

            // set the first and last columns in the map to NOT be transparent or walkable
            foreach (Cell cell in _map.GetCellsInColumns(0, _width - 1))
            {
                _map.SetCellProperties(cell.X, cell.Y, false, false, true);
            }

            // return the generated map
            return _map;
        }
    }
}
