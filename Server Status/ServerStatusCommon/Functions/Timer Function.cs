// Copyright © - 05/10/2025 - Toby Hunter
namespace ServerSiteCommon.Functions
{
    public static class TimerFunction
    {
        // Calculates the timer duration.
        public static TimeSpan GetTimerInterval(DateTime nextElapse)
        {
            TimeSpan interval = nextElapse - DateTime.UtcNow;

            if (interval < TimeSpan.Zero)
            {
                interval = TimeSpan.Zero;
            }

            return interval;
        }
    }
}
