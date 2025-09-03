using UnityEngine;

namespace Taki.Main.Data.RubiksCube
{
    internal readonly struct PieceInfo
    {
        internal Transform Transform { get; }
        internal string Id { get; }

        internal PieceInfo(Transform transform, string id)
        {
            Transform = transform;
            Id = id;
        }
    }
}