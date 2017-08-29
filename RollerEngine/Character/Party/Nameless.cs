using RollerEngine.Roller;
using RollerEngine.Logger;
using RollerEngine.Rolls.Backgrounds;
using RollerEngine.Rolls.Gifts;
using RollerEngine.Rolls.Skills;

namespace RollerEngine.Character.Party
{
    public class Nameless
    {
        public Build Build { get; private set; }
        public IRollLogger Log { get; private set; }
        public IRoller Roller { get; private set; }
        public const string CharacterName = "Безымянный";

        public Nameless(Build build, IRollLogger log, IRoller roller, HatysParty party)
        {
            Build = build;
            Build.Name = CharacterName;
            Log = log;
            Roller = roller;
        }

        public void WeeklyBoostSkill(string skill)
        {
            //add caern mod (+4 ancestors)
            CommonBuffs.ApplyCaernOfVigilPower(Build, Log);

            //Apply chiminage
            CommonBuffs.ApplyAncestorsChiminage(Build, Log);

            //buff Occult
            var ansestorsRoll = new Ancestors(Log, Roller);
            ansestorsRoll.Roll(Build, Build.Abilities.Occult);

            //Cast Pesuasion
            var persuasionRoll = new Persuasion(Log, Roller);
            persuasionRoll.Roll(Build, false, true);

            //Apply chiminage
            CommonBuffs.ApplyAncestorsChiminage(Build, Log);

            //roll Ghost Pack
            var ghostPackRoll = new GhostPack(Log, Roller);
            ghostPackRoll.Roll(Build, false, false);

            //Apply Bone Ryhtms
            CommonBuffs.ApplyBoneRythms(Build, Log);

            //Apply Channelling for 3 Rage
            CommonBuffs.ApplyChannelling(Build, Log, 3);

            //Apply chiminage
            CommonBuffs.ApplyAncestorsChiminage(Build, Log);

            //buff Instruct
            ansestorsRoll = new Ancestors(Log, Roller);
            ansestorsRoll.Roll(Build, skill);
        }

        public void CastTeachersEase(Build target, string ability)
        {
            //buff skill on target
            var teachersEase = new TeachersEase(Log, Roller);
            teachersEase.Roll(Build, target, ability, true, false);
        }

        public void Instruct(Build target, string ability)
        {
            //give XP to smb
            var instruct = new InstructionTeach(Log, Roller);
            instruct.Roll(Build, target, ability, true, false);
        }
    }
}