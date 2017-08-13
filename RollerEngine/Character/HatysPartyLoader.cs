using System;
using System.Collections.Generic;
using System.Linq;
using Makedonsky.MapLogic.SpreadSheets;
using NLog;
using RollerEngine.Character.Modifiers;

namespace RollerEngine.Character
{
    public class HatysPartyLoader
    {

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static Dictionary<string, Build> LoadFromGoogle()
        {
            Dictionary<string, Build> result = new Dictionary<string, Build>();

            //load data from Goodle spreadsheet raw "Party Stats"
            IList<IList<object>> data;
            try
            {
                 data = SpreadsheetService.GetNotEmptySpreadsheetRange("1tKXkAjTaUpIDkjmCi7w1QOVbnyYU2f-KOWEnl2EAIZg", "A1:J90", "Party sheet list");
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
                            Tuple<int, bool> traitValue = GetTraitValue(characterTuple, traitName, data[i], i);

                            if (traitValue != null)
                            {
                                result[characterTuple.Item1].Traits[traitName] = traitValue.Item1;

                                if (traitValue.Item2)
                                {
                                    //add amulet modifier
                                    result[characterTuple.Item1].AddTraitModifer(
                                        new TraitModifier("Amulet",
                                            new List<string>() {traitName}, 
                                            DurationType.Scene, 
                                            new List<string>(), 
                                            2, //alwais 2!
                                            TraitModifier.BonusTypeKind.TraitModLimited,
                                            5)
                                    );
                                }

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
                    logger.Trace("line {0} skipped, becuase it is a known placeholder '{1}'", index, traitName);
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

            if (probeBuild.Traits.ContainsKey(traitName))
            {
                return traitName;
            }

            logger.Warn("line {0} skipped, becuase it is an unknown trait: '{1}'", index, traitName);
            return String.Empty;

        }

        private static List<Tuple<string, int>> LoadCharList(IList<IList<object>> data)
        {
            List<Tuple<string, int>> result = new List<Tuple<string, int>>();

            for (int i = 3; i < 10; i++)
            {
                result.Add(new Tuple<string, int>(Convert.ToString(data[0][i]), i));
            }

            return result;
        }

        private static Tuple<int, bool> GetTraitValue(Tuple<string, int> characterTuple, string traitName, IList<object> line, int index)
        {
            if (line.Count < characterTuple.Item2+1)
            {
                logger.Trace("Can't read trait '{0}' for character '{1}' from line {2}: data line too short.", traitName, characterTuple.Item1, index);
                return null;
            }

            bool addAmulet = false;
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
                addAmulet = true;
            }

            //try parse to int
            int traitValue;
            if (! int.TryParse(strValue, out traitValue))
            {
                logger.Error("Can't read trait '{0}' for character '{1}' from line {2}: value '{3}' is not integer.", traitName, characterTuple.Item1, index, strValue);
                return null;
            }

            return new Tuple<int, bool>(traitValue, addAmulet);
        }

    }
}
