using Cysharp.Threading.Tasks;
using R3;
using System.Linq;
using System.Threading;
using Taki.Main.Data.RubiksCube;
using Taki.Utility;
using UnityEngine;

namespace Taki.Main.System.RubiksCube
{
    internal sealed class SlideRotator : ICubeActionHandler
    {
        private readonly CubeSettings _cubeSettings;

        private int _slideCount;

        private readonly IRubiksCubeRotator _cubeRotator;
        private readonly ICubeFactory _cubeFactory;
        private readonly ICubeCancellationToken _cubeCancellationToken;

        private CompositeDisposable _disposables = new();

        public SlideRotator(
            CubeSettings cubeSettings,
            IRubiksCubeRotator cubeRotator,
            ICubeFactory cubeFactory,
            ICubeCancellationToken cubeCancellationToken)
        {
            _cubeSettings = cubeSettings;

            _cubeRotator = cubeRotator;
            _cubeFactory = cubeFactory;
            _cubeCancellationToken = cubeCancellationToken;

            SetSlideCount();

            _cubeFactory.OnCubeCreated
                .Subscribe(_ => SetSlideCount())
                .AddTo(_disposables);
        }

        private async UniTask ExecuteSingleTask(
            Face face,
            int layerIndex,
            bool isClockwise,
            CancellationToken token)
        {
            await UniTask.WaitForSeconds(
                layerIndex * 0.1f, 
                cancellationToken: token);

            await _cubeRotator.ExecuteRotation(
                face,
                layerIndex,
                isClockwise,
                recordRotation: true,
                useUnsafe: true
            );
        }

        public UniTask Execute()
        {
            bool isClockwise = RandomUtility.CoinToss();
            Face face = FaceUtility.GetRandomFace();
            var token = _cubeCancellationToken.GetToken();

            var rotationTasks = Enumerable.Range(0, _slideCount)
                                          .Select(i => ExecuteSingleTask(
                                              face,
                                              i,
                                              isClockwise,
                                              token
                                          ));

            return UniTask.WhenAll(rotationTasks);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void SetSlideCount()
        {
            _slideCount = _cubeSettings.CubeSize;
            Debug.Log($"スライド回転回数を {_slideCount} に設定しました。");
        }
    }
}