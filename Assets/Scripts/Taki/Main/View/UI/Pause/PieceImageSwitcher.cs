using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Taki.Main.View
{
    public sealed class PieceImageSwitcher : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _pieceImagePrefabs = new();
        [SerializeField] private RectTransform _parentTransform;

        [SerializeField] private float _fadeDuration = 0.5f;
        [SerializeField] private Ease _fadeEase = Ease.OutQuad;
        [SerializeField] private bool _ignoreTimeScale = false;

        private List<Image> _instantiatedImages = new();
        private int _currentIndex = 0;
        private bool _isTaskRunning = false;

        private void Awake()
        {
            InstantiateImages();
        }

        private void InstantiateImages()
        {
            for (int i = 0; i < _pieceImagePrefabs.Count; i++)
            {
                var imageObject = Instantiate(_pieceImagePrefabs[i], _parentTransform);
                var imageInstance = imageObject.GetComponent<Image>();
                _instantiatedImages.Add(imageInstance);

                imageInstance.color = new Color(1f, 1f, 1f, 0f);
            }

            _instantiatedImages[_currentIndex].color = Color.white;
        }

        public async UniTask SwitchToNext(CancellationToken token)
        {
            if (_isTaskRunning) return;

            int nextIndex = (_currentIndex + 1) % _instantiatedImages.Count;
            await FadeToImage(nextIndex, token);
        }

        public async UniTask SwitchToPrevious(CancellationToken token)
        {
            if (_isTaskRunning) return;

            int nextIndex = (_currentIndex - 1 + _instantiatedImages.Count) % _instantiatedImages.Count;
            await FadeToImage(nextIndex, token);
        }

        private async UniTask FadeToImage(int nextIndex, CancellationToken token)
        {
            _isTaskRunning = true;

            Image currentImage = _instantiatedImages[_currentIndex];
            Image nextImage = _instantiatedImages[nextIndex];

            await UniTask.WhenAll(
                currentImage.DOFade(0.0f, _fadeDuration)
                    .SetEase(_fadeEase)
                    .SetUpdate(_ignoreTimeScale)
                    .ToUniTask(cancellationToken: token),
                nextImage.DOFade(1.0f, _fadeDuration)
                    .SetEase(_fadeEase)
                    .SetUpdate(_ignoreTimeScale)
                    .ToUniTask(cancellationToken: token)
            );

            _currentIndex = nextIndex;
            _isTaskRunning = false;
        }
    }
}