using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Backgrounds
{
    public class Ancestors : BackgroundRoll
    {

        public Ancestors(IRollLogger log, IRoller roller)
            : base(Build.Backgrounds.Ancestors, log, roller, new List<string>() {Build.Conditions.AncestorSpirits},
                null, Verbosity.Details)
        {
        }

        public int Roll(Build actor, string targetTrait)
        {
            AdditionalInfo = targetTrait;

            if (actor.AncestorsUsesLeft == -1)
            {
                //Ancestors won't answer this call
                Log.Log(Verbosity.Warning, ActivityChannel.Boost, string.Format("{0} won't answer {1}'s call this week due to botch!", Name, actor.Name));
                return 0;
            }

            if (actor.AncestorsUsesLeft == 0)
            {
                //Ancestors can't be called
                Log.Log(Verbosity.Warning, ActivityChannel.Boost, string.Format("{0} won't answer {1}'s call this week due to attempts!", Name, actor.Name));
                return 0;
            }

            actor.AncestorsUsesLeft--;
            base.Roll(actor, new List<Build>() {actor}, false, false);

            if (Successes < 0)
            {
                if (Successes > -3) // can be fixed with veneration.In Theory
                {
                    if (actor.HasAncestorVeneration)
                    {
                        Log.Log(Verbosity.Warning, ActivityChannel.Boost,
                            string.Format(
                                "{0} botched {1} roll, but he has Ancestor Veneration and will reroll one dice.",
                                actor.Name, Name));

                        Successes = Successes + 1; //remove one failure due to reroll '1'

                        int dc = FullRollInfo.DCInfo.AdjustedDC;
                        var rolldata = Roller.Roll(1, dc, true, false, false, "Reroll 1 Ansestor Dice");
                        Successes += rolldata.Successes; //this can be -1 for '1'; 0 if <dc ; +1 if >= dc

                        if (Successes >= 0)
                        {
                            Log.Log(Verbosity.Warning, ActivityChannel.Boost,
                                string.Format("{0} recovered from botch and got {1} succeses.", actor.Name, Successes));
                        }
                    }
                }
            }

            if (Successes > 0)
            {
                actor.TraitModifiers.Add(new TraitModifier(
                        Build.Backgrounds.Ancestors,
                        new List<string>() {targetTrait},
                        DurationType.Scene,
                        new List<string>(),
                        Successes,
                        TraitModifier.BonusTypeKind.AdditionalDice,
                        -1
                    )
                );

                Log.Log(Verbosity.Details, ActivityChannel.Boost,
                    string.Format("{0} obtained bonus {1} dies on {2} for a scene from {3} Background.", actor.Name,
                        Successes, targetTrait, Name));
            }
            else if (Successes == 0)
            {
                Log.Log(Verbosity, ActivityChannel.Boost, string.Format("{0} didn't get bonus from Ancestors!", actor.Name));
            }
            else
            {
                //botch

                //cancel any uses of Ancestors this week
                actor.AncestorsUsesLeft = -1;

                Log.Log(Verbosity.Warning, ActivityChannel.Boost,
                    string.Format("{0} bothced {1} Anscestors won't answer him for a week.", actor.Name, Name));
            }

            return Successes;
        }

        protected override int OnBotch(int successes)
        {
            return successes;
        }

        public override int GetBaseDC(Build actor, List<Build> targets)
        {
            return 8;
        }
    }
}