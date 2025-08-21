using R3;

namespace Taki.Utility
{
    public interface IPointerEvents
    {
        ReactiveCommand<Unit> OnClicked { get; }
        ReactiveCommand<Unit> OnPointerEntered { get; }
        ReactiveCommand<Unit> OnPointerExited { get; }
    }
}