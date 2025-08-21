using System;

namespace Taki.Main.System.RubiksCube
{
    internal interface ICubeSizeManager : IDisposable
    {
        bool HasCubeSizeChanged { get; }
        float CalculateCameraYPositionFactor();
        void SetPreviousCubeSize();
    }
}