using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Taki.Main.View
{
    public sealed class DualCanvasGroupFader : MonoBehaviour
    {
        [SerializeField] private List<CanvasGroup> _mainGroups = new();
        [SerializeField] private List<CanvasGroup> _oppositeGroups = new();

        [SerializeField] private float _fadeDuration = 1.0f;
        [SerializeField] private Ease _fadeEase = Ease.OutQuad;

        [SerializeField] private bool _ignoreTimeScale = false;

        private void Awake()
        {
            _oppositeGroups.ForEach(canvasGroup =>
            {
                canvasGroup.alpha = 0;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            });

            _mainGroups.ForEach(canvasGroup =>
            {
                canvasGroup.alpha = 1;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
            });
        }

        public async UniTask FadeIn(CancellationToken token)
        {
            var fadeInTasks = _mainGroups.Select(canvasGroup =>
                canvasGroup.DOFade(1.0f, _fadeDuration)
                    .SetEase(_fadeEase)
                    .SetUpdate(_ignoreTimeScale) 
                    .ToUniTask(cancellationToken: token));

            var fadeOutTasks = _oppositeGroups.Select(canvasGroup =>
                canvasGroup.DOFade(0.0f, _fadeDuration)
                    .SetEase(_fadeEase)
                    .SetUpdate(_ignoreTimeScale) 
                    .ToUniTask(cancellationToken: token));

            _oppositeGroups.ForEach(canvasGroup =>
            {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            });

            await UniTask.WhenAll(fadeInTasks.Concat(fadeOutTasks));

            _mainGroups.ForEach(canvasGroup =>
            {
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
            });
        }

        public async UniTask FadeOut(CancellationToken token)
        {
            var fadeInTasks = _oppositeGroups.Select(canvasGroup =>
                canvasGroup.DOFade(1.0f, _fadeDuration)
                    .SetEase(_fadeEase)
                    .SetUpdate(_ignoreTimeScale) 
                    .ToUniTask(cancellationToken: token));

            var fadeOutTasks = _mainGroups.Select(canvasGroup =>
                canvasGroup.DOFade(0.0f, _fadeDuration)
                    .SetEase(_fadeEase)
                    .SetUpdate(_ignoreTimeScale) 
                    .ToUniTask(cancellationToken: token));

            _mainGroups.ForEach(canvasGroup =>
            {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            });

            await UniTask.WhenAll(fadeInTasks.Concat(fadeOutTasks));

            _oppositeGroups.ForEach(canvasGroup =>
            {
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
            });
        }
    }
}