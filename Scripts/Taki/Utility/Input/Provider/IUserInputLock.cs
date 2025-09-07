
namespace Taki.Utility
{
    internal interface IUserInputLock
    {
        bool IsLocked { get; }
        void SetInputLock(bool isLocked);
    }
}