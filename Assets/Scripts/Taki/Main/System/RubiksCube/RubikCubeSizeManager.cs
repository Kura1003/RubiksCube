using Taki.Main.Data.RubiksCube;
using UnityEngine;
using VContainer;
using Taki.Main.View;
using R3;

namespace Taki.Main.System.RubiksCube
{
    internal sealed class RubikCubeSizeManager : ICubeSizeManager
    {
        private const int INITIAL_CUBE_SIZE = 5;

        private readonly CubeSettings _cubeSettings;
        private readonly CircleTextRotator _circleTextRotator;

        private int _previousCubeSize;
        private bool _hasCubeSizeChanged;

        private CompositeDisposable _disposables = new();

        public bool HasCubeSizeChanged => _hasCubeSizeChanged;

        [Inject]
        internal RubikCubeSizeManager(
            CubeSettings cubeSettings,
            CircleTextRotator circleTextRotator)
        {
            _cubeSettings = cubeSettings;
            _circleTextRotator = circleTextRotator;

            _circleTextRotator.OnRotationComplete
                .Subscribe(_ =>
                {
                    var centerObject
                    = _circleTextRotator
                    .GetRotatorObject(
                        _circleTextRotator.CenterIndex);

                    SetCubeSize(centerObject.Index);
                })
                .AddTo(_disposables);

            SetCubeSize(INITIAL_CUBE_SIZE);
            SetPreviousCubeSize();
        }

        public float CalculateCameraYPositionFactor()
        {
            float lerpFactor = _previousCubeSize * 0.1f;
            float lerpedValue = Mathf.Lerp(0, 2.0f, lerpFactor);

            Debug.Log($"Calculated Lerped Value: {lerpedValue}");

            return lerpedValue;
        }

        private void SetCubeSize(int cubeSize)
        {
            _cubeSettings.CubeSize = cubeSize;
            _hasCubeSizeChanged = _previousCubeSize != _cubeSettings.CubeSize;
        }

        public void SetPreviousCubeSize()
        {
            _previousCubeSize = _cubeSettings.CubeSize;
            _hasCubeSizeChanged = false;
        }

        public void Dispose()
        {
            SetCubeSize(INITIAL_CUBE_SIZE);
            _disposables.Dispose();
        }
    }
}