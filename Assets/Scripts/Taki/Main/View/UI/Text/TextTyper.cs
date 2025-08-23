using Cysharp.Threading.Tasks;
using System.Text;
using System.Threading;
using TMPro;
using Taki.Audio;
using UnityEngine;

namespace Taki.Main.View
{
    public class TextTyper : MonoBehaviour
    {
        [SerializeField] private TMP_Text _targetText;
        [SerializeField] private float _characterInterval = 0.05f;
        [SerializeField] private bool _ignoreTimeScale = false;
        [SerializeField] private bool _playAudio = true;
        [SerializeField, TextArea(1, 3)]
        private string _textToType;

        private StringBuilder _stringBuilder = new StringBuilder();
        private string _fullTextCache;

        private const string _warmUpCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private void Awake()
        {
            if (_targetText != null)
            {
                WarmUpTextMeshPro(destroyCancellationToken).Forget();
                _targetText.text = string.Empty;
            }
        }

        private async UniTask WarmUpTextMeshPro(CancellationToken token)
        {
            Color originalColor = _targetText.color;
            _targetText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

            _targetText.text = _warmUpCharacters;
            _targetText.ForceMeshUpdate(true, true);

            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, token);

            _targetText.color = originalColor;
            _targetText.text = string.Empty;
            _targetText.ForceMeshUpdate(true, true);
        }

        public async UniTask Type(
            string overrideText = null,
            float? interval = null,
            CancellationToken cancellationToken = default)
        {
            string text = overrideText ?? _textToType;
            float delay = interval ?? _characterInterval;

            _stringBuilder.Clear();
            _stringBuilder.Append(text);
            _fullTextCache = _stringBuilder.ToString();

            _targetText.text = _fullTextCache;
            _targetText.maxVisibleCharacters = 0;

            for (int i = 0; i < text.Length; i++)
            {
                _targetText.maxVisibleCharacters = i + 1;
                await UniTask
                    .WaitForSeconds(
                    delay,
                    _ignoreTimeScale,
                    cancellationToken: cancellationToken);

                if (_playAudio)
                {
                    AudioManager.Instance.Play("TextType", gameObject).SetVolume(1f);
                }

                if (cancellationToken.IsCancellationRequested) return;
            }
        }

        public async UniTask Clear(
            float? interval = null,
            CancellationToken cancellationToken = default)
        {
            float delay = interval ?? _characterInterval;

            int currentVisibleCharacters = _targetText.maxVisibleCharacters;

            while (currentVisibleCharacters > 0)
            {
                _targetText.maxVisibleCharacters = --currentVisibleCharacters;
                await UniTask
                    .WaitForSeconds(
                    delay,
                    _ignoreTimeScale,
                    cancellationToken: cancellationToken);

                if (_playAudio)
                {
                    AudioManager.Instance.Play("TextType", gameObject).SetVolume(1f);
                }

                if (cancellationToken.IsCancellationRequested) return;
            }
            _targetText.text = string.Empty;
        }

        public void ClearImmediately()
        {
            _targetText.text = string.Empty;
            _targetText.maxVisibleCharacters = 0;
        }
    }
}
