using Cysharp.Threading.Tasks;
using R3;
using Taki.Main.System;
using Taki.Main.System.RubiksCube;
using System.Threading;
using UnityEngine;
using VContainer;
using System.Collections.Generic;
using Taki.Audio;

namespace Taki.Main.View
{
    public sealed class PauseMenuDeactivator : MonoBehaviour
    {
        [SerializeField] private DualCanvasGroupFader _dualCanvasGroupFader;
        [SerializeField] private UICircleAnimator _circleAnimator;
        [SerializeField] private CameraShaker _cameraShaker;

        [SerializeField] private List<TextTyper> _textTypers;

        [Inject] private readonly CircleTextRotator _circleTextRotator;

        [Inject] private readonly ICubeSizeManager _cubeSizeManager;
        [Inject] private readonly ICubeInteractionHandler _cubeInteractionHandler;
        [Inject] private readonly IPauseEvents _pauseEvents;

        private Camera _mainCamera;
        private Vector3 _initialCameraLocalPosition;

        public void Awake()
        {
            _mainCamera = Camera.main;
            _initialCameraLocalPosition = _mainCamera.transform.localPosition;

            _pauseEvents.OnResumeRequested
                .Subscribe(_ =>
                OnResumeRequested(destroyCancellationToken)
                .SuppressCancellationThrow()
                .Forget())
                .AddTo(this);
        }

        private async UniTask OnResumeRequested(CancellationToken token)
        {
            _circleTextRotator.SetInputLock(true);
            _textTypers.ForEach(typer => typer.ClearImmediately());
            Time.timeScale = 1f;
            AudioManager.Instance.CurrentBgmPlayer.FadeVolume(1f, 0.5f);

            var tasks = new List<UniTask>();

            if (_cubeSizeManager.HasCubeSizeChanged)
            {
                _cubeSizeManager.SetPreviousCubeSize();
                float YPositionFactor = _cubeSizeManager.CalculateCameraYPositionFactor();

                _mainCamera.transform.localPosition =
                    new Vector3(
                        _initialCameraLocalPosition.x,
                        _initialCameraLocalPosition.y * YPositionFactor,
                        _initialCameraLocalPosition.z
                    );

                _cameraShaker.SetOriginalPosition();
                tasks.Add(_cubeInteractionHandler.ExecuteRebuild());
            }

            else
            {
                AudioManager.Instance.Play("Click", gameObject).SetVolume(0.5f);
            }

            tasks.Add(_dualCanvasGroupFader.FadeIn(token));
            _circleAnimator.MoveToDoubleInitialPosition(token).Forget();

            await UniTask.WhenAll(tasks);
        }
    }
}