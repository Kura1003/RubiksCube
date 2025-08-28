using Cysharp.Threading.Tasks;
using System;

namespace Taki.Main.System.RubiksCube
{
    internal interface ICubeActionHandler : IDisposable
    {
        UniTask Execute();
    }
}