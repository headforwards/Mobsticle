namespace Mobsticle.Interface
{
    public interface IParticipant
    {
        string Name { get; }

        bool IsDriving { get; }

        bool IsDrivingNext { get; }        
    }
}