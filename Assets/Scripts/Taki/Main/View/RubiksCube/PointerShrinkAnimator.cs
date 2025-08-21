using UnityEngine;
using DG.Tweening;
using Taki.Utility;
using Taki.Main.System;
using System;

namespace Taki.Main.View.RubiksCube
{
    public class PointerShrinkAnimator : PointerEventHandler
    {
        [SerializeField] private float _animationDuration = 0.2f;
        [SerializeField, Range(0.0f, 0.9f)] private float _scaleMultiplier = 0.9f;
        [SerializeField] private Ease _easeType = Ease.OutQuad;
        [SerializeField] private bool _ignoreTimeScale = false;

        private Vector3 _originalScale;
        private Tween _currentTween;
        private ChangeMonitor _changeMonitor;

        [Flags]
        private enum TaskState
        {
            None = 0,
            Entered = 1 << 0,
            Exited = 1 << 1,
            Paused = 1 << 2
        }

        private TaskState _currentTaskState = TaskState.None;

        private bool ShouldUpdate =>
            !_currentTaskState.HasFlag(TaskState.Paused)
            || _changeMonitor.HasEitherChanged;

        protected override void Awake()
        {
            base.Awake();

            _originalScale = transform.localScale;
            _changeMonitor = ChangeMonitor.Instance;
        }

        private void Update()
        {
            if (!ShouldUpdate)
            {
                return;
            }

            if (_currentTaskState.HasFlag(TaskState.Entered))
            {
                OnPointerEntered();
            }

            if (_currentTaskState.HasFlag(TaskState.Exited))
            {
                OnPointerExited();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _currentTween?.Kill();
        }

        protected override void OnClicked()
        {

        }

        protected override void OnPointerEntered()
        {
            if (_currentTaskState.HasFlag(TaskState.Exited))
            {
                _currentTaskState &= ~TaskState.Exited;
                _currentTaskState |= TaskState.Entered;
                _currentTaskState |= TaskState.Paused;
                _currentTween?.Kill();
                return;
            }

            _currentTaskState |= TaskState.Entered;

            _currentTween?.Kill();

            _currentTween = transform.DOScale(
                _originalScale * _scaleMultiplier,
                _animationDuration)
                .SetEase(_easeType)
                .SetUpdate(_ignoreTimeScale)
                .OnComplete(() =>
                {
                    _currentTaskState = TaskState.None;
                });
        }

        protected override void OnPointerExited()
        {
            if (_currentTaskState.HasFlag(TaskState.Entered))
            {
                _currentTaskState &= ~TaskState.Entered;
                _currentTaskState |= TaskState.Exited;
                _currentTaskState |= TaskState.Paused;
                _currentTween?.Kill();
                return;
            }

            _currentTaskState |= TaskState.Exited;

            _currentTween?.Kill();

            _currentTween = transform.DOScale(
                _originalScale,
                _animationDuration)
                .SetEase(_easeType)
                .SetUpdate(_ignoreTimeScale)
                .OnComplete(() =>
                {
                    _currentTaskState = TaskState.None;
                });
        }
    }
}