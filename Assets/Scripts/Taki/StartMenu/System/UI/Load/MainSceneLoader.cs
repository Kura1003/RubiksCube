using Cysharp.Threading.Tasks;
using System.Threading;
using Taki.Main.View;
using UnityEngine.SceneManagement;
using VContainer;
using UnityEngine;

namespace Taki.StartMenu.System
{
    internal sealed class MainSceneLoader : IMainSceneLoader
    {
        private readonly IScreenFader _screenFader;
        private readonly string _sceneName = "Main";

        private AsyncOperation _loadAsync;

        public bool IsPreloaded => _loadAsync != null;

        [Inject]
        internal MainSceneLoader(IScreenFader screenFader)
        {
            _screenFader = screenFader;
        }

        public void PreloadMainScene()
        {
            if (_loadAsync == null)
            {
                _loadAsync = SceneManager.LoadSceneAsync(_sceneName);
                _loadAsync.allowSceneActivation = false;
            }
        }

        public async UniTask LoadMainScene(CancellationToken token)
        {
            if (_loadAsync == null) return;

            _screenFader.SetFadeDuration(0.5f);
            _screenFader.SetFadeInTargetAlpha(1f);

            await _screenFader.FadeIn(token, true);
            _loadAsync.allowSceneActivation = true;
        }

        public void Dispose()
        {
            SceneManager.UnloadSceneAsync(_sceneName);
            _loadAsync = null;
        }
    }
}
