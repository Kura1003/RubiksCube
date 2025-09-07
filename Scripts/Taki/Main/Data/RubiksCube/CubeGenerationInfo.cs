using System.Collections.Generic;

namespace Taki.Main.Data.RubiksCube
{
    public readonly struct CubeGenerationInfo
    {
        internal Dictionary<Face, FaceManagers> FaceManagersMap { get; }
        internal Dictionary<Face, RotationAxisInfo> AxisInfoMap { get; }

        internal CubeGenerationInfo(
            Dictionary<Face, FaceManagers> faceManagersMap, 
            Dictionary<Face, RotationAxisInfo> axisInfoMap)
        {
            FaceManagersMap = faceManagersMap;
            AxisInfoMap = axisInfoMap;
        }
    }
}