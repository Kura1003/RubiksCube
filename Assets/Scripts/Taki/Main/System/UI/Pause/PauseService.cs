using R3;
using UnityEngine;

namespace Taki.Main.System
{
    public sealed class PauseService : IPauseEvents
    {
        public Subject<Unit> OnPauseRequested { get; } = new();
        public Subject<Unit> OnResumeRequested { get; } = new();

        private bool _isPaused = false;
        public bool IsPaused => _isPaused;

        public void Pause()
        {
            if (_isPaused)
            {
                Debug.Log("ゲームは既にポーズ状態です。");
                return;
            }

            _isPaused = true;
            OnPauseRequested.OnNext(Unit.Default);
            Debug.Log("ゲームをポーズしました。");
        }

        public void Resume()
        {
            if (!_isPaused)
            {
                Debug.Log("ゲームは既に再開されています。");
                return;
            }

            _isPaused = false;
            OnResumeRequested.OnNext(Unit.Default);
            Debug.Log("ゲームを再開しました。");
        }

        public void Dispose()
        {
            OnPauseRequested?.Dispose();
            OnResumeRequested?.Dispose();
        }
    }
}