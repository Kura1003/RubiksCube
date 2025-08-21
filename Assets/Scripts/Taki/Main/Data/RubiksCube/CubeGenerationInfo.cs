using System.Collections.Generic;

namespace Taki.Main.Data.RubiksCube
{
    public readonly struct CubeGenerationInfo
    {
        internal readonly Dictionary<Face, FaceManagers> FaceManagersMap;
        internal readonly Dictionary<Face, RotationAxisInfo> AxisInfoMap;

        internal CubeGenerationInfo(
            Dictionary<Face, FaceManagers> faceManagersMap, 
            Dictionary<Face, RotationAxisInfo> axisInfoMap)
        {
            FaceManagersMap = faceManagersMap;
            AxisInfoMap = axisInfoMap;
        }
    }
}