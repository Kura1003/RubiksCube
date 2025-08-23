using Cysharp.Threading.Tasks;
using System.Threading;
using Taki.Main.View;
using Taki.Utility.Core;
using UnityEngine;
using VContainer.Unity;

namespace Taki.StartMenu.System
{
    internal class StartMenuEntrypoint : IInitializable, ITickable
    {
        private readonly TextTyper _titleTextTyper;

        private readonly IScreenFader _screenFader;
        private readonly IMainSceneLoader _mainSceneLoader;

        private readonly CancellationToken _destroyCancellationToken;

        public StartMenuEntrypoint(
            LifetimeScope lifetimeScope,
            TextTyper titleTextTyper,
            IScreenFader screenFader,
            IMainSceneLoader mainSceneLoader)
        {
            _titleTextTyper = titleTextTyper;

            _screenFader = screenFader;
            _mainSceneLoader = mainSceneLoader;

            _destroyCancellationToken = lifetimeScope.destroyCancellationToken;
        }

        public async void Initialize()
        {
            await UniTask.Yield();
            var isCancelled = await _screenFader
                .FadeOut(
                _destroyCancellationToken)
                .SuppressCancellationThrow();

            if (!isCancelled)
            {
                await _titleTextTyper
                    .Type(cancellationToken: _destroyCancellationToken)
                    .SuppressCancellationThrow();

                _mainSceneLoader.PreloadMainScene();
            }
        }

        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                App.Exit();
            }
        }
    }
}
