
namespace Taki.Main.Data.RubiksCube
{
    internal interface ICubeStateRestorer
    {
        void RestoreAllPiecePositions();
        void RestoreAllPieceRotations();
        void RestoreBufferedPiecePositions(int layerIndex);
        void RestoreBufferedPieceRotations(int layerIndex);
    }
}