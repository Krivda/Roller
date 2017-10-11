using System;
using System.Collections.Generic;
using System.Linq;
using RollerEngine.Character.Common;
using RollerEngine.Roller;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.SpreadSheets;

namespace RollerEngine.Character
{
    public class HatysPartyLoader
    {

        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        //todo: piece of shit: breaks in parallel tests
        private static bool _expPoolStarted;

        public static HatysParty LoadParty(IRollLogger log, IRollAnalyzer roller)
        {
            var partyChars = LoadFromGoogle(log);
            return new HatysParty(partyChars, log, roller);
        }

        public static Dictionary<string, Build> LoadFromGoogle(IRollLogger log)
        {
            Dictionary<string, Build> result = new Dictionary<string, Build>();

            //load data from Goodle spreadsheet raw "Party Stats"
            IList<IList<object>> data;
            try
            {
                 data = SpreadsheetService.GetNotEmptySpreadsheetRange("1tKXkAjTaUpIDkjmCi7w1QOVbnyYU2f-KOWEnl2EAIZg", "A1:K130", "Party sheet list");
            }
            catch (Exception e)
            {
                string msg = "Can't load party charsheets from google spreadsheet";
                logger.Error(msg);
                throw new Exception(msg, e);
            }

            //parse char names from first line, get theire col indices <name, dataColIndex>
            List<Tuple<string, int>> characters = LoadCharList(data);

            //add character builds to result
            foreach (var character in characters)
            {
                result.Add(character.Item1, new Build(character.Item1));
            }

            //probe each line in data, starting from 1 (0 - are char names, already done)
            for (int i = 1; i < data.Count; i++)
            {
                Build probeBuild = result.Values.First(v => true);

                try
                {
                    //check if line contains trait values
                    string traitName = ProbeLine(probeBuild, data[i], i);

                    //if trait was resolved
                    if (!string.IsNullOrEmpty(traitName))
                    {
                        //load trait value for each character in party
                        foreach (var characterTuple in characters)
                        {
                            try
                            {
                                Tuple<int, int> traitValue = GetTraitValue(characterTuple, traitName, data[i], i);

                                if (traitValue != null)
                                {
                                    Build target = result[characterTuple.Item1];

                                    //if it a common trait
                                    if (target.Traits.ContainsKey(traitName))
                                    {
                                        target.Traits[traitName] = traitValue.Item1;

                                        if (traitValue.Item2 != 0)
                                        {
                                            //add amulet modifier
                                            result[characterTuple.Item1].AddTraitModifer(
                                                new TraitModifier("Amulet",
                                                    new List<string>() {traitName},
                                                    DurationType.Permanent,
                                                    new List<string>(),
                                                    2, //alwais 2!
                                                    TraitModifier.BonusTypeKind.TraitModLimited,
                                                    5)
                                            );
                                        }
                                    }
                                    else if (traitName.Contains(Build.DynamicTraits.ExpiriencePool))
                                    {
                                        target.Traits.Add(traitName, traitValue.Item1);
                                        target.Traits.Add(
                                            Build.DynamicTraits.GetKey(Build.DynamicTraits.ExpirienceLearned,
                                                traitName.Replace(Build.DynamicTraits.ExpiriencePool, "").Trim()),
                                            traitValue.Item2);
                                    }
                                    else if (traitName.Contains(Build.DynamicTraits.RitePool))
                                    {
                                        target.Traits.Add(traitName, traitValue.Item1);
                                        target.Traits.Add(
                                            Build.DynamicTraits.GetKey(Build.DynamicTraits.RiteLearned,
                                                traitName.Replace(Build.DynamicTraits.RitePool, "").Trim()),
                                            traitValue.Item2);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                string msg = string.Format("Can't parse line:{0} - value for character name {1}. Got exception {2}", i, characterTuple.Item1, ex);
                                logger.Error(msg);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string msg = string.Format("Can't parse line:{0}. Got exception {1}", i, ex) ;
                    logger.Error(msg);
                }
            }

            return result;
        }

        private static string ProbeLine(Build probeBuild, IList<object> line, int index)
        {
            if (line.Count==0)
            {
                logger.Trace("line {0} skipped, becuase it has no data.", index);
                return String.Empty;
            }

            string traitName = line[0].ToString();

            //skip crinos/hispo stats, take only homid
            if (traitName.Contains("Crinos") || traitName.Contains("Hispo"))
            {
                logger.Trace("line {0} skipped, becuase it's a trait '{1}' is Crinos/Hispo form.", index, traitName);
                return String.Empty;

            }

            //prep homid values
            if (traitName.Contains("Homid"))
            {
                traitName = traitName.Replace(" Homid", "");
            }

            //skip known dummies
            switch (traitName)
            {
                case "Talents:":
                case "Skills:":
                case "Knowledge:":
                case "Secondary Talents:":
                case "Secondary Skills:":
                case "Tribal Talents:":
                case "Tribal Skills:":
                case "Tribal Knowledges:":
                case "Other:":
                case "Class":
                case "Rank":
                case "Breed":
                case "Auspice":
                case "Tribe":
                case "Camp":
                case "Pack":
                case "Caern":
                    logger.Trace("line {0} skipped, becuase it is a known placeholder '{1}'", index, traitName);
                    return String.Empty;
            }

            //"Learn XP pool:":
            if (traitName.Contains("Learn XP pool:"))
            {
                _expPoolStarted = true;
                logger.Trace("line {0} skipped, becuase it is a known placeholder '{1}'", index, traitName);
                return String.Empty;
            }

            //skip "other" skills
            //Other:
            //Acrobatics, Climbing, Intrigue
            //Disguise, Escape Artistry, Fast-Draw
            //Folk Wisdom, Garou Lore
            if (traitName.Contains("Acrobatics") ||
                traitName.Contains("Disguise") ||
                traitName.Contains("Folk Wisdom"))
            {
                logger.Trace("line {0} skipped, becuase it is a known other skill '{1}'", index, traitName);
                return String.Empty;
            }

            //fix descriptions like Archery [Firearms]
            if (traitName.Contains("["))
            {
                string[] split = traitName.Split('[');
                traitName = split[0].TrimEnd();
            }

            //skip empty
            if (string.IsNullOrWhiteSpace(traitName))
            {
                logger.Trace("line {0} skipped, becuase it's empty trait.", index);
                return String.Empty;
            }

            if (_expPoolStarted)
            {
                if (typeof(Build.Abilities).GetFields().Any(info => info.GetValue(null).ToString().Equals(traitName)))
                {
                    return Build.DynamicTraits.GetKey(Build.DynamicTraits.ExpiriencePool, traitName);
                }
                
                return Build.DynamicTraits.GetKey(Build.DynamicTraits.RitePool, traitName);
            }
            else
            {
                if (probeBuild.Traits.ContainsKey(traitName))
                {
                    return traitName;
                }
            }


            logger.Warn("line {0} skipped, becuase it is an unknown trait: '{1}'", index, traitName);
            return String.Empty;

        }

        private static List<Tuple<string, int>> LoadCharList(IList<IList<object>> data)
        {
            List<Tuple<string, int>> result = new List<Tuple<string, int>>();

            for (int i = 3; i < 11; i++)
            {
                result.Add(new Tuple<string, int>(Convert.ToString(data[0][i]), i));
            }

            return result;
        }

        private static Tuple<int, int> GetTraitValue(Tuple<string, int> characterTuple, string traitName, IList<object> line, int index)
        {
            if (line.Count < characterTuple.Item2+1)
            {
                logger.Trace("Can't read trait '{0}' for character '{1}' from line {2}: data line too short.", traitName, characterTuple.Item1, index);
                return null;
            }

            int secondVal = 0;
            string strValue = Convert.ToString(line[characterTuple.Item2]).Trim();

            //fix  empty lines
            if (string.IsNullOrEmpty(strValue))
            {
                strValue = "0";
            }

            //handle amulets (3/5*)
            if (strValue.Contains("*"))
            {
                string[] split = strValue.Split('/');
                strValue = split[0].TrimEnd();

                //add amulet as a roll modifier
                secondVal = int.Parse(split[1].Replace("*", ""));
            }

            //parse exp (30(5))
            if (strValue.Contains("("))
            {
                string[] split = strValue.Split(new[] {'(', ')'}, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 2)
                {
                    throw new Exception(string.Format("invalid dynamic exp string at line {0} column {1}: {2}", index, characterTuple.Item1, strValue));
                }
                strValue = split[0].Trim();
                //add value from bracers into sec Val
                secondVal = int.Parse(split[1].Trim()); //TODO TryParse
            }

            //try parse to int
            int traitValue;
            if (! int.TryParse(strValue, out traitValue))
            {
                string err =
                    string.Format("Can't read trait '{0}' for character '{1}' from line {2}: value '{3}' is not integer.", traitName, characterTuple.Item1, index, strValue);
                logger.Error(err);
                throw new Exception(err);
            }

            return new Tuple<int, int>(traitValue, secondVal);
        }
    }
}
