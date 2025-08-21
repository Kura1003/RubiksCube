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
                Debug.Log("�Q�[���͊��Ƀ|�[�Y��Ԃł��B");
                return;
            }

            _isPaused = true;
            OnPauseRequested.OnNext(Unit.Default);
            Debug.Log("�Q�[�����|�[�Y���܂����B");
        }

        public void Resume()
        {
            if (!_isPaused)
            {
                Debug.Log("�Q�[���͊��ɍĊJ����Ă��܂��B");
                return;
            }

            _isPaused = false;
            OnResumeRequested.OnNext(Unit.Default);
            Debug.Log("�Q�[�����ĊJ���܂����B");
        }

        public void Dispose()
        {
            OnPauseRequested?.Dispose();
            OnResumeRequested?.Dispose();
        }
    }
}