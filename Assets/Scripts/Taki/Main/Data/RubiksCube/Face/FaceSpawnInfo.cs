using UnityEngine;
using Taki.Utility;

namespace Taki.Main.Data.RubiksCube
{
    internal readonly struct FaceSpawnInfo
    {
        public GridPlane Plane { get; }
        public Vector3 Normal { get; }
        public Vector3 RotationOffset { get; }

        public FaceSpawnInfo(
            GridPlane plane,
            Vector3 normal,
            Vector3 rotationOffset)
        {
            Plane = plane;
            Normal = normal;
            RotationOffset = rotationOffset;
        }
    }
}