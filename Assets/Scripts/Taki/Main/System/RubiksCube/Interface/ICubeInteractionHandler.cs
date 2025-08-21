using Cysharp.Threading.Tasks;

namespace Taki.Main.System.RubiksCube
{
    internal interface ICubeInteractionHandler
    {
        void RegisterEvents();
        void UnregisterEvents();
        UniTask ExecuteRebuild();
        UniTask ExecuteFastShuffle();
    }
}