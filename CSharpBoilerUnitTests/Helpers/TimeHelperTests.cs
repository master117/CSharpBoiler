using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpBoiler.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace CSharpBoiler.Helpers.Tests
{
    [TestClass()]
    public class TimeHelperTests
    {
        [TestMethod()]
        public void UnixTimeStampInSecondsToDateTimeTest_WhenZero()
        {
            //arange
            long zeroLong = 0;
            DateTime firstUnixDateTime = new DateTime(1970, 1, 1);
            
            //act
            DateTime zeroDateTime = TimeHelper.UnixTimeStampInSecondsToDateTime(zeroLong);

            //assert
            Assert.AreEqual(zeroDateTime, firstUnixDateTime);
        }

        [TestMethod()]
        public void UnixTimeStampInSecondsToDateTimeTest_WhenNow()
        {
            //arange
            DateTime nowDateTimeArrange = DateTime.Now;
            //remove miliseconds, since UnixTimeStampInSecondsToDateTime only works on seconds.
            nowDateTimeArrange = new DateTime(
                nowDateTimeArrange.Ticks - (nowDateTimeArrange.Ticks % TimeSpan.TicksPerSecond),
                nowDateTimeArrange.Kind);
            DateTime firstUnixDateTime = new DateTime(1970, 1, 1);
            long nowLong = (long)(nowDateTimeArrange - firstUnixDateTime).TotalSeconds;

            //act
            DateTime nowDateTime = TimeHelper.UnixTimeStampInSecondsToDateTime(nowLong);

            //assert
            Assert.AreEqual(nowDateTime, nowDateTimeArrange);
        }

        [TestMethod()]
        public void UnixTimeStampInSecondsToDateTimeTest_WhenTimeStampIsTooLarge_ShouldThrowArgumentOutOfRangeException()
        {
            //arange
            long maxLong = (long)(DateTime.MaxValue - DateTime.MinValue).TotalSeconds;
            
            try
            {
                //act
                DateTime maxDateTime = TimeHelper.UnixTimeStampInSecondsToDateTime(maxLong);
            }
            catch (ArgumentOutOfRangeException)
            {
                //assert
                return;
            }            
            Assert.Fail();
        }
    }
}
