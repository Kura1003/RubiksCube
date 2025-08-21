
namespace Taki.Main.Data.RubiksCube
{
    internal readonly struct RotationLayerInfo
    {
        public bool IsFrontLayer { get; }
        public bool IsOppositeLayer { get; }
        public bool IsMiddleLayer { get; }

        public RotationLayerInfo(int layerIndex, int size)
        {
            IsFrontLayer = layerIndex == 0;
            IsOppositeLayer = layerIndex == size - 1;
            IsMiddleLayer = !IsFrontLayer && !IsOppositeLayer;
        }
    }
}