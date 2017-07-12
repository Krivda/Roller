using Makedonsky.MapLogic.SpreadSheets;
using NUnit.Framework;
using RollerEngine.SpreadSheets;

namespace UnitTests
{
    public class GsheetApiTest
    {
        [Test]
        public void TestAPI()
        {
            ApiTest.Test();

        }

        [Test]
        public void GetPartySpreadsheet()
        {
            var data = SpreadsheetService.GetNotEmptySpreadsheetRange("1tKXkAjTaUpIDkjmCi7w1QOVbnyYU2f-KOWEnl2EAIZg", "A1:J93", "Party sheet list");
            Assert.AreEqual("Attributes:", data[0][0]);
        }
    }
}