using R3;
using System;

namespace Taki.Main.System
{
    internal interface IPauseEvents : IDisposable
    {
        Subject<Unit> OnPauseRequested { get; }
        Subject<Unit> OnResumeRequested { get; }

        bool IsPaused { get; }

        void Pause();
        void Resume();
    }
}