using System.Collections.Generic;
using UnityEngine;
using Taki.Utility;

namespace Taki.StartMenu.View
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

        private RectTransform[,] _backgroundRects;
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

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    if (_backgroundRects[row, col] == null) continue;
                    _backgroundRects[row, col].anchoredPosition3D += new Vector3(delta.x, delta.y, 0f);
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
            if (_prefabs.Count == 0)
            {
                Debug.LogWarning("背景のプレハブが設定されていません。");
                return;
            }

            Vector3 center = _parent.transform.localPosition;
            _gridPoints = GridPointCalculator.GenerateGridPoints(
                center,
                _rows,
                _columns,
                _spacing,
                _plane);

            _genIndex = 0;
            _backgroundRects = new RectTransform[_rows, _columns];

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    CreateAt(row, col);
                }

                _genIndex -= 2;
            }

            _genIndex--;
        }

        private void ShiftGrid()
        {
            RectTransform[,] buffer = new RectTransform[_rows, _columns];

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    if (row < _rows - 1 && col < _columns - 1)
                    {
                        buffer[row, col + 1] = _backgroundRects[row + 1, col];
                    }
                }
            }

            for (int col = 0; col < _columns; col++)
            {
                if (_backgroundRects[0, col] != null)
                {
                    Destroy(_backgroundRects[0, col].gameObject);
                }
            }

            for (int row = 0; row < _rows; row++)
            {
                if (_backgroundRects[row, _columns - 1] != null)
                {
                    Destroy(_backgroundRects[row, _columns - 1].gameObject);
                }
            }

            _backgroundRects = buffer;

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    if (_backgroundRects[row, col] == null)
                    {
                        CreateAt(row, col);
                    }
                }
            }

            _genIndex += 3;
        }

        private void CreateAt(int row, int col)
        {
            int prefabIndex = _genIndex % _prefabs.Count;
            _genIndex++;

            GameObject prefab = _prefabs[prefabIndex];
            Vector3 spawnPos = _gridPoints[row, col];

            GameObject newInstance = Instantiate(prefab, _parent);
            RectTransform rect = newInstance.GetComponent<RectTransform>();

            if (rect != null)
            {
                rect.anchoredPosition3D = spawnPos;
                rect.transform.localScale = Vector3.one;
                _backgroundRects[row, col] = rect;
            }
        }
    }
}
