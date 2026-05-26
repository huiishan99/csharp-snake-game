using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SnakeGame
{
    public sealed class LeaderboardEntry
    {
        public LeaderboardEntry(string modeName, int score, string speedLabel, string boardName, string challengeSeed, DateTime playedAtUtc)
        {
            ModeName = Clean(modeName);
            Score = Math.Max(0, score);
            SpeedLabel = Clean(speedLabel);
            BoardName = Clean(boardName);
            ChallengeSeed = Clean(challengeSeed);
            PlayedAtUtc = playedAtUtc;
        }

        public string ModeName { get; private set; }
        public int Score { get; private set; }
        public string SpeedLabel { get; private set; }
        public string BoardName { get; private set; }
        public string ChallengeSeed { get; private set; }
        public DateTime PlayedAtUtc { get; private set; }

        internal string Serialize()
        {
            return string.Join(
                "\t",
                new[]
                {
                    ModeName,
                    Score.ToString(CultureInfo.InvariantCulture),
                    SpeedLabel,
                    BoardName,
                    ChallengeSeed,
                    PlayedAtUtc.Ticks.ToString(CultureInfo.InvariantCulture)
                });
        }

        internal static bool TryDeserialize(string line, out LeaderboardEntry entry)
        {
            entry = null;
            if (string.IsNullOrWhiteSpace(line))
            {
                return false;
            }

            string[] parts = line.Split('\t');
            if (parts.Length != 6)
            {
                return false;
            }

            int score;
            long ticks;
            if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out score)
                || !long.TryParse(parts[5], NumberStyles.Integer, CultureInfo.InvariantCulture, out ticks))
            {
                return false;
            }

            DateTime playedAtUtc;
            try
            {
                playedAtUtc = new DateTime(ticks, DateTimeKind.Utc);
            }
            catch
            {
                playedAtUtc = DateTime.UtcNow;
            }

            entry = new LeaderboardEntry(parts[0], score, parts[2], parts[3], parts[4], playedAtUtc);
            return true;
        }

        private static string Clean(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value.Replace("\t", " ").Replace("\r", " ").Replace("\n", " ").Trim();
        }
    }

    public sealed class LeaderboardStore
    {
        private const int MaxEntriesPerMode = 5;
        private readonly List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

        public void Add(LeaderboardEntry entry)
        {
            if (entry == null)
            {
                return;
            }

            entries.Add(entry);
            TrimMode(entry.ModeName);
        }

        public IList<LeaderboardEntry> GetTopEntries(string modeName)
        {
            List<LeaderboardEntry> modeEntries = GetSortedModeEntries(modeName);
            if (modeEntries.Count > MaxEntriesPerMode)
            {
                modeEntries.RemoveRange(MaxEntriesPerMode, modeEntries.Count - MaxEntriesPerMode);
            }

            return modeEntries;
        }

        public string Serialize()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < entries.Count; i++)
            {
                if (i > 0)
                {
                    builder.AppendLine();
                }

                builder.Append(entries[i].Serialize());
            }

            return builder.ToString();
        }

        public static LeaderboardStore Deserialize(string data)
        {
            LeaderboardStore store = new LeaderboardStore();
            if (string.IsNullOrWhiteSpace(data))
            {
                return store;
            }

            string[] lines = data.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                LeaderboardEntry entry;
                if (LeaderboardEntry.TryDeserialize(line, out entry))
                {
                    store.Add(entry);
                }
            }

            return store;
        }

        private void TrimMode(string modeName)
        {
            List<LeaderboardEntry> modeEntries = GetSortedModeEntries(modeName);
            for (int i = MaxEntriesPerMode; i < modeEntries.Count; i++)
            {
                entries.Remove(modeEntries[i]);
            }
        }

        private List<LeaderboardEntry> GetSortedModeEntries(string modeName)
        {
            List<LeaderboardEntry> modeEntries = new List<LeaderboardEntry>();
            foreach (LeaderboardEntry entry in entries)
            {
                if (string.Equals(entry.ModeName, modeName, StringComparison.OrdinalIgnoreCase))
                {
                    modeEntries.Add(entry);
                }
            }

            modeEntries.Sort(CompareEntries);
            return modeEntries;
        }

        private static int CompareEntries(LeaderboardEntry left, LeaderboardEntry right)
        {
            int scoreComparison = right.Score.CompareTo(left.Score);
            if (scoreComparison != 0)
            {
                return scoreComparison;
            }

            return right.PlayedAtUtc.CompareTo(left.PlayedAtUtc);
        }
    }
}
