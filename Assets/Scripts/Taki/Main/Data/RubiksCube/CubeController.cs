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

        private struct RotationBuffers
        {
            public RotationLineInfo[] RotationLineInfoBuffer;
            public Face RotationFaceBuffer;
            public RotationLayerInfo RotationLayerInfoBuffer;
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
                _rotationBuffersMap[i] = new RotationBuffers();
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

            var buffers = _rotationBuffersMap[layerIndex];
            buffers.RotationLineInfoBuffer = face.GetRotationLineInfos(layerIndex, _cachedSize);
            buffers.RotationLayerInfoBuffer = new RotationLayerInfo(layerIndex, _cachedSize);

            if (buffers.RotationLayerInfoBuffer.IsMiddleLayer)
            {
                buffers.RotationFaceBuffer = default;
            }
            else
            {
                buffers.RotationFaceBuffer = face.GetRotationFace(buffers.RotationLayerInfoBuffer);
            }

            _rotationBuffersMap[layerIndex] = buffers;
        }

        public void ClearRotationBuffers(int layerIndex)
        {
            _rotationBuffersMap.Remove(layerIndex);
        }

        private PieceInfo[] GetLinePieces(RotationLineInfo lineInfo)
        {
            return GetFaceManagers(lineInfo.Face).Swapper.GetLinePieces(lineInfo);
        }

        private void SetLinePieces(RotationLineInfo lineInfo, PieceInfo[] otherPieces)
        {
            GetFaceManagers(lineInfo.Face).Swapper.ReplacePieces(lineInfo, otherPieces);
        }

        private void SwapPieceTransforms(RotationLineInfo lineInfo, Transform[] otherTransforms)
        {
            GetFaceManagers(lineInfo.Face).Swapper.SwapPositions(lineInfo, otherTransforms);
        }

        private bool GetCorrectRotationDirectionForSide(
            Face face,
            bool initialIsClockwise)
        {
            var facesThatInvertDirection =
                Face.Right |
                Face.Top |
                Face.Back;

            return initialIsClockwise ^ face.IsContainedIn(facesThatInvertDirection);
        }

        private bool ShouldReverseSide(int sideIndex, bool isClockwise)
            => (isClockwise ? 0 : 1) == (sideIndex & 1);

        public void RotateSideLines(
            Face face,
            int layerIndex,
            bool isClockwise,
            bool shouldSwapTransforms)
        {
            var buffers = _rotationBuffersMap[layerIndex];

            isClockwise = GetCorrectRotationDirectionForSide(face, isClockwise);

            PieceInfo[][] sidePieces = new PieceInfo[SIDE_LINE_COUNT][];

            for (int i = 0; i < SIDE_LINE_COUNT; i++)
            {
                sidePieces[i] = GetLinePieces(buffers.RotationLineInfoBuffer[i]);

                if (ShouldReverseSide(i, isClockwise))
                {
                    Array.Reverse(sidePieces[i]);
                }
            }

            Transform[][] pieceTransforms = null;
            if (shouldSwapTransforms)
            {
                pieceTransforms = new Transform[SIDE_LINE_COUNT][];
                for (int i = 0; i < SIDE_LINE_COUNT; i++)
                {
                    pieceTransforms[i] = new Transform[sidePieces[i].Length];
                    for (int j = 0; j < sidePieces[i].Length; j++)
                    {
                        pieceTransforms[i][j] = sidePieces[i][j].Transform;
                    }
                }
            }

            int[] sideRotationOrder = isClockwise ? new[] { 0, 1, 2, 3 } : new[] { 0, 3, 2, 1 };

            if (shouldSwapTransforms)
            {
                SwapPieceTransforms(
                    buffers.RotationLineInfoBuffer[sideRotationOrder[0]],
                    pieceTransforms[sideRotationOrder[1]]);

                SwapPieceTransforms(
                    buffers.RotationLineInfoBuffer[sideRotationOrder[^1]],
                    pieceTransforms[sideRotationOrder[0]]);

                SwapPieceTransforms(
                    buffers.RotationLineInfoBuffer[sideRotationOrder[2]],
                    pieceTransforms[sideRotationOrder[^1]]);
            }

            var tempSide = sidePieces[sideRotationOrder[0]];

            for (int i = 0; i < sideRotationOrder.Length - 1; i++)
            {
                SetLinePieces(
                    buffers.RotationLineInfoBuffer[sideRotationOrder[i]],
                    sidePieces[sideRotationOrder[i + 1]]);
            }

            SetLinePieces(
                buffers.RotationLineInfoBuffer[sideRotationOrder[^1]],
                tempSide);
        }

        private bool GetCorrectRotationDirectionForSurface(bool initialIsClockwise, RotationBuffers buffers)
        {
            var facesThatRequireInversion =
                Face.Top |
                Face.Left |
                Face.Back;

            return initialIsClockwise ^
                   (buffers.RotationFaceBuffer
                       .IsContainedIn(facesThatRequireInversion)
                       == buffers.RotationLayerInfoBuffer.IsOppositeLayer);
        }

        public void RotateFaceSurface(int layerIndex, bool isClockwise)
        {
            var buffers = _rotationBuffersMap[layerIndex];

            if (buffers.RotationLayerInfoBuffer.IsMiddleLayer) return;

            isClockwise = GetCorrectRotationDirectionForSurface(isClockwise, buffers);

            GetFaceManagers(buffers.RotationFaceBuffer).Swapper.Rotate(isClockwise);
        }

        public void ParentBufferedPiecesTo(int layerIndex, Transform parent)
        {
            var buffers = _rotationBuffersMap[layerIndex];

            foreach (var lineInfo in buffers.RotationLineInfoBuffer)
            {
                GetFaceManagers(lineInfo.Face).Manipulator.ParentLine(lineInfo, parent);
            }

            if (buffers.RotationLayerInfoBuffer.IsMiddleLayer) return;
            GetFaceManagers(buffers.RotationFaceBuffer).Manipulator.UnparentAll(parent);
        }

        public void RotateFaceSurfacePieces(
            int layerIndex,
            int angle,
            Vector3 localAxis)
        {
            var buffers = _rotationBuffersMap[layerIndex];

            if (buffers.RotationLayerInfoBuffer.IsMiddleLayer) return;

            if (buffers.RotationLayerInfoBuffer.IsOppositeLayer)
            {
                angle = angle.Negate();
            }

            GetFaceManagers(buffers.RotationFaceBuffer).Manipulator.RotateAll(angle, localAxis);
        }

        public void RotateSideLinePieces(
            int layerIndex,
            int angle,
            Vector3 worldAxis)
        {
            var buffers = _rotationBuffersMap[layerIndex];

            foreach (var lineInfo in buffers.RotationLineInfoBuffer)
            {
                GetFaceManagers(lineInfo.Face).Manipulator.RotateLine(lineInfo, angle, worldAxis);
            }
        }

        public void SaveAllPiecePositions()
        {
            foreach (var manager in _faceManagersMap.Values)
            {
                manager.CoordinatesManager.SaveAllPositions();
            }
        }

        public void SaveAllPieceRotations()
        {
            foreach (var manager in _faceManagersMap.Values)
            {
                manager.CoordinatesManager.SaveAllRotations();
            }
        }

        public void SaveBufferedPiecePositions(int layerIndex)
        {
            var buffers = _rotationBuffersMap[layerIndex];

            foreach (var lineInfo in buffers.RotationLineInfoBuffer)
            {
                GetFaceManagers(lineInfo.Face).CoordinatesManager.SavePositions(lineInfo);
            }

            if (buffers.RotationLayerInfoBuffer.IsMiddleLayer) return;
            GetFaceManagers(buffers.RotationFaceBuffer).CoordinatesManager.SaveAllPositions();
        }

        public void SaveBufferedPieceRotations(int layerIndex)
        {
            var buffers = _rotationBuffersMap[layerIndex];

            foreach (var lineInfo in buffers.RotationLineInfoBuffer)
            {
                GetFaceManagers(lineInfo.Face).CoordinatesManager.SaveRotations(lineInfo);
            }

            if (buffers.RotationLayerInfoBuffer.IsMiddleLayer) return;
            GetFaceManagers(buffers.RotationFaceBuffer).CoordinatesManager.SaveAllRotations();
        }

        public void RestoreAllPiecePositions()
        {
            foreach (var manager in _faceManagersMap.Values)
            {
                manager.CoordinatesManager.RestoreAllPositions();
            }
        }

        public void RestoreAllPieceRotations()
        {
            foreach (var manager in _faceManagersMap.Values)
            {
                manager.CoordinatesManager.RestoreAllRotations();
            }
        }

        public void RestoreBufferedPiecePositions(int layerIndex)
        {
            var buffers = _rotationBuffersMap[layerIndex];

            foreach (var lineInfo in buffers.RotationLineInfoBuffer)
            {
                GetFaceManagers(lineInfo.Face).CoordinatesManager.RestorePositions(lineInfo);
            }

            if (buffers.RotationLayerInfoBuffer.IsMiddleLayer) return;
            GetFaceManagers(buffers.RotationFaceBuffer).CoordinatesManager.RestoreAllPositions();
        }

        public void RestoreBufferedPieceRotations(int layerIndex)
        {
            var buffers = _rotationBuffersMap[layerIndex];

            foreach (var lineInfo in buffers.RotationLineInfoBuffer)
            {
                GetFaceManagers(lineInfo.Face).CoordinatesManager.RestoreRotations(lineInfo);
            }

            if (buffers.RotationLayerInfoBuffer.IsMiddleLayer) return;
            GetFaceManagers(buffers.RotationFaceBuffer).CoordinatesManager.RestoreAllRotations();
        }
    }
}