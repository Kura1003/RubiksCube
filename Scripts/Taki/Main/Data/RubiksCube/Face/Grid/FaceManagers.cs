
namespace Taki.Main.Data.RubiksCube
{
    internal readonly struct FaceManagers
    {
        private readonly FaceCoordinateRegistry _coordinateRegistry;
        private readonly FaceSwapper _swapper;
        private readonly FaceTransformManipulator _manipulator;

        internal FaceCoordinateRegistry CoordinateRegistry => _coordinateRegistry;
        internal FaceSwapper Swapper => _swapper;
        internal FaceTransformManipulator Manipulator => _manipulator;

        internal FaceManagers(
            PieceInfo[,] piecesInfo,
            int cubeSize)
        {
            _coordinateRegistry = new FaceCoordinateRegistry(piecesInfo, cubeSize);
            _swapper = new FaceSwapper(piecesInfo, cubeSize);
            _manipulator = new FaceTransformManipulator(piecesInfo, cubeSize);
        }
    }
}