using System;
using Cysharp.Threading.Tasks;

namespace Taki.Main.System.RubiksCube
{
    internal interface ICubeActionHandler : IDisposable
    {
        UniTask Execute();
    }
}