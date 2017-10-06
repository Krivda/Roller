using System;
using System.Collections.Generic;
using System.Linq;

namespace RollerEngine.WodSystem.WTA
{
    public enum Gift
    {
        Persuasion,
        GhostPack
    }

    public static class GiftExtensions
    {
        public static GiftInfo Info(this Gift gift)
        {
            return GiftInfo.GiftsDictionary.Gifts[gift];
        }

        public static int ExpirienceCost(this GiftInfo giftInfo)
        {
            return 3 * giftInfo.Level; //TODO match auspice/breed/tribe etc
        }
    }

    public class GiftInfo
    {
        public Gift Gift { get; private set; }
        public string Name { get; private set; }
        public int Level { get; private set; }
        public List<string> Conditions { get; private set; }

        private GiftInfo(string giftName, int giftLevel, string giftKind, List<string> conditions)
        {
            Name = giftName;
            Level = giftLevel;
            Conditions = conditions;
            //TODO resolve string to obtain answers
            //TODO resolve camp?
        }

        private void Init(Gift gift)
        {
            Gift = gift;
        }

        public static GiftInfo ByName(string giftName)
        {
            return GiftsDictionary.Gifts.First(ri => ri.Value.Name.Equals(giftName)).Value;
        }

        public static class GiftsDictionary
        {
            public static Dictionary<Gift, GiftInfo> Gifts { get; private set; }

            static GiftsDictionary()
            {
                Gifts = new Dictionary<Gift, GiftInfo>
                {
                    {Gift.Persuasion, new GiftInfo("Persuasion", 1, "string", new List<string>())},
                    {Gift.GhostPack, new GiftInfo("Ghost Pack", 2, "string", new List<string>())}
                };

                foreach (var gift in Gifts)
                {
                    gift.Value.Init(gift.Key);
                }
            }
        }
    }
}