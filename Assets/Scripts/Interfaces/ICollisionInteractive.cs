namespace Interfaces
{
    public interface ICollisionInteractive 
    {
        void OnCollision(CollisionTracker col);
    }
    
    public interface ICollisionInteractiveWithEndAction : ICollisionInteractive
    {
        void EndAction();
    }
}
