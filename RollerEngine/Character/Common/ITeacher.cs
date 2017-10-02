namespace RollerEngine.Character.Common
{
    public interface ITeacher
    {
        Build Self { get; }
        void Teach(IStudent student, string ability, bool hasWill);
    }
}