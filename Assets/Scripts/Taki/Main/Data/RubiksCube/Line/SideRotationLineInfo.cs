
namespace Taki.Main.Data.RubiksCube
{
    internal readonly struct SideRotationLineInfo
    {
        public Face LineFace { get; }

        public Line LineType { get; }

        public bool IsReversed { get; }

        public SideRotationLineInfo(
            Face lineFace,
            Line lineType,
            bool isReversed)
        {
            LineFace = lineFace;
            LineType = lineType;
            IsReversed = isReversed;
        }

        public RotationLineInfo GetLineInfo(
            int layerIndex,
            int size)
        {
            int index = 0;
            if (IsReversed)
            {
                index = (size - 1 - layerIndex) % size;
            }
            else
            {
                index = layerIndex % size;
            }

            return new RotationLineInfo(LineFace, LineType, index);
        }
    }
}