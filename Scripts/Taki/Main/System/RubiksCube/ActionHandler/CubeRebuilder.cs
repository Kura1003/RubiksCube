using Cysharp.Threading.Tasks;
using Taki.Audio;
using Taki.Main.Data.RubiksCube;
using Taki.Main.View;
using UnityEngine;

namespace Taki.Main.System.RubiksCube
{
    internal sealed class CubeRebuilder : ICubeActionHandler
    {
        private readonly Transform _cubePivot;
        private readonly CameraShaker _cameraShaker;
        private readonly CubeSettings _cubeSettings;

        private readonly ICubeFactory _cubeFactory;
        private readonly ICubeDataProvider _cubeDataProvider;
        private readonly IRotationAxisProvider _rotationAxisProvider;

        private bool _isExecutingTask;

        public CubeRebuilder(
            Transform cubePivot,
            CameraShaker cameraShaker,
            CubeSettings cubeSettings,
            ICubeFactory cubeFactory,
            ICubeDataProvider cubeDataProvider,
            IRotationAxisProvider rotationAxisProvider)
        {
            _cubePivot = cubePivot;
            _cameraShaker = cameraShaker;
            _cubeSettings = cubeSettings;

            _cubeFactory = cubeFactory;
            _cubeDataProvider = cubeDataProvider;
            _rotationAxisProvider = rotationAxisProvider;
        }

        public async UniTask Execute()
        {
            if (_isExecutingTask)
            {
                AudioManager
                    .Instance
                    .Play(
                    "Block", 
                    _cubePivot.gameObject)
                    .SetVolume(1f);

                return;
            }

            AudioManager
                .Instance
                .Play(
                "Destroy",
                _cubePivot.gameObject)
                .SetVolume(1f);

            _isExecutingTask = true;

            _cubeFactory.Destroy();
            _cameraShaker.ShakeCamera();

            var cubeSize = _cubeSettings.CubeSize;
            var pieceSpacing = _cubeSettings.PieceSpacing;

            var generationInfo = await _cubeFactory.CreateAsync(
                pieceSpacing,
                cubeSize,
                _cubePivot);

            if (generationInfo is null)
            {
                _isExecutingTask = false;
                return;
            }

            _cubeDataProvider.Setup(
                generationInfo.Value.FaceManagersMap,
                cubeSize);

            _rotationAxisProvider.SetUp(
                generationInfo.Value.AxisInfoMap,
                _cubePivot);

            _isExecutingTask = false;
        }

        public void Dispose()
        {
            _isExecutingTask = false;
        }
    }
}