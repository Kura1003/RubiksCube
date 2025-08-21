using UnityEngine;
using Taki.Utility.Core;

namespace Taki.Main.Data.RubiksCube
{
    internal class FaceTransformManipulator
    {
        private readonly PieceInfo[,] PiecesInfo;
        private readonly int _cachedSize;

        internal FaceTransformManipulator(
            PieceInfo[,] piecesInfo,
            int cubeSize)
        {
            Thrower.IfNull(piecesInfo, nameof(piecesInfo));

            PiecesInfo = piecesInfo;
            _cachedSize = cubeSize;
        }

        internal void ParentLine(RotationLineInfo lineInfo, Transform parent)
        {
            for (int i = 0; i < _cachedSize; i++)
            {
                Vector2Int index = lineInfo.GetIndex(i);
                PiecesInfo[index.x, index.y].Parent(parent);
            }
        }

        internal void UnparentAll(Transform parent)
        {
            for (int x = 0; x < _cachedSize; x++)
            {
                for (int y = 0; y < _cachedSize; y++)
                {
                    PiecesInfo[x, y].Parent(parent);
                }
            }
        }

        internal void RotateLine(
            RotationLineInfo lineInfo,
            float angle,
            Vector3 worldAxis)
        {
            for (int i = 0; i < _cachedSize; i++)
            {
                Vector2Int index = lineInfo.GetIndex(i);
                PiecesInfo[index.x, index.y].Rotate(angle, worldAxis);
            }
        }

        internal void RotateAll(float angle, Vector3 localAxis)
        {
            for (int x = 0; x < _cachedSize; x++)
            {
                for (int y = 0; y < _cachedSize; y++)
                {
                    PiecesInfo[x, y].RotateLocal(angle, localAxis);
                }
            }
        }
    }
}