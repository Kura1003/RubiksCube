using Taki.Main.View;
using Taki.Utility;
using VContainer;

namespace Taki.Main.System
{
    public sealed class ResumeButtonHandler : PointerEventHandler
    {
        [Inject] private readonly CircleTextRotator _circleTextRotator;

        [Inject] private readonly IPauseEvents _pauseEvents;

        protected override void OnClicked()
        {
            if (_circleTextRotator.IsLocked) return;

            _pauseEvents.Resume();
        }

        protected override void OnPointerEntered()
        {
        }

        protected override void OnPointerExited()
        {
        }
    }
}