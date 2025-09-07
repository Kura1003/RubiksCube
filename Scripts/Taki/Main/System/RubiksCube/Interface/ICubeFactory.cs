using Cysharp.Threading.Tasks;
using R3;
using Taki.Main.Data.RubiksCube;
using UnityEngine;

namespace Taki.Main.System.RubiksCube
{
    internal interface ICubeFactory
    {
        Observable<Unit> OnCubeCreated { get; }
        Observable<Unit> OnCubeDestroyed { get; }

        CubeGenerationInfo Create(
            float pieceSpacing,
            int cubeSize,
            Transform parentTransform);

        UniTask<CubeGenerationInfo?> CreateAsync(
            float pieceSpacing,
            int cubeSize,
            Transform parentTransform);

        void Destroy();
    }
}