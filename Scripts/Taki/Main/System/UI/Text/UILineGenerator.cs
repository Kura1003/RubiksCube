using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Taki.Main.View;
using Taki.Utility;
using Cysharp.Threading.Tasks;

namespace Taki.Main.System
{
    public sealed class UILineGenerator : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _prefabObjects = new();
        [SerializeField] private Color _hoverColor = Color.white;
        [SerializeField] private PointerColorAnimator _pointerColorAnimator;

        [Header("Line Settings")]
        [SerializeField] private float _lineLength = 100f;
        [SerializeField] private float _spacing = 10f;
        [SerializeField] private GridPlane _plane = GridPlane.XZ;

        private async void Awake()
        {
            List<GraphicColorEntry> newEntries = GenerateUIElements();

            await UniTask.Yield();
            _pointerColorAnimator.SetGraphicEntries(newEntries, true);
        }

        private List<GraphicColorEntry> GenerateUIElements()
        {
            Vector3[] points = LinePointCalculator.GenerateLinePoints(
                Vector3.zero,
                _lineLength,
                _spacing,
                _plane);

            List<GraphicColorEntry> newEntries = new();

            for (int i = 0; i < points.Length; i++)
            {
                GameObject prefab = _prefabObjects[i % _prefabObjects.Count];
                GameObject newObject = Instantiate(prefab, transform);

                RectTransform rectTransform = newObject.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = points[i];

                Graphic graphic = newObject.GetComponent<Graphic>();
                GraphicColorEntry entry = new GraphicColorEntry
                {
                    Graphic = graphic,
                    HoverColor = _hoverColor
                };
                newEntries.Add(entry);
            }

            return newEntries;
        }
    }
}