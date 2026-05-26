using System;

namespace SnakeGame
{
    public enum GameModePreset
    {
        Classic = 0,
        Arcade = 1,
        Maze = 2,
        SpeedRun = 3,
        Zen = 4
    }

    public enum BoardSizePreset
    {
        Compact = 0,
        Standard = 1,
        Wide = 2
    }

    public enum VisualThemePreset
    {
        Classic = 0,
        Neon = 1,
        Handheld = 2,
        Soft = 3
    }

    public sealed class GameModeSettings
    {
        public GameModeSettings(GameModePreset preset, string name, string description, int speed, bool wrapWalls, bool progressiveSpeed, bool obstacles)
        {
            Preset = preset;
            Name = name;
            Description = description;
            Speed = GameSpeed.ClampSpeed(speed);
            WrapWalls = wrapWalls;
            ProgressiveSpeed = progressiveSpeed;
            Obstacles = obstacles;
        }

        public GameModePreset Preset { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Speed { get; private set; }
        public bool WrapWalls { get; private set; }
        public bool ProgressiveSpeed { get; private set; }
        public bool Obstacles { get; private set; }
    }

    public sealed class BoardSizeSettings
    {
        public BoardSizeSettings(BoardSizePreset preset, string name, int gridWidth, int gridHeight)
        {
            Preset = preset;
            Name = name;
            GridWidth = Math.Max(1, gridWidth);
            GridHeight = Math.Max(1, gridHeight);
        }

        public BoardSizePreset Preset { get; private set; }
        public string Name { get; private set; }
        public int GridWidth { get; private set; }
        public int GridHeight { get; private set; }

        public string DisplayName
        {
            get { return Name + " " + GridWidth + "x" + GridHeight; }
        }
    }

    public static class GamePresets
    {
        public const GameModePreset DefaultMode = GameModePreset.Arcade;
        public const BoardSizePreset DefaultBoardSize = BoardSizePreset.Standard;
        public const VisualThemePreset DefaultTheme = VisualThemePreset.Classic;

        private static readonly GameModeSettings[] modes =
        {
            new GameModeSettings(GameModePreset.Classic, "Classic", "WRAP WALLS, FIXED PACE", 5, true, false, false),
            new GameModeSettings(GameModePreset.Arcade, "Arcade", "PROGRESSIVE SPEED, CLEAN GRID", 5, true, true, false),
            new GameModeSettings(GameModePreset.Maze, "Maze", "SOLID WALLS WITH OBSTACLES", 5, false, true, true),
            new GameModeSettings(GameModePreset.SpeedRun, "Speed Run", "FAST START, SOLID WALLS", 8, false, true, false),
            new GameModeSettings(GameModePreset.Zen, "Zen", "SLOW WRAP RUN, NO RUSH", 3, true, false, false)
        };

        private static readonly BoardSizeSettings[] boardSizes =
        {
            new BoardSizeSettings(BoardSizePreset.Compact, "Compact", 36, 28),
            new BoardSizeSettings(BoardSizePreset.Standard, "Standard", 44, 30),
            new BoardSizeSettings(BoardSizePreset.Wide, "Wide", 56, 30)
        };

        private static readonly string[] themeNames =
        {
            "Classic",
            "Neon",
            "Handheld",
            "Soft"
        };

        public static GameModeSettings GetMode(GameModePreset preset)
        {
            int index = ClampIndex((int)preset, modes.Length, (int)DefaultMode);
            return modes[index];
        }

        public static BoardSizeSettings GetBoardSize(BoardSizePreset preset)
        {
            int index = ClampIndex((int)preset, boardSizes.Length, (int)DefaultBoardSize);
            return boardSizes[index];
        }

        public static string GetThemeName(VisualThemePreset preset)
        {
            int index = ClampIndex((int)preset, themeNames.Length, (int)DefaultTheme);
            return themeNames[index];
        }

        public static string[] GetModeNames()
        {
            string[] names = new string[modes.Length];
            for (int i = 0; i < modes.Length; i++)
            {
                names[i] = modes[i].Name;
            }

            return names;
        }

        public static string[] GetBoardSizeNames()
        {
            string[] names = new string[boardSizes.Length];
            for (int i = 0; i < boardSizes.Length; i++)
            {
                names[i] = boardSizes[i].DisplayName;
            }

            return names;
        }

        public static string[] GetThemeNames()
        {
            string[] names = new string[themeNames.Length];
            Array.Copy(themeNames, names, themeNames.Length);
            return names;
        }

        public static GameModePreset ParseMode(int value)
        {
            return (GameModePreset)ClampIndex(value, modes.Length, (int)DefaultMode);
        }

        public static BoardSizePreset ParseBoardSize(int value)
        {
            return (BoardSizePreset)ClampIndex(value, boardSizes.Length, (int)DefaultBoardSize);
        }

        public static VisualThemePreset ParseTheme(int value)
        {
            return (VisualThemePreset)ClampIndex(value, themeNames.Length, (int)DefaultTheme);
        }

        private static int ClampIndex(int value, int count, int defaultValue)
        {
            if (value < 0 || value >= count)
            {
                return defaultValue;
            }

            return value;
        }
    }
}
