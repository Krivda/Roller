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
        public void GetPartySpreadsheet()
        {
            var data = SpreadsheetService.GetNotEmptySpreadsheetRange("1tKXkAjTaUpIDkjmCi7w1QOVbnyYU2f-KOWEnl2EAIZg", "A1:J130", "Party sheet list");
            Assert.AreEqual("Talents:", data[28][0]);
            Assert.AreEqual("Skills:", data[40][0]);
            Assert.AreEqual("Knowledge:", data[60][0]);
        }

        [Test]
        public void TestPartyLoad()
        {
            IRollLogger logger = CompositeLogger.InitLogging(Verbosity.Debug, null, null, null);
            var res = HatysPartyLoader.LoadFromGoogle(logger);

            Assert.AreEqual(8, res.Count, "should load 8 characters");

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