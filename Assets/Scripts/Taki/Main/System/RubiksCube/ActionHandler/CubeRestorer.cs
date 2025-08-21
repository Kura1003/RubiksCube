using Cysharp.Threading.Tasks;

namespace Taki.Main.System.RubiksCube
{
    internal sealed class CubeRestorer : ICubeActionHandler
    {
        private readonly IRubiksCubeRotator _cubeRotator;

        public CubeRestorer(IRubiksCubeRotator cubeRotator)
        {
            _cubeRotator = cubeRotator;
        }

        public UniTask Execute()
        {
            return _cubeRotator.RestoreToInitialState();
        }

        public void Dispose()
        {

        }
    }
}