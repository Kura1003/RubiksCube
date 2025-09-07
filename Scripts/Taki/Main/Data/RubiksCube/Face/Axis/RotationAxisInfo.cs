using System.Collections.Generic;
using UnityEngine;

namespace Taki.Main.Data.RubiksCube
{
    internal readonly struct RotationAxisInfo
    {
        internal Vector3 Normal { get; }
        internal List<Transform> RotationAxes { get; }

        internal RotationAxisInfo(
            Vector3 normal, 
            List<Transform> rotationAxes)
        {
            Normal = normal;
            RotationAxes = rotationAxes;
        }
    }
}