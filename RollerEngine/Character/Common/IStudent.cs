namespace RollerEngine.Character.Common
{
    public interface IStudent
    {
        Build Self { get; }

        void Learn(string ability, bool withWill);
    }
}