namespace Taki.Main.Data.RubiksCube
{
    internal interface ICubeRotationLogic
    {
        void SetRotationBuffers(Face face, int layerIndex);
        void ClearRotationBuffers(int layerIndex);
        public void RotateSideLines(
            Face face,
            int layerIndex,
            bool isClockwise,
            bool shouldSwapTransforms);
        void RotateFaceSurface(int layerIndex, bool isClockwise);
    }
}