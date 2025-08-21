using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

namespace Taki.Main.View
{
    public sealed class GUIScreenFader : MonoBehaviour, IScreenFader
    {
        private GUITextureFader _fader;

        public void Initialize(Texture2D texture, Rect positionRect)
        {
            _fader = new GUITextureFader(texture, positionRect);
        }

        public void SetTextureColor(Color color)
        {
            _fader.SetTextureColor(color);
        }

        public void SetFadeDuration(float duration)
        {
            _fader.SetFadeDuration(duration);
        }

        public void SetFadeInTargetAlpha(float alpha)
        {
            _fader.SetFadeInTargetAlpha(alpha);
        }

        public UniTask FadeIn(CancellationToken token, bool keepDrawingAfterFade = false)
        {
            return _fader.FadeIn(token, keepDrawingAfterFade);
        }

        public UniTask FadeOut(CancellationToken token)
        {
            return _fader.FadeOut(token);
        }

        public void SetAlphaToZero()
        {
            _fader.SetAlphaToZero();
        }

        private void OnGUI()
        {
            _fader?.Draw();
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }
    }
}