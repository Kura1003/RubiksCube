
namespace Taki.Utility
{
    public interface IUserInputLock
    {
        bool IsLocked { get; }
        void SetInputLock(bool isLocked);
    }
}