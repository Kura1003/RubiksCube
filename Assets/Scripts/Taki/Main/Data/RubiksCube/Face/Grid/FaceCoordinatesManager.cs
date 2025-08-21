using UnityEngine;
using Taki.Utility.Core;

namespace Taki.Main.Data.RubiksCube
{
    internal class FaceCoordinatesManager
    {
        private readonly PieceInfo[,] PiecesInfo;
        private readonly int _cachedSize;

        private readonly Vector3[,] _localPositions;
        private readonly Quaternion[,] _localRotations;

        internal FaceCoordinatesManager(
            PieceInfo[,] piecesInfo,
            int cubeSize)
        {
            Thrower.IfNull(piecesInfo, nameof(piecesInfo));

            PiecesInfo = piecesInfo;
            _cachedSize = cubeSize;

            _localPositions = new Vector3[_cachedSize, _cachedSize];
            _localRotations = new Quaternion[_cachedSize, _cachedSize];

            Initialize();
        }

        private void Initialize()
        {
            for (int x = 0; x < _cachedSize; x++)
            {
                for (int y = 0; y < _cachedSize; y++)
                {
                    _localPositions[x, y] = PiecesInfo[x, y].Transform.localPosition;
                    _localRotations[x, y] = PiecesInfo[x, y].Transform.localRotation;
                }
            }
        }

        public void SavePositions(RotationLineInfo lineInfo)
        {
            for (int i = 0; i < _cachedSize; i++)
            {
                Vector2Int index = lineInfo.GetIndex(i);
                _localPositions[index.x, index.y] 
                    = PiecesInfo[index.x, index.y].Transform.localPosition;
            }
        }

        public void SaveRotations(RotationLineInfo lineInfo)
        {
            for (int i = 0; i < _cachedSize; i++)
            {
                Vector2Int index = lineInfo.GetIndex(i);
                _localRotations[index.x, index.y] 
                    = PiecesInfo[index.x, index.y].Transform.localRotation;
            }
        }

        public void SaveAllPositions()
        {
            for (int x = 0; x < _cachedSize; x++)
            {
                for (int y = 0; y < _cachedSize; y++)
                {
                    _localPositions[x, y] = PiecesInfo[x, y].Transform.localPosition;
                }
            }
        }

        public void SaveAllRotations()
        {
            for (int x = 0; x < _cachedSize; x++)
            {
                for (int y = 0; y < _cachedSize; y++)
                {
                    _localRotations[x, y] = PiecesInfo[x, y].Transform.localRotation;
                }
            }
        }

        public void RestorePositions(RotationLineInfo lineInfo)
        {
            for (int i = 0; i < _cachedSize; i++)
            {
                Vector2Int index = lineInfo.GetIndex(i);
                PiecesInfo[index.x, index.y].Transform.localPosition 
                    = _localPositions[index.x, index.y];
            }
        }

        public void RestoreRotations(RotationLineInfo lineInfo)
        {
            for (int i = 0; i < _cachedSize; i++)
            {
                Vector2Int index = lineInfo.GetIndex(i);
                PiecesInfo[index.x, index.y].Transform.localRotation 
                    = _localRotations[index.x, index.y];
            }
        }

        public void RestoreAllPositions()
        {
            for (int x = 0; x < _cachedSize; x++)
            {
                for (int y = 0; y < _cachedSize; y++)
                {
                    PiecesInfo[x, y].Transform.localPosition = _localPositions[x, y];
                }
            }
        }

        public void RestoreAllRotations()
        {
            for (int x = 0; x < _cachedSize; x++)
            {
                for (int y = 0; y < _cachedSize; y++)
                {
                    PiecesInfo[x, y].Transform.localRotation = _localRotations[x, y];
                }
            }
        }
    }
}