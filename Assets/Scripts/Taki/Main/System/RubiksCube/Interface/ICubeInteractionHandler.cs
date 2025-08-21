using Cysharp.Threading.Tasks;

namespace Taki.Main.System.RubiksCube
{
    public interface ICubeInteractionHandler
    {
        void RegisterEvents();
        void UnregisterEvents();
        UniTask ExecuteRebuild();
        UniTask ExecuteFastShuffle();
    }
}