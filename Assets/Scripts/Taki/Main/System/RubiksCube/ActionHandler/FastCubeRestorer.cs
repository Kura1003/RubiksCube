using Cysharp.Threading.Tasks;
using Taki.Main.Data.RubiksCube;

namespace Taki.Main.System.RubiksCube
{
    internal sealed class FastCubeRestorer : ICubeActionHandler
    {
        private const float DEFAULT_ROTATION_DURATION = 0.3f;

        private readonly CubeSettings _cubeSettings;
        private readonly float _fastDuration;
        private readonly IRubiksCubeRotator _cubeRotator;

        public FastCubeRestorer(
            CubeSettings cubeSettings,
            float fastDuration,
            IRubiksCubeRotator cubeRotator)
        {
            _cubeSettings = cubeSettings;
            _fastDuration = fastDuration;
            _cubeRotator = cubeRotator;
        }

        public async UniTask Execute()
        {
            _cubeSettings.RotationDuration = _fastDuration;
            await _cubeRotator.RestoreToInitialState();
            _cubeSettings.RotationDuration = DEFAULT_ROTATION_DURATION;
        }

        public void Dispose()
        {
            _cubeSettings.RotationDuration = DEFAULT_ROTATION_DURATION;
        }
    }
}