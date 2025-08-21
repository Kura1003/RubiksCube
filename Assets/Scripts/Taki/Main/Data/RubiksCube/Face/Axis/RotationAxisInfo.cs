using System.Collections.Generic;
using UnityEngine;

namespace Taki.Main.Data.RubiksCube
{
    internal readonly struct RotationAxisInfo
    {
        public Vector3 Normal { get; }
        public List<Transform> RotationAxes { get; }

        internal RotationAxisInfo(
            Vector3 normal, 
            List<Transform> rotationAxes)
        {
            Normal = normal;
            RotationAxes = rotationAxes;
        }
    }
}