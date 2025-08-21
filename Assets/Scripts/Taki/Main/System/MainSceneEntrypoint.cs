using AnnulusGames.LucidTools.Audio;
using Cysharp.Threading.Tasks;
using System.Threading;
using Taki.Audio;
using Taki.Main.System.RubiksCube;
using Taki.Main.View;
using Taki.Utility;
using Taki.Utility.Core;
using UnityEngine;
using VContainer.Unity;

namespace Taki.Main.System
{
    internal class MainSceneEntrypoint : IInitializable, ITickable
    {
        private AudioManager _audioManager;

        private readonly CancellationToken _destroyCancellationToken;

        private readonly ICubeInteractionHandler _cubeInteractionHandler;
        private readonly IScreenFader _screenFader;
        private readonly ISceneReloader _sceneReloader;
        private readonly IPauseMenuActivator _pauseMenuActivator;
        private readonly IPauseMenuDeactivator _pauseMenuDeactivator;

        private readonly ButtonEntrypoint _buttonEntrypoint;
        private readonly UserInputLockEntrypoint _lockEntrypoint;

        public MainSceneEntrypoint(
            LifetimeScope lifetimeScope,
            ICubeInteractionHandler cubeInteractionHandler,
            IScreenFader screenFader,
            ISceneReloader sceneReloader,
            IPauseMenuActivator pauseMenuActivator,
            IPauseMenuDeactivator pauseMenuDeactivator,
            ButtonEntrypoint buttonEntrypoint,
            UserInputLockEntrypoint lockEntrypoint)
        {
            _destroyCancellationToken = lifetimeScope.destroyCancellationToken;

            _cubeInteractionHandler = cubeInteractionHandler;
            _screenFader = screenFader;
            _sceneReloader = sceneReloader;
            _pauseMenuActivator = pauseMenuActivator;
            _pauseMenuDeactivator = pauseMenuDeactivator;

            _buttonEntrypoint = buttonEntrypoint;
            _lockEntrypoint = lockEntrypoint;
        }

        public async void Initialize()
        {
            await SetupSystem().SuppressCancellationThrow();
            var isCancelled = await _screenFader
                .FadeOut(
                _destroyCancellationToken)
                .SuppressCancellationThrow();

            if (!isCancelled)
            {
                OnFadeOutComplete();
            }
        }

        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                App.Exit();
            }
        }

        private async UniTask SetupSystem()
        {
            await UniTask.Yield(cancellationToken: _destroyCancellationToken);

            LucidAudio.BGMVolume = 0f;
            LucidAudio.SEVolume = 0f;
            _audioManager = AudioManager.Instance;

            await PreloadCube();

            _sceneReloader.PreloadScene();
            _buttonEntrypoint.CollectHandlers();
            _lockEntrypoint.CollectUserInputLocks();
            _lockEntrypoint.SetAllUserInputLocks(true);

            await UniTask.Yield(cancellationToken: _destroyCancellationToken);

        }

        private async UniTask PreloadCube()
        {
            await _cubeInteractionHandler.ExecuteFastShuffle();
            await _cubeInteractionHandler.ExecuteRebuild();
        }

        private async void OnFadeOutComplete()
        {
            await UniTask.WaitForSeconds(
                1.0f,
                cancellationToken: _destroyCancellationToken,
                ignoreTimeScale: true);

            LucidAudio.BGMVolume = 1f;
            LucidAudio.SEVolume = 1f;
            _audioManager.CurrentBgmPlayer = _audioManager
                .Play("Air_On_The_G_String-MB02-3", _buttonEntrypoint.gameObject);

            _audioManager
                .CurrentBgmPlayer
                .SetLoop(true)
                .SetVolume(0f)
                .FadeVolume(1f, 0.5f);

            _pauseMenuActivator.Initialize();
            _pauseMenuDeactivator.Initialize();
            _buttonEntrypoint.InitializeHandlers();
            _lockEntrypoint.SetUserInputLockByTag(UserInputLockTag.Normal, false);
            _cubeInteractionHandler.RegisterEvents();
        }
    }
}