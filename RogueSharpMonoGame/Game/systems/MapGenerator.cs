﻿using System.Linq;
using Game.core;
using RogueSharp;

namespace Game.systems
{
    public class MapGenerator
    {
        // dimensions of map
        private readonly int _width;
        private readonly int _height;
        private readonly int _maxRooms;
        private readonly int _roomMaxSize;
        private readonly int _roomMinSize;

        // custom map class (blueprint)
        private readonly DungeonMap _map;

        // pass the dimensions of the maps to create as well as the sizes and maximum number of rooms to the generator
        public MapGenerator(int width, int height,
            int maxRooms, int roomMaxSize, int roomMinSize)
        {
            _width = width;
            _height = height;
            _maxRooms = maxRooms;
            _roomMaxSize = roomMaxSize;
            _roomMinSize = roomMinSize;
            _map = new DungeonMap();
        }

        // generate a map with a simple open floor and walls on the outside
        public DungeonMap CreateMap()
        {
            // set the properties of all cells to false
            _map.Initialize(_width, _height);

            // try and place as many rooms as the specified maxRooms
            // NOTE: only using decrementing loop breccias of WordPress formatting
            for (int r = _maxRooms; r > 0; r--)
            {
               // determine the size and position of the room randomly
               int roomWidth = Game.Random.Next(_roomMinSize, _roomMaxSize);
               int roomHeight = Game.Random.Next(_roomMinSize, _roomMaxSize);
               int roomXPosition = Game.Random.Next(0, _width - roomWidth - 1);
               int roomYPosition = Game.Random.Next(0, _height - roomHeight - 1);

               // all rooms can be represented as rectangles
               var newRoom = new Rectangle(roomXPosition, roomYPosition, roomWidth, roomHeight);

               //check if the room rectangle intersects with any other rooms
               bool newRoomIntersects = _map.Rooms.Any(room => newRoom.Intersects(room));

               // if it doesn't intersect add it to the list of rooms
               if (!newRoomIntersects)
               {
                   _map.Rooms.Add(newRoom);
               }
            }

            // iterate through each room that we wanted placed and call CreateRoom to make it
            foreach ( Rectangle room in _map.Rooms)
            {
                CreateRoom(room);
            }

            // Place the player after the first room is generated for the map
            PlacePlayer();

            // return the generated map
            return _map;
        }

        // given a rectangular area on the map set the cell properties for that area to true
        private void CreateRoom(Rectangle room)
        {
            for (int x = room.Left + 1; x < room.Right; x++)
            {
                for (int y = room.Top + 1; y < room.Bottom; y++)
                {
                    _map.SetCellProperties(x,y,true,true,true);
                }
            }
        }

        // when a map is generated place the Player in the center of the first room created
        private void PlacePlayer()
        {
            // get a reference to the Player
            Player player = Game.Player;

            // if no player exists then create a new one
            if (player == null)
            {
                player = new Player();
            }

            // place the player in the center of the first room created
            player.X = _map.Rooms[0].Center.X;
            player.Y = _map.Rooms[0].Center.Y;

            // call DungeonMap to add the player to the map
            _map.AddPlayer(player);
        }
    }
}
