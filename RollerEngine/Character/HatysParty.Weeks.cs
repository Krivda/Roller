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

            //TODO: Dates, CacaoCalcs, Priorities
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
                    plan.Add(new CreateTalens(Yoki, "Cacao", "Plant"));
                    break;

                //15 Feb (no teaching week)
                case 2:
                    buffPlan.Nameless = NamelessBuff.MaxBoostSecondaryTrait(Build.Abilities.Rituals);

                    //Kinfolks learn nothing special

                    //Spiridon,Yoki: create Talens
                    plan.Add(new CreateTalens(Spiridon, "Cacao", "Plant"));
                    plan.Add(new CreateTalens(Yoki, "Cacao", "Plant"));

                    //Nameless teach Spiridon Ancestor Veneration
                    plan.Add(new TeachRiteToGarou(Nameless, Spiridon, Rite.AncestorVeneration, 1));
                    plan.Add(new LearnRiteFromGarou(Spiridon, Nameless, Rite.AncestorVeneration, 1));

                    //TODO: Spiridon should learn Fetish!! PRIORITY!!

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
                    plan.Add(new QueueRiteLearning(Kurt, Rite.TalismanAdaptation)); //Urfin has token

                    plan.Add(new QueueRiteLearning(Yoki, Rite.SpiritAwakening));
                    plan.Add(new QueueRiteLearning(Yoki, Rite.Summoning));
                    plan.Add(new QueueRiteLearning(Yoki, Rite.Contrition));
                    plan.Add(new QueueRiteLearning(Yoki, Rite.RenewingTalen));
                    plan.Add(new QueueRiteLearning(Yoki, Rite.SpiritCage)); //Yoki has token

                    plan.Add(new QueueRiteLearning(Spiridon, Rite.RenewingTalen));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.Fetish));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.SacredPeace));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.CaernBuilding));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.Signpost));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.Trepassing));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.InvitationToAncestors));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.BowelsOfMother));
                    //plan.Add(new QueueRiteLearning(Spiridon, Rite.Teachers)); - already learned
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.FeastForSpirits));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.Comfort));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.Askllepios));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.SinEatingGaia));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.SinEaterWendigo));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.Balance));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.Nightshade));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.Deliverance));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.QuestingStone));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.Becoming));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.Ostracism));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.StoneOfScorn));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.VoiceOfJackal));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.Passage));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.BaptismOfFire));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.MootRite));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.Renunciation));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.Wounding));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.HuntingPrayer));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.PrayerForPrey));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.GreetMoon));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.GreetSun));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.Adoration));
                    plan.Add(new QueueRiteLearning(Spiridon, Rite.Praise));

                    //TODO: more rites, more tokens

                    //plan.Add(new QueueRiteLearning(Spiridon, Rite.BadgersBurrow)); no token
                    //plan.Add(new QueueRiteLearning(Spiridon, Rite.OpenedBridge)); no token
                    //plan.Add(new QueueRiteLearning(Spiridon, Rite.ShroudedGlen)); no token
                    //plan.Add(new QueueRiteLearning(Spiridon, Rite.OpenedSky)); no token
                    //plan.Add(new QueueRiteLearning(Spiridon, Rite.WinterWolf)); no token
                    //plan.Add(new QueueRiteLearning(Spiridon, Rite.Totem)); no token
                    //plan.Add(new QueueRiteLearning(Spiridon, Rite.PartedVeil)); no token

                    //THERE IS NO MUCH SENSE TO LEARN THEESE RITES
                    //plan.Add(new QueueRiteLearning(Spiridon, Rite.GatheringForDeparted));   //I prefer Rite.Rememberance
                    break;

                //TODO: need to calc/have BoneRhythms and Veneration and OpenCaern
                //22 Feb (teaching week)
                case 3:
                    buffPlan.Nameless = NamelessBuff.MaxBoostInstruct();

                    //teach
                    plan.Add(new TeachAbility(Nameless, Spiridon, Build.Abilities.Performance));
                    plan.Add(new TeachAbility(Spiridon, Kinfolk1, Build.Abilities.Occult));
                    plan.Add(new TeachAbility(Kurt, Kinfolk2, Build.Abilities.SpiritLore));
                    plan.Add(new TeachAbility(Yoki, Nameless, Build.Abilities.Rituals));
                    plan.Add(new TeachAbility(Kinfolk1, Yoki, Build.Abilities.Occult));
                    plan.Add(new TeachAbility(Kinfolk2, Kurt, Build.Abilities.Firearms)); //TODO: FUCK FUCK "have more skill"
                    break;

                //1 Mar
                case 4:
                    buffPlan.Nameless = NamelessBuff.MaxBoostSecondaryTrait(Build.Abilities.Rituals);

                    //Spiridon,Yoki: create Talens
                    plan.Add(new CreateTalens(Spiridon, "Cacao", "Plant"));
                    plan.Add(new CreateTalens(Yoki, "Cacao", "Plant"));

                    //Spiridon: create fetish base for Carnyx
                    plan.Add(new CreateFetishBase(Spiridon, 4, "Carnyx of Victory"));
                    break;

                //8 Mar (teaching week, Lynn appearance)
                case 5:
                    buffPlan.Nameless = NamelessBuff.MaxBoostInstruct();

                    //teach
                    //plan.Add(new TeachAbility(Nameless, - need to give Nameless time for learning
                    plan.Add(new TeachAbility(Spiridon, Kinfolk2, Build.Abilities.Meditation));
                    plan.Add(new TeachAbility(Kurt, Kinfolk1, Build.Abilities.SpiritLore));
                    plan.Add(new TeachAbility(Yoki, Nameless, Build.Abilities.Rituals));
                    plan.Add(new TeachAbility(Kinfolk1, Yoki, Build.Abilities.Occult));
                    plan.Add(new TeachAbility(Kinfolk2, Yoki, Build.Abilities.SpiritLore));

                    //TODO: Curator? make Lynn (as theurge) our main talen creator
                    plan.Add(new QueueRiteLearning(Lynn, Rite.BoneRhythms));
                    plan.Add(new QueueRiteLearning(Lynn, Rite.OpenedCaern));
                    plan.Add(new QueueRiteLearning(Lynn, Rite.Cleansing));
                    plan.Add(new QueueRiteLearning(Lynn, Rite.TalismanDedication));
                    plan.Add(new QueueRiteLearning(Lynn, Rite.Contrition));
                    plan.Add(new QueueRiteLearning(Lynn, Rite.SpiritAwakening));
                    plan.Add(new QueueRiteLearning(Lynn, Rite.Summoning));
                    plan.Add(new QueueRiteLearning(Lynn, Rite.RenewingTalen));
                    plan.Add(new QueueRiteLearning(Lynn, Rite.Binding));
                    plan.Add(new QueueRiteLearning(Lynn, Rite.SacredFire));

                    break;

                //15 Mar
                case 6:
                    //Nameless should learn Open Caern this week
                    buffPlan.Nameless = NamelessBuff.MaxBoostSecondaryTrait(Build.Abilities.Rituals);

                    plan.Add(new CreateFetishActivity(Spiridon, 4, "Carnyx of Victory", "War"));

                    //Yoki: create Talens
                    plan.Add(new CreateTalens(Yoki, "Cacao", "Plant"));

                    //Spiridon teach Lynn Ancestor Veneration
                    plan.Add(new TeachRiteToGarou(Spiridon, Lynn, Rite.AncestorVeneration, 1));
                    plan.Add(new LearnRiteFromGarou(Lynn, Spiridon, Rite.AncestorVeneration, 1));

                    break;

                //22 Mar (teaching week)
                case 7:
                    buffPlan.Nameless = NamelessBuff.MaxBoostCarnyx();

                    //teach
                    plan.Add(new TeachAbility(Nameless, Kinfolk1, Build.Abilities.Leadership));
                    plan.Add(new TeachAbility(Spiridon, Nameless, Build.Abilities.VisageOfFenris));
                    plan.Add(new TeachAbility(Kurt, Kinfolk2, Build.Abilities.Athletics));
                    plan.Add(new TeachAbility(Yoki, Kurt, Build.Abilities.Occult));
                    plan.Add(new TeachAbility(Kinfolk1, Spiridon, Build.Abilities.Crafts));
                    plan.Add(new TeachAbility(Kinfolk2, Yoki, Build.Abilities.Meditation));
                    break;

                //29 Mar
                case 8:
                    //todo: very dirty hack
                    buffPlan.Nameless = NamelessBuff.MaxBoostSecondaryTrait(Build.Abilities.VisageOfFenris);

                    //Lynn teach Yoki Ancestor Veneration
                    plan.Add(new TeachRiteToGarou(Lynn, Yoki, Rite.AncestorVeneration, 1));
                    plan.Add(new LearnRiteFromGarou(Yoki, Lynn, Rite.AncestorVeneration, 1));
                    break;

                //05 Apr (teaching week)
                case 9:
                    //teach
                    //plan.Add(new TeachAbility(Nameless, ?, Build.Abilities.Brawl));
                    //plan.Add(new TeachAbility(Spiridon, Name, Build.Abilities.Meditation));
                    //plan.Add(new TeachAbility(Kurt, Kinfolk1, Build.Abilities.SpiritLore));
                    plan.Add(new TeachAbility(Yoki, Nameless, Build.Abilities.Occult));
                    plan.Add(new TeachAbility(Kinfolk1, Spiridon, Build.Abilities.Crafts));
                    plan.Add(new TeachAbility(Kinfolk2, Yoki, Build.Abilities.SpiritLore));
                    break;

                //plan.Add(new TeachAbility(Kurt, Yoki, Build.Abilities.SpiritLore));
                //plan.Add(new TeachAbility(Kurt, ?, Build.Abilities.SpiritLore));
                //plan.Add(new TeachAbility(Spiridon, Yoki, Build.Abilities.Herbalism));
                //plan.Add(new TeachAbility(Kinfolk2, Nameless, Build.Abilities.Survival))
                //UnbrokenCord.plan.Add(new TeachAbility(Lynn, Nameless, Build.Abilities.Enigmas));
                //plan.Add(new TeachAbility(Kinfolk2, Lynn, Build.Abilities.Medicine));
                //plan.Add(new TeachAbility(Poison, Spiridon, Build.Abilities.Poison));
                //plan.Add(new TeachGiftToGarou(Spiridon, Nameless, "Visage of Fenris"));
                //plan.Add(new TeachAbility(Spiridon, Kinfolk1, Build.Abilities.Meditation));

                //
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
		Udjin: 4 - for Rite of Fetish (3); Rite of Deliverance (3); Nightshade (4); Deliverance (3); Sin-Eater (2)
                     */

                //12 Apr
                case 10:

                    //Lynn teach Kurt Ancestor Veneration
                    plan.Add(new TeachRiteToGarou(Lynn, Kurt, Rite.AncestorVeneration, 1));
                    plan.Add(new LearnRiteFromGarou(Kurt, Lynn, Rite.AncestorVeneration, 1));


                    break;

                //19 Apr (teaching week)
                case 11:
                    break;

                //26 Apr
                case 12:
                    break;

                //3 May (teaching week)
                case 13:
                    break;

                //10 May
                case 14:
                    break;

                //17 May (teaching week)
                case 15:
                    break;

                //24 May
                case 16:
                    break;

                //31 May (teaching week, last event of 6th arc)
                case 17:
                    break;

                //07,14,21,28 June
                case 18:
                case 19:
                case 20:
                case 21:
                    break;

                //05 Jul (first event of 7th arc)
                case 22:
                    break;
            }

            if (buffPlan.Nameless == null)
            {
                buffPlan.Nameless = NamelessBuff.MaxBoostInstruct();
            }
            return weekPlan;
        }
    }

}