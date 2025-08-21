using DG.Tweening;
using UnityEngine;

namespace Taki.Main.View
{
    public class CameraShaker : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private float _shakeDuration = 0.5f;
        [SerializeField] private float _shakeStrength = 0.5f;
        [SerializeField] private int _shakeVibrato = 10;
        [SerializeField] private float _shakeRandomness = 90f;

        private Transform _cameraTransform;
        private Vector3 _originalPosition;
        private Tween _currentShakeTween;

        private void Awake()
        {
            _cameraTransform = Camera.main.transform;

            SetOriginalPosition();
        }

        private void OnDestroy()
        {
            StopCameraShake();
        }

        public void SetOriginalPosition()
        {
            _originalPosition = _cameraTransform.localPosition;
        }

        public void ShakeCamera()
        {
            StopCameraShake();
            HandleCameraShake();
        }

        private void HandleCameraShake()
        {
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _currentShakeTween = _cameraTransform.DOShakePosition(
                    _shakeDuration,
                    _shakeStrength,
                    _shakeVibrato,
                    _shakeRandomness,
                    false,
                    true)
                .SetUpdate(true)
                .OnKill(() =>
                {
                    _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    if (_cameraTransform != null)
                    {
                        _cameraTransform.localPosition = _originalPosition;
                    }
                });
        }

        private void StopCameraShake()
        {
            if (_currentShakeTween != null)
            {
                _currentShakeTween.Kill();
                _currentShakeTween = null;
            }
        }
    }
}