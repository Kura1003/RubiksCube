using UnityEngine;
using Taki.Utility.Core;

namespace Taki.Main.Data.RubiksCube
{
    internal class FaceSwapper
    {
        private readonly PieceInfo[,] _piecesInfo;
        private readonly int _cachedSize;

        internal FaceSwapper(
            PieceInfo[,] piecesInfo,
            int cubeSize)
        {
            Thrower.IfNull(piecesInfo, nameof(piecesInfo));

            _piecesInfo = piecesInfo;
            _cachedSize = cubeSize;
        }

        public void SwapPositions(RotationLineInfo lineInfo, Transform[] otherTransforms)
        {
            for (int i = 0; i < _cachedSize; i++)
            {
                Vector2Int index = lineInfo.GetIndex(i);
                _piecesInfo[index.x, index.y].SwapLocalPosition(otherTransforms[i]);
            }
        }

        public void ReplacePieces(RotationLineInfo lineInfo, PieceInfo[] otherPieces)
        {
            for (int i = 0; i < _cachedSize; i++)
            {
                Vector2Int index = lineInfo.GetIndex(i);
                _piecesInfo[index.x, index.y].ReplaceInfo(otherPieces[i]);
            }
        }

        public void ReplaceAll(PieceInfo[,] otherPieces)
        {
            for (int row = 0; row < _cachedSize; row++)
            {
                for (int col = 0; col < _cachedSize; col++)
                {
                    _piecesInfo[row, col].ReplaceInfo(otherPieces[row, col]);
                }
            }
        }

        public void Rotate(bool isClockwise)
        {
            var tempMatrix = new PieceInfo[_cachedSize, _cachedSize];

            if (isClockwise)
            {
                for (int row = 0; row < _cachedSize; row++)
                {
                    for (int col = 0; col < _cachedSize; col++)
                    {
                        tempMatrix[col, _cachedSize - 1 - row] = _piecesInfo[row, col];
                    }
                }
            }

            else
            {
                for (int row = 0; row < _cachedSize; row++)
                {
                    for (int col = 0; col < _cachedSize; col++)
                    {
                        tempMatrix[_cachedSize - 1 - col, row] = _piecesInfo[row, col];
                    }
                }
            }

            ReplaceAll(tempMatrix);
        }

        public PieceInfo[] GetLinePieces(RotationLineInfo lineInfo)
        {
            var linePieces = new PieceInfo[_cachedSize];
            for (int i = 0; i < _cachedSize; i++)
            {
                Vector2Int index = lineInfo.GetIndex(i);
                linePieces[i] = _piecesInfo[index.x, index.y];
            }
            return linePieces;
        }
    }
}