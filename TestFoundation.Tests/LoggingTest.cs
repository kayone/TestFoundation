using NLog;
using NUnit.Framework;
using Kayone.TestFoundation;

namespace Kayone.TestFoundation.Tests
{
    [TestFixture]
    public class LoggingTest : TestCore
    {
        Logger logger = LogManager.GetCurrentClassLogger(); 

        [Test]
        public void logs_should_be_sent_to_LogVerification()
        {
            logger.Error("Some error");

            ExceptionVerification.ExpectedErrors(1);
        }
    }
}
