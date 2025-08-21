using System.Collections.Generic;
using UnityEngine;
using Taki.Utility.Core;

namespace Taki.Main.Data.RubiksCube
{
    internal class RotationAxisManager : IRotationAxisProvider
    {
        private Transform _parentTransform;
        private Dictionary<Face, RotationAxisInfo> _axisInfoMap = new();
        private readonly Dictionary<Face, Vector3> _cachedFaceNormals = new();

        internal RotationAxisManager(
            Dictionary<Face, RotationAxisInfo> axisInfoMap,
            Transform parentTransform)
        {
            Thrower.IfNull(axisInfoMap, nameof(axisInfoMap));
            Thrower.IfNull(parentTransform, nameof(parentTransform));

            SetUp(axisInfoMap, parentTransform);
        }

        public void SetUp(
            Dictionary<Face, RotationAxisInfo> axisInfoMap,
            Transform parentTransform)
        {
            _axisInfoMap = axisInfoMap;
            _parentTransform = parentTransform;

            _cachedFaceNormals.Clear();
            foreach (var pair in _axisInfoMap)
            {
                var face = pair.Key;
                var normal = pair.Value.Normal;
                _cachedFaceNormals[face] = _parentTransform.TransformDirection(normal);
            }
        }

        public Vector3 GetFaceNormal(Face face) => _cachedFaceNormals[face];

        public Transform GetRotationAxis(Face face, int layerIndex)
        {
            var axes = _axisInfoMap[face].RotationAxes;
            return axes[layerIndex];
        }

        public Transform GetCenterTransform() => _parentTransform;
    }
}