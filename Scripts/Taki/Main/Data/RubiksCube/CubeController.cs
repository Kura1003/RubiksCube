using System;
using System.Collections.Generic;
using Taki.Utility;
using Taki.Utility.Core;
using UnityEngine;

namespace Taki.Main.Data.RubiksCube
{
    internal class CubeController :
        ICubeDataProvider,
        ICubeRotationLogic,
        ICubeTransformManipulator,
        ICubeStateSaver,
        ICubeStateRestorer
    {
        private const int SIDE_LINE_COUNT = 4;

        private Dictionary<Face, FaceManagers> _faceManagersMap;
        private int _cachedSize;

        private readonly Dictionary<int, RotationBuffers> _rotationBuffersMap = new();

        private readonly struct RotationBuffers
        {
            internal RotationLineInfo[] RotationLineInfo { get; }
            internal Face RotationFace { get; }
            internal RotationLayerInfo RotationLayerInfo { get; }

            internal static RotationBuffers Create(
                RotationLineInfo[] rotationLineInfo,
                Face rotationFace,
                RotationLayerInfo rotationLayerInfo)
            {
                return new RotationBuffers(
                    rotationLineInfo,
                    rotationFace,
                    rotationLayerInfo);
            }

            private RotationBuffers(
                RotationLineInfo[] rotationLineInfo,
                Face rotationFace,
                RotationLayerInfo rotationLayerInfo)
            {
                RotationLineInfo = rotationLineInfo;
                RotationFace = rotationFace;
                RotationLayerInfo = rotationLayerInfo;
            }
        }

        internal CubeController(
            Dictionary<Face, FaceManagers> faceManagersMap,
            int cubeSize)
        {
            Setup(faceManagersMap, cubeSize);
        }

        public void Setup(
            Dictionary<Face, FaceManagers> faceManagersMap,
            int cubeSize)
        {
            Thrower.IfNull(faceManagersMap, nameof(faceManagersMap));

            _faceManagersMap = faceManagersMap;
            _cachedSize = cubeSize;

            for (int i = 0; i < _cachedSize; i++)
            {
                _rotationBuffersMap[i] = RotationBuffers.Create(null, default, default);
            }
        }

        public FaceManagers GetFaceManagers(Face face)
        {
            Thrower.IfTrue(
                !_faceManagersMap.ContainsKey(face),
                $"指定された面 {face} に対応するマネージャが見つかりません。"
            );

            return _faceManagersMap[face];
        }

        public void SetRotationBuffers(
            Face face,
            int layerIndex)
        {
            Thrower.IfOutOfRange(layerIndex, 0, _cachedSize - 1);

            var rotationLineInfo = face.GetRotationLineInfos(layerIndex, _cachedSize);
            var rotationLayerInfo = new RotationLayerInfo(layerIndex, _cachedSize);

            Face rotationFace = rotationLayerInfo.IsMiddleLayer ? default : face.GetRotationFace(rotationLayerInfo);

            _rotationBuffersMap[layerIndex] = 
                RotationBuffers.Create(
                rotationLineInfo,
                rotationFace,
                rotationLayerInfo
            );
        }

        public void ClearRotationBuffers(int layerIndex)
        {
            _rotationBuffersMap[layerIndex] = RotationBuffers.Create(null, default, default);
        }

        private PieceInfo[] GetLinePieces(RotationLineInfo lineInfo)
        {
            return GetFaceManagers(lineInfo.Face).Swapper.GetLinePieces(lineInfo);
        }

        private void SetLinePieces(RotationLineInfo lineInfo, PieceInfo[] otherPieces)
        {
            GetFaceManagers(lineInfo.Face).Swapper.ReplacePieces(lineInfo, otherPieces);
        }

        private bool GetCorrectRotationDirectionForSide(
            Face face,
            bool initialIsClockwise)
        {
            const Face RTB = (Face)0x1A;
            return initialIsClockwise ^ face.IsContainedIn(RTB);
        }

        private bool ShouldReverseSide(int sideIndex, bool isClockwise) => isClockwise ^ sideIndex.IsOdd();

        public void RotateSideLines(
            Face face,
            int layerIndex,
            bool isClockwise)
        {
            var buffers = _rotationBuffersMap[layerIndex];

            isClockwise = GetCorrectRotationDirectionForSide(face, isClockwise);

            PieceInfo[][] sidePieces = new PieceInfo[SIDE_LINE_COUNT][];

            for (int i = 0; i < SIDE_LINE_COUNT; i++)
            {
                sidePieces[i] = GetLinePieces(buffers.RotationLineInfo[i]);

                if (ShouldReverseSide(i, isClockwise))
                {
                    Array.Reverse(sidePieces[i]);
                }
            }

            int[] lineCycle = isClockwise ? new[] { 0, 1, 2, 3 } : new[] { 0, 3, 2, 1 };

            var tempSide = sidePieces[lineCycle[0]];

            for (int i = 0; i < lineCycle.Length - 1; i++)
            {
                SetLinePieces(buffers.RotationLineInfo[lineCycle[i]], sidePieces[lineCycle[i + 1]]);
            }

            SetLinePieces(buffers.RotationLineInfo[lineCycle[^1]], tempSide);
        }

        private bool GetCorrectRotationDirectionForSurface(
            bool initialIsClockwise,
            RotationBuffers buffers)
        {
            const Face TLB = (Face)0x16;

            if (buffers.RotationLayerInfo.IsOppositeLayer)
            {
                return initialIsClockwise ^ buffers.RotationFace.IsContainedIn(TLB);
            }

            return initialIsClockwise ^ !buffers.RotationFace.IsContainedIn(TLB);
        }

        public void RotateFaceSurface(int layerIndex, bool isClockwise)
        {
            var buffers = _rotationBuffersMap[layerIndex];

            if (buffers.RotationLayerInfo.IsMiddleLayer) return;

            isClockwise = GetCorrectRotationDirectionForSurface(isClockwise, buffers);

            GetFaceManagers(buffers.RotationFace).Swapper.Rotate(isClockwise);
        }

        public void ParentBufferedPiecesTo(int layerIndex, Transform parent)
        {
            var buffers = _rotationBuffersMap[layerIndex];

            foreach (var lineInfo in buffers.RotationLineInfo)
            {
                GetFaceManagers(lineInfo.Face).Manipulator.ParentLine(lineInfo, parent);
            }

            if (buffers.RotationLayerInfo.IsMiddleLayer) return;

            GetFaceManagers(buffers.RotationFace).Manipulator.UnparentAll(parent);
        }

        public void RotateFaceSurfacePieces(
            int layerIndex,
            int angle,
            Vector3 localAxis)
        {
            var buffers = _rotationBuffersMap[layerIndex];

            if (buffers.RotationLayerInfo.IsMiddleLayer) return;

            if (buffers.RotationLayerInfo.IsOppositeLayer)
            {
                angle = angle.Negate();
            }

            GetFaceManagers(buffers.RotationFace).Manipulator.RotateAll(angle, localAxis);
        }

        public void RotateSideLinePieces(
            int layerIndex,
            int angle,
            Vector3 worldAxis)
        {
            var buffers = _rotationBuffersMap[layerIndex];

            foreach (var lineInfo in buffers.RotationLineInfo)
            {
                GetFaceManagers(lineInfo.Face).Manipulator.RotateLine(lineInfo, angle, worldAxis);
            }
        }

        public void SaveAllPiecePositions()
        {
            foreach (var manager in _faceManagersMap.Values)
            {
                manager.CoordinateRegistry.SaveAllPositions();
            }
        }

        public void SaveAllPieceRotations()
        {
            foreach (var manager in _faceManagersMap.Values)
            {
                manager.CoordinateRegistry.SaveAllRotations();
            }
        }

        public void SaveBufferedPiecePositions(int layerIndex)
        {
            var buffers = _rotationBuffersMap[layerIndex];

            foreach (var lineInfo in buffers.RotationLineInfo)
            {
                GetFaceManagers(lineInfo.Face).CoordinateRegistry.SavePositions(lineInfo);
            }

            if (buffers.RotationLayerInfo.IsMiddleLayer) return;

            GetFaceManagers(buffers.RotationFace).CoordinateRegistry.SaveAllPositions();
        }

        public void SaveBufferedPieceRotations(int layerIndex)
        {
            var buffers = _rotationBuffersMap[layerIndex];

            foreach (var lineInfo in buffers.RotationLineInfo)
            {
                GetFaceManagers(lineInfo.Face).CoordinateRegistry.SaveRotations(lineInfo);
            }

            if (buffers.RotationLayerInfo.IsMiddleLayer) return;

            GetFaceManagers(buffers.RotationFace).CoordinateRegistry.SaveAllRotations();
        }

        public void RestoreAllPiecePositions()
        {
            foreach (var manager in _faceManagersMap.Values)
            {
                manager.CoordinateRegistry.RestoreAllPositions();
            }
        }

        public void RestoreAllPieceRotations()
        {
            foreach (var manager in _faceManagersMap.Values)
            {
                manager.CoordinateRegistry.RestoreAllRotations();
            }
        }

        public void RestoreBufferedPiecePositions(int layerIndex)
        {
            var buffers = _rotationBuffersMap[layerIndex];

            foreach (var lineInfo in buffers.RotationLineInfo)
            {
                GetFaceManagers(lineInfo.Face).CoordinateRegistry.RestorePositions(lineInfo);
            }

            if (buffers.RotationLayerInfo.IsMiddleLayer) return;

            GetFaceManagers(buffers.RotationFace).CoordinateRegistry.RestoreAllPositions();
        }

        public void RestoreBufferedPieceRotations(int layerIndex)
        {
            var buffers = _rotationBuffersMap[layerIndex];

            foreach (var lineInfo in buffers.RotationLineInfo)
            {
                GetFaceManagers(lineInfo.Face).CoordinateRegistry.RestoreRotations(lineInfo);
            }

            if (buffers.RotationLayerInfo.IsMiddleLayer) return;

            GetFaceManagers(buffers.RotationFace).CoordinateRegistry.RestoreAllRotations();
        }
    }
}
