using System.Collections.Generic;
using UnityEngine;
using Taki.Utility;
using System.Linq;

namespace Taki.Main.System
{
    public sealed class UICircleGenerator : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _prefabObjects = new();

        [Header("Circle Settings")]
        [SerializeField] private int _itemCount = 5;
        [SerializeField] private float _radius = 5.0f;
        [SerializeField] private GridPlane _plane = GridPlane.XY;

        public IReadOnlyList<Transform> InstantiatedItems => _instantiatedItems;
        public IReadOnlyList<Vector3> CirclePoints => _circlePoints;
        public IReadOnlyList<Vector3> DoubleRadiusCirclePoints => _doubleRadiusCirclePoints;

        private readonly List<Transform> _instantiatedItems = new();
        private readonly List<Vector3> _circlePoints = new();
        private readonly List<Vector3> _doubleRadiusCirclePoints = new();

        [ContextMenu("Create Items in a Circle")]
        public void CreateItems(bool isActive = true, Transform parentTransform = null)
        {
            DestroyItems();

            _circlePoints
                .AddRange(
                CirclePointCalculator
                .GenerateCirclePoints(
                    Vector3.zero,
                    _radius,
                    _itemCount,
                    _plane));

            _doubleRadiusCirclePoints
                .AddRange(
                CirclePointCalculator
                .GenerateCirclePoints(
                    Vector3.zero,
                    _radius * 2,
                    _itemCount,
                    _plane));

            for (int i = 0; i < _circlePoints.Count; i++)
            {
                GameObject prefab = _prefabObjects[i % _prefabObjects.Count];

                GameObject newItem =
                    Instantiate(
                        prefab,
                        parentTransform != null ? parentTransform : transform);

                newItem.SetActive(isActive);
                newItem.transform.localPosition = _circlePoints[i];

                Vector3 currentRotation = Vector3.zero;
                newItem.transform.localEulerAngles = currentRotation;

                _instantiatedItems.Add(newItem.transform);
            }
        }

        [ContextMenu("Clear Items in a Circle")]
        public void DestroyItems()
        {
            _instantiatedItems.ToList().ForEach(obj => DestroyImmediate(obj.gameObject));
            _instantiatedItems.Clear();
            _circlePoints.Clear();
            _doubleRadiusCirclePoints.Clear();
        }
    }
}