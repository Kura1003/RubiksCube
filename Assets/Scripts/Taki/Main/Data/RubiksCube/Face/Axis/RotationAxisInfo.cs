using System.Collections.Generic;
using UnityEngine;

namespace Taki.Main.Data.RubiksCube
{
    internal readonly struct RotationAxisInfo
    {
        public readonly Vector3 Normal;
        public readonly List<Transform> RotationAxes;

        public RotationAxisInfo(
            Vector3 normal, 
            List<Transform> rotationAxes)
        {
            Normal = normal;
            RotationAxes = rotationAxes;
        }
    }
}