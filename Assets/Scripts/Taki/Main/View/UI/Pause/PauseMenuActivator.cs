using Cysharp.Threading.Tasks;
using R3;
using Taki.Main.System;
using System.Threading;
using UnityEngine;
using VContainer;
using System.Collections.Generic;
using System.Linq;
using Taki.Audio;

namespace Taki.Main.View
{
    public sealed class PauseMenuActivator : MonoBehaviour, IPauseMenuActivator
    {
        [SerializeField] private DualCanvasGroupFader _dualCanvasGroupFader;
        [SerializeField] private List<TextTyper> _textTypers;

        [Inject] private readonly CircleTextRotator _circleTextRotator;
        [Inject] private readonly IPauseEvents _pauseEvents;

        public void Initialize()
        {
            _pauseEvents.OnPauseRequested
                .Subscribe(_ =>
                {
                    OnPauseRequested(destroyCancellationToken)
                        .SuppressCancellationThrow()
                        .Forget();
                })
                .AddTo(this);
        }

        private async UniTask OnPauseRequested(CancellationToken token)
        {
            Time.timeScale = 0f;
            AudioManager.Instance.CurrentBgmPlayer.FadeVolume(0.5f, 0.5f);

            var tasks = new List<UniTask>();

            var typerTasks = _textTypers
                .Select(typer => typer.Type(cancellationToken: token))
                .ToList();

            tasks.AddRange(typerTasks);
            tasks.Add(_dualCanvasGroupFader.FadeOut(token));

            await UniTask.WhenAll(tasks);

            _circleTextRotator.SetInputLock(false);
        }
    }
}