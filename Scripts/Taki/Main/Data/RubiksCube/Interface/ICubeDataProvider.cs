using System.Collections.Generic;

namespace Taki.Main.Data.RubiksCube
{
    internal interface ICubeDataProvider
    {
        void Setup(Dictionary<Face, FaceManagers> faceManagersMap, int cubeSize);
        FaceManagers GetFaceManagers(Face face);
    }
}