using NUnit.Framework;
using RollerEngine.Character;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Rolls;
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

        [Test]
        public void TestPartyLoad()
        {
            var res = HatysPartyLoader.LoadFromGoogle(new StringBufferLogger());

            Assert.AreEqual(7, res.Count, "should load 7 characters");

            Assert.AreEqual(7, res["Krivda"].Traits[Build.RollableTraits.Gnosis], "krivda has Gnosis 7");
            Assert.AreEqual(2, res["Alisa"].Traits[Build.Atributes.Dexterity], "krivda has Dex 2");
            Assert.AreEqual(3, res["Keltur"].Traits[Build.Abilities.Intimidation], "Keltur has Initimidation 3");
            //amulet?
            var modifiers = res["Keltur"].TraitModifiers.FindAll(tm => tm.Traits.Contains(Build.Abilities.Intimidation));

            Assert.AreEqual(1, modifiers.Count, "Keltur has amulet on Initimidation.");
            Assert.AreEqual(5, modifiers[0].Limit, "Keltur has amulet on Initimidation with limit 5.");

        }
    }
}