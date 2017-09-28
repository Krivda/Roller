using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Character.Party;
using RollerEngine.Rolls.Rites;
using RollerEngine.WeekPlan;

namespace RollerEngine.Character
{
    public partial class HatysParty
    {
        private WeekPlan GetPlanByWeekNumber(int weekNo)
        {
            List<WeeklyActivity> plan = new List<WeeklyActivity>();
            HatysBuffPlan buffPlan = new HatysBuffPlan();

            WeekPlan weekPlan = new WeekPlan()
            {
                Activities = plan,
                BuffPlan = buffPlan
            };

            switch (weekNo)
            {
                //8 Feb (teaching week)
                case 1:
                    buffPlan.Nameless = NamelessBuff.MaxBoostInstruct();

                    //teach
                    plan.Add(new TeachAbility(Nameless, Kinfolk1, Build.Abilities.Leadership));
                    plan.Add(new TeachAbility(Kurt, Yoki, Build.Abilities.Demolitions));
                    plan.Add(new TeachAbility(Kinfolk1, Kurt, Build.Abilities.Firearms));
                    //talens
                    plan.Add(new CreateTalens(Yoki, "Cacao", 1));
                    break;

                //15 Feb (no teaching week)
                case 2:
                    buffPlan.Nameless = NamelessBuff.MaxBoostInstruct();

                    //Kinfolks learn nothing special

                    //Spiridon,Yoki: create Talens
                    plan.Add(new CreateTalens(Spiridon, "Cacao", 1));
                    plan.Add(new CreateTalens(Yoki, "Cacao", 1));

                    //Nameless teach Spiridon Ancestor Veneration
                    plan.Add(new TeachRiteToGarou(Nameless, Spiridon, Rite.AncestorVeneration, 1));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.AncestorVeneration));
                    //plan.Add(new LearnRite(Spiridon, 1)); //TODO: need to select specific rite to learn this week! and priority!

                    //MAIN common RITES for nearest future (teached from summoned spirits)
                    plan.Add(new QueueRiteLearning(Nameless, Rite.OpenedCaern));
                    //plan.Add(new QueueRiteLearning(Spiridon, Rite.OpenedCaern)); - already learned
                    plan.Add(new QueueRiteLearning(Kurt, Rite.OpenedCaern));
                    plan.Add(new QueueRiteLearning(Yoki, Rite.OpenedCaern));

                    //plan.Add(new QueueRiteLearning(Nameless, Rite.BoneRhythms)); -already learned
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.BoneRhythms));
                    plan.Add(new QueueRiteLearning(Kurt, Rite.BoneRhythms));
                    plan.Add(new QueueRiteLearning(Yoki, Rite.BoneRhythms));

                    //TODO: add priority for rite learning
                    plan.Add(new QueueRiteLearning(Yoki, Rite.SpiritAwakening));
                    plan.Add(new QueueRiteLearning(Yoki, Rite.RenewingTalen));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.RenewingTalen));

                    plan.Add(new QueueRiteLearning(Spiridon, Rite.Fetish));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.InvitationToAncestors));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.FeastForSpirits));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.Teachers));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.SacredPeace));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.CaernBuilding));
                    break;

                //TODO: need to calc/have BoneRhythms and Veneration and OpenCaern
                //TODO: do fetishes as learn rite
                //22 Feb (teaching week)
                case 3:
                    buffPlan.Nameless = NamelessBuff.MaxBoostInstruct();

                    //teach
                    plan.Add(new TeachAbility(Nameless, Spiridon, Build.Abilities.Performance));
                    plan.Add(new TeachAbility(Spiridon, Kinfolk1, Build.Abilities.Occult));
                    plan.Add(new TeachAbility(Kurt, Kinfolk2, Build.Abilities.SpiritLore));
                    plan.Add(new TeachAbility(Yoki, Nameless, Build.Abilities.Rituals));
                    plan.Add(new TeachAbility(Kinfolk1, Yoki, Build.Abilities.Occult));
                    plan.Add(new TeachAbility(Kinfolk2, Kurt, Build.Abilities.Firearms));
                    break;

                case 4:
                    buffPlan.Nameless = NamelessBuff.BoostSecondaryTrait(Build.Abilities.Rituals);
                    break;

                case 5:
                    buffPlan.Nameless = NamelessBuff.MaxBoostInstruct();

                    //teach
                    //plan.Add(new TeachAbility(Nameless, ?, Build.Abilities.Brawl));                    
                    plan.Add(new TeachAbility(Spiridon, Kinfolk2, Build.Abilities.Meditation));
                    plan.Add(new TeachAbility(Kurt, Kinfolk1, Build.Abilities.SpiritLore));
                    plan.Add(new TeachAbility(Yoki, Nameless, Build.Abilities.Rituals));
                    plan.Add(new TeachAbility(Kinfolk1, Yoki, Build.Abilities.Occult));
                    //plan.Add(new TeachAbility(Kinfolk2, ?, Build.Abilities.SpiritLore));

                    plan.Add(new CreateFetishBase(Spiridon, "Carnyx of Victory"));
                    break;

                case 6:
                    break;

                case 7:
                    break;

                    //plan.Add(new TeachAbility(Kurt, Yoki, Build.Abilities.SpiritLore));
                    //plan.Add(new TeachAbility(Kurt, ?, Build.Abilities.SpiritLore));
                    //plan.Add(new TeachAbility(Spiridon, Yoki, Build.Abilities.Herbalism));
                    //plan.Add(new TeachAbility(?, Nameless, Build.Abilities.Rituals));
                    //plan.Add(new TeachAbility(?, Nameless, Build.Abilities.Occult));

                    //plan.Add(new TeachAbility(Kinfolk2, Nameless, Build.Abilities.Survival));

                    //plan.Add(new TeachAbility(Kinfolk1, Spiridon, Build.Abilities.Craft));
                    //UnbrokenCord.plan.Add(new TeachAbility(Lynn, Nameless, Build.Abilities.Enigmas));
                    //plan.Add(new TeachAbility(Kinfolk2, Lynn, Build.Abilities.Medicine));
                    //plan.Add(new TeachAbility(Poison, Spiridon, Build.Abilities.Poison));
                    //plan.Add(new TeachGiftToGarou(Spiridon, Nameless, "Visage of Fenris"));

                    //plan.Add(new TeachAbility(Spiridon, Kinfolk1, Build.Abilities.Meditation));

                    //Spiridon: create Talens
                    plan.Add(new CreateTalens(Spiridon, "Cacao", 1));
                    break;

                //


                //TODO
                //Lynn: Talisman Dedication, Cleanising
                //Ancestor Seeking: Krivda; Keltur; <Yoki> ...
                //Ancestor Veneration: Krivda; <Kurt> ...

                //Spiridon PACK of rites!!!
                /*
                     * +bone rythms
                     * 15b. rites to dictionary for planned
		Papa Serega: 5 - for Rite of Signpost(4); Rite of Trespassing(5); Crash Space (2); Shopping Chart (2)
		Golosa Vetrov: 5 - for Rite of Caern Building (5); Rite of Balance (3); Invitation to Ancestors (4)
		Babka Aine: 5 - for Rite of Sacred Peace(5)/Bowels of Mother(0), Asklepius (3), Comfort (2), Sin-Eating (3); of Teachers (1)
		Udjin: 4 - for Rite of FetishRoll (3); Rite of Deliverance (3); Nightshade (4); Deliverance (3); Sin-Eater (2)
                     */


                case 24:
                    plan.Add(new LearnRite(Spiridon, 1));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.AncestorSeeking));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.AncestorSeeking));
                    plan.Add(new LearnRite(Spiridon, 5));
                    plan.Add(new TeachAbility(Nameless, Kinfolk1, Build.Abilities.Leadership));
                    //plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.TeachAbility, Yoki, Ptitsa, Self.Abilities.Stealth)); //done
                    //plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.TeachAbility, Spiridon, Kurt, Self.Abilities.Rituals)); //can't teach that week
                    plan.Add(new TeachAbility(Kurt, Yoki, Build.Abilities.Demolitions));
                    plan.Add(new TeachAbility(Kinfolk1, Kurt, Build.Abilities.Firearms));
                    //plan.Add(new WeeklyActivity(WeeklyActivity.ActivityKind.TeachAbility, Kinfolk2, Nameless, Self.Abilities.Brawl)); //done


                    //learning
                    //plan.Add(new WeeklyActivity(Nameless, 2));
                    //plan.Add(new WeeklyActivity(Spiridon, 2));
                    //plan.Add(new WeeklyActivity(Kurt, 2));
                    plan.Add(new LearnAbility(Yoki, 4));
                    plan.Add(new LearnAbility(Yoki, 4));
                    //plan.Add(new WeeklyActivity(Kinfolk1, 2));
                    //plan.Add(new WeeklyActivity(Kinfolk2, 2));

                    break;
                case 25:
                    //learning
                    plan.Add(new LearnAbility(Nameless, 1));
                    plan.Add(new LearnAbility(Spiridon, 1));
                    plan.Add(new LearnAbility(Kurt, 1));
                    plan.Add(new LearnAbility(Yoki, 1));
                    plan.Add(new LearnAbility(Yoki, 1));
                    plan.Add(new LearnAbility(Kinfolk1, 1));
                    plan.Add(new LearnAbility(Kinfolk2, 1));

                    break;
            }

            return weekPlan;
        }
    }

}