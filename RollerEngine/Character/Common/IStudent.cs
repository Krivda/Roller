namespace RollerEngine.Character.Common
{
    public interface IStudent
    {
        Build Self { get; }

        void LearnAbility(string ability, bool withWill);
    }
}