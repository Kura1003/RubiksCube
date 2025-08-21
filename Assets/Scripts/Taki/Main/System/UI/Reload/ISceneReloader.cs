using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace Taki.Main.System
{
    internal interface ISceneReloader : IDisposable
    {
        UniTask ReloadScene(CancellationToken token);
        void PreloadScene();
    }
}