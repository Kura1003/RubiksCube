using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Taki.Main.Data.RubiksCube;
using Taki.Utility;
using Taki.Utility.Core;
using UnityEngine;
using R3;
using System;
using Cysharp.Threading.Tasks;

namespace Taki.Main.System.RubiksCube
{
    public class RubiksCubeFactory : MonoBehaviour, ICubeFactory, ICubeCancellationToken
    {
        [SerializeField] private List<GameObject> _cubePiecePrefabs = new List<GameObject>();

        private readonly Subject<Unit> _onCubeCreated = new Subject<Unit>();
        private readonly Subject<Unit> _onCubeDestroyed = new Subject<Unit>();

        public Observable<Unit> OnCubeCreated => _onCubeCreated;
        public Observable<Unit> OnCubeDestroyed => _onCubeDestroyed;

        private CancellationTokenSource _cancellationTokenSource;

        public CancellationToken GetToken() => _cancellationTokenSource?.Token ?? CancellationToken.None;

        private List<GameObject> _instantiatedPieces = new();
        private List<GameObject> _instantiatedRotationAxes = new();

        private void OnDestroy()
        {
            _onCubeCreated.Dispose();
            _onCubeDestroyed.Dispose();
            CancelAndDispose();
        }

        public void CancelAndDispose()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        public CubeGenerationInfo Create(
            float pieceSpacing,
            int cubeSize,
            Transform parentTransform)
        {
            try
            {
                CancelAndDispose();
                _cancellationTokenSource = new CancellationTokenSource();

                Thrower.IfNull(parentTransform, nameof(parentTransform));

                var faceSpawnData = new List<FaceSpawnDataBase>
                {
                    new FrontFaceSpawnData(),
                    new BackFaceSpawnData(),
                    new LeftFaceSpawnData(),
                    new RightFaceSpawnData(),
                    new TopFaceSpawnData(),
                    new BottomFaceSpawnData()
                };

                var faceManagersMap = new Dictionary<Face, FaceManagers>();
                var axisInfoMap = new Dictionary<Face, RotationAxisInfo>();

                _instantiatedPieces.Clear();
                _instantiatedRotationAxes.Clear();

                var prefabIndex = 0;
                var prefabCount = _cubePiecePrefabs.Count;

                Thrower.IfTrue(
                    prefabCount == 0,
                    $"キューブピースのプレハブリストが空です。" +
                    $"現在のプレハブ数: {prefabCount}");

                foreach (var config in faceSpawnData)
                {
                    var face = config.Face;
                    var id = face.ToString()[0].ToString();
                    var faceSpawnInfo = config.SpawnInfo;
                    var faceNormal = parentTransform.TransformDirection(faceSpawnInfo.Normal);
                    var faceCenter = parentTransform.position + faceNormal * (cubeSize * pieceSpacing / 2f);

                    var piecesMatrix = new PieceInfo[cubeSize, cubeSize];

                    var gridPoints = GridPointCalculator.GenerateGridPoints(
                        Vector3.zero,
                        cubeSize,
                        cubeSize,
                        pieceSpacing,
                        faceSpawnInfo.Plane);

                    for (int row = 0; row < cubeSize; row++)
                    {
                        for (int col = 0; col < cubeSize; col++)
                        {
                            var localOffset = gridPoints[row, col];
                            var spawnPos = faceCenter + localOffset;
                            var rotation = Quaternion.LookRotation(faceNormal)
                                * Quaternion.Euler(faceSpawnInfo.RotationOffset);

                            var pieceGO = Instantiate(
                                _cubePiecePrefabs[prefabIndex % prefabCount],
                                spawnPos,
                                rotation,
                                parentTransform);

                            _instantiatedPieces.Add(pieceGO);

                            var pieceInfo = new PieceInfo(pieceGO.transform, id);
                            piecesMatrix[row, col] = pieceInfo;
                        }
                    }

                    Vector3 firstPiecePos = piecesMatrix[0, 0].Transform.position;
                    Vector3 lastPiecePos = piecesMatrix[cubeSize - 1, cubeSize - 1].Transform.position;
                    Vector3 axisPosition = (firstPiecePos + lastPiecePos) / 2f;

                    var rotationAxesGameObjects = new List<GameObject>();
                    for (int i = 0; i < cubeSize; i++)
                    {
                        var axisGO = new GameObject($"{face}RotationAxis{i + 1}");
                        axisGO.transform.position = axisPosition;
                        axisGO.transform.LookAt(parentTransform);
                        axisGO.transform.SetParent(parentTransform);
                        rotationAxesGameObjects.Add(axisGO);
                    }

                    var rotationAxesTransforms = rotationAxesGameObjects.Select(go => go.transform).ToList();

                    var faceManagers = new FaceManagers(piecesMatrix, cubeSize);
                    var rotationAxisInfo = new RotationAxisInfo(faceSpawnInfo.Normal, rotationAxesTransforms);

                    faceManagersMap.Add(face, faceManagers);
                    axisInfoMap.Add(face, rotationAxisInfo);

                    _instantiatedRotationAxes.AddRange(rotationAxesGameObjects);

                    prefabIndex++;
                }

                _onCubeCreated.OnNext(Unit.Default);

                return new CubeGenerationInfo(faceManagersMap, axisInfoMap);
            }
            catch (Exception ex)
            {
                Debug.LogError($"キューブの生成中にエラーが発生しました: {ex.Message}");
                return default;
            }
        }

        public async UniTask<CubeGenerationInfo?> CreateAsync(
            float pieceSpacing,
            int cubeSize,
            Transform parentTransform)
        {
            try
            {
                CancelAndDispose();
                _cancellationTokenSource = new CancellationTokenSource();
                var token = _cancellationTokenSource.Token;

                Thrower.IfNull(parentTransform, nameof(parentTransform));

                var faceSpawnData = new List<FaceSpawnDataBase>
                {
                    new FrontFaceSpawnData(),
                    new BackFaceSpawnData(),
                    new LeftFaceSpawnData(),
                    new RightFaceSpawnData(),
                    new TopFaceSpawnData(),
                    new BottomFaceSpawnData()
                };

                var faceManagersMap = new Dictionary<Face, FaceManagers>();
                var axisInfoMap = new Dictionary<Face, RotationAxisInfo>();

                _instantiatedPieces.Clear();
                _instantiatedRotationAxes.Clear();

                var prefabIndex = 0;
                var prefabCount = _cubePiecePrefabs.Count;

                Thrower.IfTrue(
                    prefabCount == 0,
                    $"キューブピースのプレハブリストが空です。" +
                    $"現在のプレハブ数: {prefabCount}");

                foreach (var config in faceSpawnData)
                {
                    token.ThrowIfCancellationRequested();

                    var face = config.Face;
                    var id = face.ToString()[0].ToString();
                    var faceSpawnInfo = config.SpawnInfo;
                    var faceNormal = parentTransform.TransformDirection(faceSpawnInfo.Normal);
                    var faceCenter = parentTransform.position + faceNormal * (cubeSize * pieceSpacing / 2f);

                    var piecesMatrix = new PieceInfo[cubeSize, cubeSize];

                    var gridPoints = GridPointCalculator.GenerateGridPoints(
                        Vector3.zero,
                        cubeSize,
                        cubeSize,
                        pieceSpacing,
                        faceSpawnInfo.Plane);

                    for (int row = 0; row < cubeSize; row++)
                    {
                        token.ThrowIfCancellationRequested();

                        for (int col = 0; col < cubeSize; col++)
                        {
                            var localOffset = gridPoints[row, col];
                            var spawnPos = faceCenter + localOffset;
                            var rotation = Quaternion.LookRotation(faceNormal)
                                * Quaternion.Euler(faceSpawnInfo.RotationOffset);

                            var pieceGO = Instantiate(
                                _cubePiecePrefabs[prefabIndex % prefabCount],
                                spawnPos,
                                rotation,
                                parentTransform);

                            _instantiatedPieces.Add(pieceGO);

                            var pieceInfo = new PieceInfo(pieceGO.transform, id);
                            piecesMatrix[row, col] = pieceInfo;
                        }

                        await UniTask.Yield();
                    }

                    Vector3 firstPiecePos = piecesMatrix[0, 0].Transform.position;
                    Vector3 lastPiecePos = piecesMatrix[cubeSize - 1, cubeSize - 1].Transform.position;
                    Vector3 axisPosition = (firstPiecePos + lastPiecePos) / 2f;

                    var rotationAxesGameObjects = new List<GameObject>();
                    for (int i = 0; i < cubeSize; i++)
                    {
                        var axisGO = new GameObject($"{face}RotationAxis{i + 1}");
                        axisGO.transform.position = axisPosition;
                        axisGO.transform.LookAt(parentTransform);
                        axisGO.transform.SetParent(parentTransform);
                        rotationAxesGameObjects.Add(axisGO);
                    }

                    await UniTask.Yield();
                    token.ThrowIfCancellationRequested();

                    var rotationAxesTransforms = rotationAxesGameObjects.Select(go => go.transform).ToList();

                    var faceManagers = new FaceManagers(piecesMatrix, cubeSize);
                    var rotationAxisInfo = new RotationAxisInfo(faceSpawnInfo.Normal, rotationAxesTransforms);

                    faceManagersMap.Add(face, faceManagers);
                    axisInfoMap.Add(face, rotationAxisInfo);

                    _instantiatedRotationAxes.AddRange(rotationAxesGameObjects);

                    prefabIndex++;
                }

                _onCubeCreated.OnNext(Unit.Default);

                return new CubeGenerationInfo(faceManagersMap, axisInfoMap);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("キューブの生成がキャンセルされました。");
                return default;
            }
            catch (Exception ex)
            {
                Debug.LogError($"キューブの生成中にエラーが発生しました: {ex.Message}");
                return default;
            }
        }

        public void Destroy()
        {
            CancelAndDispose();

            _instantiatedPieces
                .Where(piece => piece != null)
                .ToList()
                .ForEach(Destroy);
            _instantiatedPieces.Clear();

            _instantiatedRotationAxes
                .Where(axisGO => axisGO != null)
                .ToList()
                .ForEach(Destroy);
            _instantiatedRotationAxes.Clear();

            _onCubeDestroyed.OnNext(Unit.Default);
        }
    }
}