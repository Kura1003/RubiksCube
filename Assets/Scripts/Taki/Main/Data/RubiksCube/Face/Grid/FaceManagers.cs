
namespace Taki.Main.Data.RubiksCube
{
    internal readonly struct FaceManagers
    {
        private readonly FaceCoordinatesManager _coordinatesManager;
        private readonly FaceSwapper _swapper;
        private readonly FaceTransformManipulator _manipulator;

        public FaceCoordinatesManager CoordinatesManager => _coordinatesManager;
        public FaceSwapper Swapper => _swapper;
        public FaceTransformManipulator Manipulator => _manipulator;

        internal FaceManagers(
            PieceInfo[,] piecesInfo,
            int cubeSize)
        {
            _coordinatesManager = new FaceCoordinatesManager(piecesInfo, cubeSize);
            _swapper = new FaceSwapper(piecesInfo, cubeSize);
            _manipulator = new FaceTransformManipulator(piecesInfo, cubeSize);
        }
    }
}