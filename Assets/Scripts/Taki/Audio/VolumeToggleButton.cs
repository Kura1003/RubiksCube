using Taki.Utility;
using UnityEngine;

namespace Taki.Audio
{
    public sealed class VolumeToggleButton : PointerEventHandler
    {
        [SerializeField] private XMarkAnimator _xMarkAnimator;

        protected override void OnClicked()
        {
            var audioManager = AudioManager.Instance;
            audioManager.ToggleMasterVolume();

            if (audioManager.IsVolumeMuted)
            {
                OnMuted();
            }
            else
            {
                OnUnmuted();
            }
        }

        protected override void OnPointerEntered()
        {

        }

        protected override void OnPointerExited()
        {

        }

        private void OnMuted()
        {
            _xMarkAnimator.PlayXMarkAnimation();
        }

        private void OnUnmuted()
        {
            _xMarkAnimator.ResetXMark();
        }
    }
}