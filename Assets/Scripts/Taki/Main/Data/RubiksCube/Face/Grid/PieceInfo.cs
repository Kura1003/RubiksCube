using UnityEngine;

namespace Taki.Main.Data.RubiksCube
{
    internal readonly struct PieceInfo
    {
        public Transform Transform { get; }
        public string Id { get; }

        public PieceInfo(Transform transform, string id)
        {
            Transform = transform;
            Id = id;
        }
    }
}