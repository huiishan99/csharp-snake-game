using System;
using System.Text;

namespace SnakeGame
{
    public static class ChallengeSeed
    {
        private const int MaximumCodeLength = 16;

        public static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();
            string upper = value.Trim().ToUpperInvariant();
            for (int i = 0; i < upper.Length && builder.Length < MaximumCodeLength; i++)
            {
                char character = upper[i];
                if ((character >= 'A' && character <= 'Z') || (character >= '0' && character <= '9') || character == '-')
                {
                    builder.Append(character);
                }
            }

            return builder.ToString();
        }

        public static int? ToRandomSeed(string value)
        {
            string normalized = Normalize(value);
            if (normalized.Length == 0)
            {
                return null;
            }

            unchecked
            {
                int hash = (int)2166136261;
                for (int i = 0; i < normalized.Length; i++)
                {
                    hash ^= normalized[i];
                    hash *= 16777619;
                }

                return hash & int.MaxValue;
            }
        }

        public static string Generate(DateTime utcNow)
        {
            int dayStamp = Math.Max(0, utcNow.Year * 10000 + utcNow.Month * 100 + utcNow.Day);
            int minuteStamp = Math.Max(0, utcNow.Hour * 60 + utcNow.Minute);
            return "RUN-" + dayStamp.ToString("00000000") + "-" + minuteStamp.ToString("0000");
        }
    }
}
