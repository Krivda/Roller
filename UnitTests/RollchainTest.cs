using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using NUnit.Framework;
using RollerEngine.Character;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Roller;
using RollerEngine.Rolls;

namespace UnitTests
{
    class RollchainTest
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Test]
        public void TestNameLessBuff()
        {
            var rollLogger = new NLogLogger(Logger);
            Logger.Info("Started");
            var res = HatysParty.LoadFromGoogle(rollLogger, new OfflineDiceRoller(rollLogger));

            res.Nameless.WeeklyBoostSkill(Build.Abilities.Instruction);
        }

        [Test]
        public void TestPartyBuffStats()
        {
            var rollLogger = new NLogLogger(Logger);
            var devNullLogger = new StringBufferLogger();
            var roller = new OfflineDiceRoller(rollLogger);
            var devNullRoller = new OfflineDiceRoller(devNullLogger);

            int sumNameless = 0;
            int sumSpiridon = 0;
            int minNameless = 1000000;
            int minSpiridon = 1000000;
            int maxNameless = 0;
            int maxSpiridon = 0;
            
            var res = HatysParty.LoadFromGoogle(devNullLogger, devNullRoller);
            //var res = HatysParty.LoadFromGoogle(rollLogger, roller);

            int count = 10000;
            StringBuilder sbErr = new StringBuilder();

            for (int i = 0; i < count; i++)
            {

                //var SpiridonBonus = Build.Abilities.Instruction;
                var SpiridonBonus = Build.Abilities.Rituals;

                try
                {
                    res.StartScene();
                    res.Nameless.WeeklyPreBoost();
                    
                    /*
                    res.Spiridon.WeeklyPreBoost(Build.Abilities.Occult); //it is important to Spiridon to be second due to -1 dc of Teacher's Ease of Nameless
                    res.Nameless.WeeklyBoostSkill(Build.Abilities.Instruction);
                    res.Spiridon.WeeklyBoostSkill(SpiridonBonus);
                    */    
                    
                    res.Spiridon.WeeklyPreBoost(Build.Abilities .Empathy); //it is important to Spiridon to be second due to -1 dc of Teacher's Ease of Nameless
                    res.Spiridon.WeeklyBoostSkill(SpiridonBonus);
                    res.Nameless.WeeklyBoostSkill(Build.Abilities.Instruction);
                   

                    //Logger.Info("Started");

                    //Logger.Info("---nameless buff skipped");
                    //Logger.Info("---nameless buff skipped");
                    //Logger.Info("---nameless buff skipped");

                    //res.Spiridon.Log = rollLogger;
                    //res.Spiridon.Roller = roller;

                    //typeof(Spirdon).GetProperty("Log").SetValue(res.Spiridon, rollLogger, null);

                    //Logger.Info("---Total");
                    //Logger.Info("---Total");
                    //Logger.Info("---Total");


                    int currNameless = 0;
                    int currSpiridon = 0;

                    var tmN = res.Nameless.Self.TraitModifiers.Find(modifier =>
                        modifier.Traits.Contains(Build.Abilities.Instruction));
                    if (tmN != null)
                        currNameless = tmN.Value;

                    var tmS = res.Spiridon.Self.TraitModifiers.Find(modifier => modifier.Traits.Contains(SpiridonBonus));
                    if (tmS != null)
                        currSpiridon = tmS.Value;


                    //Logger.Info("Nameless buffed {0} on Instruct", currNameless);
                    //Logger.Info("Spiridon buffed {0} on {1}", currSpiridon, SpiridonBonus);


                    sumNameless += currNameless;
                    sumSpiridon += currSpiridon;

                    maxNameless = Math.Max(maxNameless, currNameless);
                    maxSpiridon = Math.Max(maxSpiridon, currSpiridon);

                    minNameless = Math.Min(minNameless, currNameless);
                    minSpiridon = Math.Min(minSpiridon, currSpiridon);
                }
                catch (BotchException e)
                {
                    sbErr.AppendLine(e.Message);
                }
            }

            Logger.Info("Nameless: {0} ({1}-{2})", sumNameless / (count*1.0), minNameless, maxNameless);
            Logger.Info("Spiridon: {0} ({1}-{2})", sumSpiridon / (count*1.0), minSpiridon, maxSpiridon);
            
            Logger.Info("Botched:");
            Logger.Info(sbErr);

            OfflineDiceRoller.LogStats(rollLogger);
        }


        [Test]
        public void Multiweek()
        {
            var rollLogger = new NLogLogger(Logger, Verbosity.Important);
            var devNullLogger = new StringBufferLogger();
            var roller = new OfflineDiceRoller(rollLogger);
            var devNullRoller = new OfflineDiceRoller(devNullLogger);

            var currLogger = rollLogger;
            var currRoller = roller;

            var res = HatysParty.LoadFromGoogle(currLogger, currRoller);


            res.DoWeek(1);
            res.DoWeek(2);

        }

    }
}