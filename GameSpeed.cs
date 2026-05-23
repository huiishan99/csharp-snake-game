using System;

namespace SnakeGame
{
    public static class GameSpeed
    {
        public const int MinimumSpeed = 1;
        public const int MaximumSpeed = 10;
        public const int DefaultSpeed = 5;
        public const bool DefaultProgressiveSpeed = true;

        private const int MinimumTimerInterval = 80;
        private const int BaseTimerInterval = 320;
        private const int TimerIntervalStep = 22;
        private const int ProgressiveScoreStep = 40;
        private const int ProgressiveTimerStep = 8;
        private const int ProgressiveTimerBonusMax = 96;

        public static int ClampSpeed(int speed)
        {
            return ClampSpeed(speed, MinimumSpeed, MaximumSpeed);
        }

        public static int ClampSpeed(int speed, int minimumSpeed, int maximumSpeed)
        {
            int lowerBound = Math.Min(minimumSpeed, maximumSpeed);
            int upperBound = Math.Max(minimumSpeed, maximumSpeed);
            return Math.Max(lowerBound, Math.Min(upperBound, speed));
        }

        public static int GetTimerInterval(int selectedSpeed, int score, bool useProgressiveSpeed)
        {
            int clampedSpeed = ClampSpeed(selectedSpeed);
            int interval = BaseTimerInterval - clampedSpeed * TimerIntervalStep - GetProgressiveTimerBonus(score, useProgressiveSpeed);
            return Math.Max(MinimumTimerInterval, interval);
        }

        public static string GetDisplayLabel(int selectedSpeed, bool useProgressiveSpeed)
        {
            return "Speed: " + GetDisplayValue(selectedSpeed, useProgressiveSpeed);
        }

        public static string GetDisplayValue(int selectedSpeed, bool useProgressiveSpeed)
        {
            return ClampSpeed(selectedSpeed) + (useProgressiveSpeed ? "+" : string.Empty);
        }

        private static int GetProgressiveTimerBonus(int score, bool useProgressiveSpeed)
        {
            if (!useProgressiveSpeed)
            {
                return 0;
            }

            int scoreSteps = Math.Max(0, score / ProgressiveScoreStep);
            return Math.Min(ProgressiveTimerBonusMax, scoreSteps * ProgressiveTimerStep);
        }
    }
}
