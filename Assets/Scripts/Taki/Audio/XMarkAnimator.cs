using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Taki.Audio
{
    public sealed class XMarkAnimator : MonoBehaviour
    {
        [SerializeField] private Color _xMarkColor = Color.red;
        [SerializeField] private float _lineThicknessRatio = 0.1f;
        [SerializeField] private RectTransform _centerRectTransform;

        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private Ease _animationEase = Ease.OutQuad;
        [SerializeField] private Vector2 _targetScale = new Vector2(1.0f, 1.0f);

        [SerializeField] private Image _line1Image;
        [SerializeField] private Image _line2Image;

        private bool _isAnimating = false;
        private Texture2D _xMarkTexture;
        private Sequence _currentAnimationSequence;

        private void Awake()
        {
            _xMarkTexture = CreateColorTexture(_xMarkColor);
            _line1Image.sprite = 
                Sprite.Create(
                    _xMarkTexture, 
                    new Rect(
                        0, 
                        0, 
                        _xMarkTexture.width, 
                        _xMarkTexture.height), 
                    Vector2.one * 0.5f);

            _line2Image.sprite = 
                Sprite.Create(
                    _xMarkTexture, 
                    new Rect(
                        0, 
                        0, 
                        _xMarkTexture.width, 
                        _xMarkTexture.height), 
                    Vector2.one * 0.5f);

            _line1Image.transform.localScale = Vector3.zero;
            _line1Image.color = _xMarkColor;

            _line2Image.transform.localScale = Vector3.zero;
            _line2Image.color = _xMarkColor;

            _line1Image.raycastTarget = false;
            _line2Image.raycastTarget = false;
        }

        private Texture2D CreateColorTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        [ContextMenu("Play X Mark Animation")]
        public void PlayXMarkAnimation()
        {
            if (_isAnimating)
            {
                return;
            }

            CancelAnimation();

            _isAnimating = true;

            _line1Image.rectTransform.anchoredPosition = Vector2.zero;
            _line2Image.rectTransform.anchoredPosition = Vector2.zero;

            float lineHeight = _centerRectTransform.rect.height;
            float lineWidth = lineHeight * _lineThicknessRatio;
            _line1Image.rectTransform.sizeDelta = new Vector2(lineWidth, lineHeight);
            _line2Image.rectTransform.sizeDelta = new Vector2(lineWidth, lineHeight);

            _line1Image.transform.localRotation = Quaternion.Euler(0, 0, 45);
            _line2Image.transform.localRotation = Quaternion.Euler(0, 0, -45);

            _currentAnimationSequence = DOTween.Sequence();

            _currentAnimationSequence.Join(_line1Image.transform.DOScale(_targetScale, _animationDuration));
            _currentAnimationSequence.Join(_line2Image.transform.DOScale(_targetScale, _animationDuration));

            _currentAnimationSequence
                .SetEase(_animationEase)
                .SetUpdate(true)
                .OnComplete(() => _isAnimating = false);
        }

        [ContextMenu("Reset X Mark")]
        public void ResetXMark()
        {
            CancelAnimation();

            _line1Image.transform.localScale = Vector3.zero;
            _line2Image.transform.localScale = Vector3.zero;
            _isAnimating = false;
        }

        private void CancelAnimation()
        {
            if (_currentAnimationSequence != null && _currentAnimationSequence.IsActive())
            {
                _currentAnimationSequence.Kill();
                _currentAnimationSequence = null;
            }
        }

        private void OnDestroy()
        {
            ResetXMark();
        }
    }
}