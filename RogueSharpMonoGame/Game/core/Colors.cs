using RLNET;

namespace Game.core
{
    // specifies the colors of game objects from the Palette colors
    public class Colors
    {
        // specific floor colors
        public static RLColor FloorBackground = RLColor.Black;
        public static RLColor Floor = Palette.AlternateDarkest;
        public static RLColor FloorBackgroundFov = Palette.DbDark;
        public static RLColor FloorFov = Palette.Alternate;

        // specific wall colors
        public static RLColor WallBackground = Palette.SecondaryDarkest;
        public static RLColor Wall = Palette.Secondary;
        public static RLColor WallBackgroundFov = Palette.SecondaryDarker;
        public static RLColor WallFov = Palette.SecondaryLighter;

        // specific text color
        public static RLColor TextHeading = Palette.DbLight;

        // specific player color
        public static RLColor Player = Palette.DbLight;
    }
}