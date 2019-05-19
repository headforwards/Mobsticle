namespace Mobsticle.Logic.Mobsticle
{
    public interface IParticipant
    {
        string Name { get; }

        bool IsDriving { get; }

        bool IsDrivingNext { get; }
    }
}