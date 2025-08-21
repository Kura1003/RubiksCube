using Cysharp.Threading.Tasks;
using Taki.Main.Data.RubiksCube;
using Taki.Utility;

namespace Taki.Main.System.RubiksCube
{
    internal sealed class CubeShuffler : ICubeActionHandler
    {
        private readonly int _shuffleCount;
        private readonly IRubiksCubeRotator _cubeRotator;

        public CubeShuffler(
            int shuffleCount,
            IRubiksCubeRotator cubeRotator)
        {
            _shuffleCount = shuffleCount;
            _cubeRotator = cubeRotator;
        }

        public async UniTask Execute()
        {
            for (int i = 0; i < _shuffleCount; i++)
            {
                Face face = FaceUtility.GetRandomFace();
                int layerIndex = RandomUtility.Range(int.MaxValue);
                bool isClockwise = RandomUtility.CoinToss();

                await _cubeRotator.ExecuteRotation(face, layerIndex, isClockwise);
            }
        }

        public void Dispose()
        {

        }
    }
}