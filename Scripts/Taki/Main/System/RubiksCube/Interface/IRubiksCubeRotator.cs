using Cysharp.Threading.Tasks;
using System;
using Taki.Main.Data.RubiksCube;

namespace Taki.Main.System.RubiksCube
{
    internal interface IRubiksCubeRotator : IDisposable
    {
        UniTask ExecuteRotation(
            Face face,
            int layerIndex,
            bool isClockwise,
            bool recordRotation = true,
            bool useUnsafe = false);

        void ExecuteRotationWithoutAnimation(
            Face face,
            int layerIndex,
            bool isClockwise,
            bool recordRotation = true);

        UniTask RestoreCube(int count);

        UniTask RestoreToInitialState();
    }
}