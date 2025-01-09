using Game.core;

namespace Game.systems
{
    // carry out the commands the Player executes
    public class CommandSystem
    {
        // return value of TRUE if the Player was able to move
        // return value of FALSE if the player couldn't move, such as a wall
        public bool MovePlayer(Direction direction)
        {
            int x = Game.Player.X;
            int y = Game.Player.Y;

            switch (direction)
            {
                case Direction.Up:
                {
                    y = Game.Player.Y - 1;
                    break;
                }
                case Direction.Down:
                {
                    y = Game.Player.Y + 1;
                    break;
                }
                case Direction.Left:
                {
                    x = Game.Player.X - 1;
                    break;
                }
                case Direction.Right:
                {
                    x = Game.Player.X + 1;
                    break;
                }
                default:
                {
                    return false;
                }
            }

            // move the Player if possible and return true afterward
            if (Game.DungeonMap.SetActorPosition(Game.Player, x, y))
            {
                return true;
            }

            // if the Player couldn't be moved
            return false;
        }
    }
}