using UnityEngine;
using System.Collections.Generic;

namespace Taki.Main.Data.RubiksCube
{
    internal interface IRotationAxisProvider
    {
        Vector3 GetFaceNormal(Face face);
        Transform GetRotationAxis(Face face, int layerIndex);
        Transform GetCenterTransform();
        void SetUp(
            Dictionary<Face, RotationAxisInfo> axisInfoMap,
            Transform parentTransform);
    }
}