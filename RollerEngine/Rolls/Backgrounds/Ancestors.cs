using System.Collections.Generic;
using RollerEngine.Character.Common;
using RollerEngine.Logger;
using RollerEngine.Modifiers;
using RollerEngine.Roller;

namespace RollerEngine.Rolls.Backgrounds
{
    class Ancestors : BackgroundRoll
    {

        public Ancestors(IRollLogger log, IRoller roller)
            : base(Build.Backgrounds.Ansestors, log, roller, new List<string>() {Build.Conditions.AncestorSpirits}, null, Verbosity.Details)
        {}
        

        public int Roll(Build actor, string targetTrait)
        {
            AdditionalInfo = targetTrait;


            int successes = 0;
            try
            {
                successes = base.Roll(actor, new List<Build>() { actor }, false, false);
            }
            catch (BotchException)
            {
                bool rethrow = true;
                if (Successes > -3)
                {
                    if (actor.HasAncestorVeneration)
                    {
                        Log.Log(Verbosity.Warning, string.Format("{0} botched {1} roll, but he has Ancestor Veneration and will reroll one dice.", actor.Name, Name));

                        successes = Successes + 1; //remove one failure due to reroll '1'

                        int dc = FullRollInfo.DCInfo.AdjustedDC;                        
                        var rolldata = Roller.Roll(1, dc, true, false, false, "Reroll 1 Ansestor Dice");
                        successes += rolldata.Successes; //this can be -1 for '1'; 0 if <dc ; +1 if >= dc

                        if (successes >= 0)
                        {
                            rethrow = false;
                            Log.Log(Verbosity.Warning, string.Format("{0} recovered from botch and got {1} succeses.", actor.Name, successes));
                        }
                        else
                        {
                            Log.Log(Verbosity.Warning, string.Format("{0} didn't recover from botch!", actor.Name));
                        }
                    }
                }

                if (rethrow)
                    throw;
            }

            if (successes > 0)
            {
                actor.TraitModifiers.Add(new TraitModifier(
                    Build.Backgrounds.Ansestors,
                    new List<string>() {targetTrait},
                    DurationType.Scene,
                    new List<string>(),
                    successes,
                    TraitModifier.BonusTypeKind.AdditionalDice,
                    -1
                    )
                );

                Log.Log(Verbosity.Details, string.Format("{0} obtained bonus {1} dies on {2} for a scene from {3} Background.", actor.Name, successes, targetTrait ,Name));
            }
            else
            {
                Log.Log(Verbosity.Details, string.Format("{0} didn't get bonus from {1} Background.", actor.Name, Name));
            }

            return successes;

        }

        public override int GetBaseDC(Build actor, List<Build> targets)
        {
            //base DC is 8
            return 8;
        }
    }
}