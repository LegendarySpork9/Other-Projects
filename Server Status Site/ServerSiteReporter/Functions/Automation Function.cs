using ServerSiteReporter.Models;

namespace ServerSiteReporter.Functions
{
    internal class AutomationFunction
    {
        public TimeSpan GetTimerInterval(DateTime nextElapse)
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
