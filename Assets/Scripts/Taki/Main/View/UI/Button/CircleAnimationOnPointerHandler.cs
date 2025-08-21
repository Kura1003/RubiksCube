using Cysharp.Threading.Tasks;
using Taki.Utility;
using UnityEngine;
using VContainer;

namespace Taki.Main.View
{
    public sealed class CircleAnimationOnPointerHandler : PointerEventHandler
    {
        [SerializeField] private UICircleAnimator _circleAnimator;

        protected override void Awake()
        {
            base.Awake();
            _circleAnimator.Initialize();
        }

        protected override void OnClicked()
        {
        }

        protected override void OnPointerEntered()
        {
            _circleAnimator.OpenAnimation(destroyCancellationToken)
                .SuppressCancellationThrow()
                .Forget();
        }

        protected override void OnPointerExited()
        {
            _circleAnimator.CloseAnimation(destroyCancellationToken)
                .SuppressCancellationThrow()
                .Forget();
        }
    }
}