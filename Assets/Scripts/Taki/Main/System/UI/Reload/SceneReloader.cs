using AnnulusGames.LucidTools.Audio;
using Cysharp.Threading.Tasks;
using System.Threading;
using Taki.Main.View;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Taki.Main.System
{
    internal sealed class SceneReloader : ISceneReloader
    {
        private readonly IScreenFader _screenFader;
        private readonly string _sceneName;

        private AsyncOperation _reloadAsync;

        [Inject]
        public SceneReloader(IScreenFader screenFader)
        {
            _screenFader = screenFader;
            _sceneName = SceneManager.GetActiveScene().name;
        }

        public void PreloadScene()
        {
            if (_reloadAsync == null)
            {
                _reloadAsync = SceneManager.LoadSceneAsync(_sceneName);
                _reloadAsync.allowSceneActivation = false;
            }
        }

        public async UniTask ReloadScene(CancellationToken token)
        {
            if (_reloadAsync == null) return;

            _screenFader.SetFadeDuration(0.5f);
            _screenFader.SetFadeInTargetAlpha(1f);

            await _screenFader.FadeIn(token, true);
            _reloadAsync.allowSceneActivation = true;
        }

        public void Dispose()
        {
            SceneManager.UnloadSceneAsync(_sceneName);
            _reloadAsync = null;
        }
    }
}