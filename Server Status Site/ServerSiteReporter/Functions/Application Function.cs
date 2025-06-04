namespace ServerSiteReporter.Functions
{
    internal class ApplicationFunction
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
