using Cysharp.Threading.Tasks;
using Taki.Main.Data.RubiksCube;
using Taki.Utility;

namespace Taki.Main.System.RubiksCube
{
    internal sealed class FastCubeShuffler : ICubeActionHandler
    {
        private readonly int _shuffleCount;

        private readonly IRubiksCubeRotator _cubeRotator;
        private readonly ICubeStateRestorer _cubeStateRestorer;

        public FastCubeShuffler(
            int shuffleCount, 
            IRubiksCubeRotator cubeRotator,
            ICubeStateRestorer cubeStateRestorer)
        {
            _shuffleCount = shuffleCount;

            _cubeRotator = cubeRotator;
            _cubeStateRestorer = cubeStateRestorer;
        }

        public async UniTask Execute()
        {
            var faces = FaceUtility.GetFaces(Face.Left | Face.Top | Face.Front);
            for (int i = 0; i < _shuffleCount; i++)
            {
                faces.Shuffle();
                Face face = faces[0];
                int layerIndex = RandomUtility.Range(int.MaxValue);
                bool isClockwise = RandomUtility.CoinToss();

                _cubeRotator
                    .ExecuteRotationWithoutAnimation(
                    face, 
                    layerIndex, 
                    isClockwise);
            }

            _cubeStateRestorer.RestoreAllPiecePositions();
            _cubeStateRestorer.RestoreAllPieceRotations();

            await UniTask.Yield();
        }

        public void Dispose()
        {

        }
    }
}