
namespace Taki.Main.Data.RubiksCube
{
    internal interface ICubeStateSaver
    {
        void SaveAllPiecePositions();
        void SaveAllPieceRotations();
        void SaveBufferedPiecePositions(int layerIndex);
        void SaveBufferedPieceRotations(int layerIndex);
    }
}