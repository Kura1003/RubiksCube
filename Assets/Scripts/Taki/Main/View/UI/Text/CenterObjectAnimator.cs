using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Taki.Main.View
{
    public sealed class CenterObjectAnimator : MonoBehaviour
    {
        [SerializeField] private Graphic _centerGraphic;
        [SerializeField] private List<RectTransform> _attachmentsToAnimate;
        [SerializeField] private List<RectTransform> _attachmentsToSetPosition;

        [SerializeField] private Color _centerGraphicColor = Color.yellow;
        [SerializeField] private float _centerGraphicDuration = 0.3f;
        [SerializeField] private Ease _centerGraphicEase = Ease.Linear;

        [SerializeField] private bool _ignoreTimeScale = false;

        [Inject] private readonly CircleTextRotator _circleTextRotator;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            _circleTextRotator.Initialize();

            _attachmentsToSetPosition.ForEach(attachment =>
            {
                attachment.position
                = _circleTextRotator
                .GetRotatorObject(_circleTextRotator.CenterIndex)
                .RectTransform
                .position;
            });

            _circleTextRotator.OnRotationComplete
                .Subscribe(_ =>
                {
                    AnimateCenterGraphicColor(
                        destroyCancellationToken)
                    .SuppressCancellationThrow()
                    .Forget();
                })
                .AddTo(this);
        }

        private async UniTask AnimateCenterGraphicColor(CancellationToken token)
        {
            var originalColor = _centerGraphic.color;
            _centerGraphic.color = _centerGraphicColor;

            await _centerGraphic
                .DOColor(originalColor, _centerGraphicDuration)
                .SetEase(_centerGraphicEase)
                .SetUpdate(_ignoreTimeScale)
                .WithCancellation(token);
        }
    }
}