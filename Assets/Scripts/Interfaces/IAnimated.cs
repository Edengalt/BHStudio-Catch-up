namespace Interfaces
{
    public interface IAnimated
    {
        void StopAnimation();
        void StartAnim(string animName);
        void SetBool(string name, bool state);
        void SetFloat(string name, float value);
    }
}
