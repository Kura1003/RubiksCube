using UnityEngine;
using Taki.Utility;

namespace Taki.Main.Data.RubiksCube
{
    internal readonly struct FaceSpawnInfo
    {
        internal GridPlane Plane { get; }
        internal Vector3 Normal { get; }
        internal Vector3 RotationOffset { get; }

        internal FaceSpawnInfo(
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