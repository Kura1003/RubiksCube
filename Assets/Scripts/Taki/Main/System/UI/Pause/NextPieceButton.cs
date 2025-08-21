using Cysharp.Threading.Tasks;
using Taki.Main.View;
using Taki.Utility;
using UnityEngine;

namespace Taki.Main.System
{
    public sealed class NextPieceButton : PointerEventHandler
    {
        [SerializeField] private PieceImageSwitcher _pieceImageSwitcher;

        protected override void OnClicked()
        {
            _pieceImageSwitcher
                .SwitchToNext(destroyCancellationToken)
                .SuppressCancellationThrow()
                .Forget();
        }

        protected override void OnPointerEntered()
        {
        }

        protected override void OnPointerExited()
        {
        }
    }
}