using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Taki.Utility;

namespace Taki.StartMenu.UI
{
    public class StartMenuBackground : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> _prefabs = new();

        [SerializeField]
        private RectTransform _parent;

        [SerializeField]
        private int _rows = 3;

        [SerializeField]
        private int _columns = 3;

        [SerializeField]
        private float _spacing = 100f;

        [SerializeField]
        private GridPlane _plane = GridPlane.XY;

        [SerializeField]
        private bool _enableScroll = false;

        [SerializeField]
        private float _speed = 20f;

        private readonly List<List<GameObject>> _backgrounds = new();
        private readonly List<List<RectTransform>> _backgroundRects = new();
        private Vector3[,] _gridPoints;
        private Vector2 _shiftAccum = Vector2.zero;
        private Vector2 _scrollDir;

        private int _genIndex = 0;

        private void Start()
        {
            Vector2 start = new Vector2(0, 1);
            Vector2 end = new Vector2(1, 0);
            _scrollDir = (end - start).normalized;
        }

        private void Update()
        {
            if (!_enableScroll) return;

            Vector2 delta = _scrollDir * _speed * Time.deltaTime;
            _shiftAccum += delta;

            foreach (var rectRow in _backgroundRects)
            {
                foreach (var rect in rectRow)
                {
                    rect.anchoredPosition3D += new Vector3(delta.x, delta.y, 0f);
                }
            }

            if (_shiftAccum.x >= _spacing)
            {
                ShiftGrid();
                _shiftAccum = Vector2.zero;
            }
        }

        public void Generate()
        {
            Clear();

            if (_prefabs.Count == 0)
            {
                Debug.LogWarning("背景のプレハブが設定されていません。");
                return;
            }

            Vector3 center = _parent.transform.localPosition;
            _gridPoints = GridPointCalculator.GenerateGridPoints(center, _rows, _columns, _spacing, _plane);

            _genIndex = 0;
            _backgrounds.Clear();
            _backgroundRects.Clear();

            for (int col = 0; col < _columns; col++)
            {
                var backgroundList = new List<GameObject>();
                var rectList = new List<RectTransform>();

                _backgrounds.Add(backgroundList);
                _backgroundRects.Add(rectList);

                for (int row = 0; row < _rows; row++)
                {
                    CreateAt(row, col, backgroundList, rectList);
                }
            }

            _genIndex += 3;
        }

        public void Remove()
        {
            Clear();
        }

        private void ShiftGrid()
        {
            var tempBackgrounds = new List<List<GameObject>>();
            var tempRects = new List<List<RectTransform>>();

            for (int col = 0; col < _columns; col++)
            {
                tempBackgrounds.Add(Enumerable.Repeat<GameObject>(null, _rows).ToList());
                tempRects.Add(Enumerable.Repeat<RectTransform>(null, _rows).ToList());
            }

            for (int col = 0; col < _columns - 1; col++)
            {
                for (int row = 0; row < _rows; row++)
                {
                    tempBackgrounds[col + 1][row] = _backgrounds[col][row];
                    tempRects[col + 1][row] = _backgroundRects[col][row];
                }
            }

            _backgrounds[0].ForEach(Destroy);

            for (int row = 0; row < _rows; row++)
            {
                CreateAt(row, 0, tempBackgrounds[0], tempRects[0]);
            }

            _backgrounds.Clear();
            _backgrounds.AddRange(tempBackgrounds);
            _backgroundRects.Clear();
            _backgroundRects.AddRange(tempRects);
        }

        private void CreateAt(int row, int col, List<GameObject> rowList, List<RectTransform> rectList)
        {
            int prefabIndex = _genIndex % _prefabs.Count;
            _genIndex++;

            GameObject prefab = _prefabs[prefabIndex];
            Vector3 spawnPos = _gridPoints[row, col];

            GameObject newInstance = Instantiate(prefab, _parent);
            RectTransform rect = newInstance.GetComponent<RectTransform>();
            rect.anchoredPosition3D = spawnPos;
            rect.transform.localScale = Vector3.one;

            if (rowList.Count > row)
            {
                rowList[row] = newInstance;
                rectList[row] = rect;
            }
            else
            {
                rowList.Add(newInstance);
                rectList.Add(rect);
            }
            Debug.Log($"[生成] 行: {row}, 列: {col}, プレハブインデックス: {prefabIndex}");
        }

        private void Clear()
        {
            _backgrounds.SelectMany(rowList => rowList)
                .ToList()
                .ForEach(Destroy);

            _backgrounds.Clear();
            _backgroundRects.Clear();
            _gridPoints = null;
        }
    }
}