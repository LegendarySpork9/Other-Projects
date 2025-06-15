using ServerSiteCommon.Functions;

namespace ServerSite.Tests.Common.Functions
{
    [TestClass]
    public class TimerFunctionTest
    {
        [TestMethod]
        public void TestTimerInterval()
        {
            DateTime now = DateTime.UtcNow;
            TimeSpan interval = TimerFunction.GetTimerInterval(now.AddMilliseconds(-now.Millisecond).AddMinutes(5));

            Assert.IsTrue(interval > TimeSpan.Zero);
        }

        [TestMethod]
        public void TestTimerIntervalFail()
        {
            TimeSpan interval = TimerFunction.GetTimerInterval(DateTime.UtcNow);

            Assert.IsTrue(interval == TimeSpan.Zero);
        }
    }
}
