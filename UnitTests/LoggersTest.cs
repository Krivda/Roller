using System;
using System.Collections.Generic;
using System.Xml.Schema;
using NLog;
using NUnit.Framework;
using RollerEngine.Logger;

namespace UnitTests
{
    internal class LoggersTest
    {
        private static readonly Logger NLogger = LogManager.GetCurrentClassLogger();

        private void TestLoggingEnums(IRollLogger logger, Verbosity verbosity, ActivityChannel channel)
        {
            logger.Log(verbosity, channel, string.Format("Testing Verbosity '{0}' in '{1}' activity channel",
                Enum.GetName(typeof(Verbosity), verbosity),
            Enum.GetName(typeof(ActivityChannel), channel)));
        }

        [Test]
        public void TestLoggers()
        {
            var logger = LoggerFactory.CreateCompositeLogger(
                Verbosity.Details,
                new List<ActivityChannel>() { ActivityChannel.Gathering },
                LoggerFactory.CreateStringBufferLogger(),
                LoggerFactory.CreateNLogLogger(NLogger));

            TestLoggingEnums(logger, Verbosity.Error, ActivityChannel.Main);                //1
            TestLoggingEnums(logger, Verbosity.Warning, ActivityChannel.Intermediate);      //2
            TestLoggingEnums(logger, Verbosity.Critical, ActivityChannel.Boost);            //3
            TestLoggingEnums(logger, Verbosity.Important, ActivityChannel.Restoration);     //4
            TestLoggingEnums(logger, Verbosity.Normal, ActivityChannel.Gathering);          //should not be counted
            TestLoggingEnums(logger, Verbosity.Details, ActivityChannel.Creation);          //5
            TestLoggingEnums(logger, Verbosity.Debug, ActivityChannel.TeachLearn);          //should not be counted
            TestLoggingEnums(logger, Verbosity.Error, ActivityChannel.Rites);               //6

            var ss = logger.Loggers[0].GetChannelLog(ActivityChannel.Main).Split(new [] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
            Assert.AreEqual(6, ss.Length);

        }
    }
}