using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Taki.Main.System;

namespace Taki.Main.View
{
    public class UICircleAnimator : MonoBehaviour
    {
        [SerializeField] private List<UICircleGenerator> _circleGenerators;
        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private Ease _openEaseType = Ease.OutQuad;
        [SerializeField] private Ease _closeEaseType = Ease.InQuad;
        [SerializeField] private bool _ignoreTimeScale = false;

        private readonly List<Transform> _instantiatedItems = new();
        private readonly List<Vector3> _initialPositions = new();
        private readonly List<Vector3> _doubleRadiusPositions = new();
        private readonly List<Tween> _activeTweens = new();

        private bool _isSpecialTaskExecuting = false;

        public void Initialize()
        {
            _instantiatedItems.Clear();
            _initialPositions.Clear();
            _doubleRadiusPositions.Clear();

            _circleGenerators
                .Where(i => i != null)
                .ToList()
                .ForEach(i =>
                {
                    i.CreateItems();
                    _instantiatedItems.AddRange(i.InstantiatedItems);
                    _initialPositions.AddRange(i.CirclePoints);
                    _doubleRadiusPositions.AddRange(i.DoubleRadiusCirclePoints);
                });

            _instantiatedItems.ForEach(t => t.localPosition = Vector3.zero);
        }

        public void Dispose()
        {
            CancelAllAnimations();

            _circleGenerators
                .Where(i => i != null)
                .ToList()
                .ForEach(i => i.DestroyItems());

            _instantiatedItems.Clear();
            _initialPositions.Clear();
            _doubleRadiusPositions.Clear();
        }

        public async UniTask OpenAnimation(CancellationToken token)
        {
            if (_isSpecialTaskExecuting) return;

            CancelAllAnimations();

            var tasks = _instantiatedItems.Select((t, i) =>
            {
                var targetPosition = _initialPositions[i];
                var tween = t.DOLocalMove(targetPosition, _animationDuration)
                    .SetEase(_openEaseType)
                    .SetUpdate(_ignoreTimeScale);

                _activeTweens.Add(tween);

                return tween.ToUniTask(cancellationToken: token);
            }).ToList();

            await UniTask.WhenAll(tasks);
            _activeTweens.Clear();
        }

        public async UniTask CloseAnimation(CancellationToken token)
        {
            if (_isSpecialTaskExecuting) return;

            CancelAllAnimations();

            var tasks = _instantiatedItems.Select(t =>
            {
                var targetPosition = Vector3.zero;
                var tween = t.DOLocalMove(targetPosition, _animationDuration)
                    .SetEase(_closeEaseType)
                    .SetUpdate(_ignoreTimeScale);

                _activeTweens.Add(tween);

                return tween.ToUniTask(cancellationToken: token);
            }).ToList();

            await UniTask.WhenAll(tasks);
            _activeTweens.Clear();
        }

        public async UniTask MoveToDoubleInitialPosition(CancellationToken token)
        {
            if (_isSpecialTaskExecuting) return;

            CancelAllAnimations();

            _isSpecialTaskExecuting = true;

            var tasks = _instantiatedItems.Select((t, i) =>
            {
                var targetPosition = _doubleRadiusPositions[i];
                var tween = t.DOLocalMove(targetPosition, _animationDuration * 2)
                    .SetEase(_openEaseType)
                    .SetUpdate(_ignoreTimeScale);

                _activeTweens.Add(tween);

                return tween.ToUniTask(cancellationToken: token);
            }).ToList();

            await UniTask.WhenAll(tasks);

            _instantiatedItems.ForEach(t => t.localPosition = Vector3.zero);
            _isSpecialTaskExecuting = false;
        }

        private void CancelAllAnimations()
        {
            _activeTweens
                .Where(tween => tween != null && tween.IsActive())
                .ToList()
                .ForEach(tween => tween.Kill());

            _activeTweens.Clear();
        }
    }
}