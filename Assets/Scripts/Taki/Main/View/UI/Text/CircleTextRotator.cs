using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using TMPro;
using Taki.Utility;
using UnityEngine;
using Taki.Audio;

namespace Taki.Main.View
{
    [Serializable]
    public struct TextRotatorObject
    {
        public RectTransform RectTransform;
        public TextMeshProUGUI Text;
        public int Index;
    }

    public class CircleTextRotator : MonoBehaviour, IUserInputLock
    {
        [SerializeField] private List<TextRotatorObject> _rotatorObjects;
        [SerializeField] private float _radius = 5.0f;
        [SerializeField] private RectTransform _rotationAxis;
        [SerializeField] private float _snapDuration = 0.5f;
        [SerializeField] private Ease _snapEase = Ease.OutQuad;
        [SerializeField] private bool _ignoreTimeScale = false;

        public readonly int CenterIndex = 3;

        public TextRotatorObject GetRotatorObject(int index) => _rotatorObjects[index];

        private readonly Subject<Unit> _onRotationComplete = new Subject<Unit>();
        public Observable<Unit> OnRotationComplete => _onRotationComplete;

        private bool _isLocked = false;
        private bool _isTaskExecuting = false;

        public bool IsLocked => _isLocked || _isTaskExecuting;

        private Dictionary<KeyCode, Func<CancellationToken, UniTask>> _keyActionMap = new();

        private void Update()
        {
            if (IsLocked)
            {
                return;
            }

            _keyActionMap.ToList().ForEach(pair =>
            {
                if (Input.GetKeyDown(pair.Key))
                {
                    pair.Value
                    .Invoke(destroyCancellationToken)
                    .SuppressCancellationThrow()
                    .Forget();
                }
            });
        }

        public void OnDestroy()
        {
            _onRotationComplete.Dispose();
        }

        public void Initialize()
        {
            InitializeKeyActionMap();

            var circlePoints =
                CirclePointCalculator
                .GenerateCirclePoints(
                    Vector3.zero,
                    _radius,
                    _rotatorObjects.Count,
                    GridPlane.XY);

            for (int i = 0; i < _rotatorObjects.Count; i++)
            {
                var rotatorObject = _rotatorObjects[i];
                rotatorObject.RectTransform.anchoredPosition = circlePoints[i];
                rotatorObject.RectTransform.SetParent(_rotationAxis);
                rotatorObject.Text.text = rotatorObject.Index.ToString();
            }

            RotateInstantly(1);
        }

        private void InitializeKeyActionMap()
        {
            _keyActionMap = new Dictionary<KeyCode, Func<CancellationToken, UniTask>>()
            {
                { KeyCode.A, RotateCounterClockwise },
                { KeyCode.D, RotateClockwise }
            };
        }

        public void SetInputLock(bool isLocked)
        {
            _isLocked = isLocked;
        }

        private UniTask RotateClockwise(CancellationToken token) => Rotate(-1, token);
        private UniTask RotateCounterClockwise(CancellationToken token) => Rotate(1, token);

        private UniTask Rotate(int direction, CancellationToken token)
        {
            _isTaskExecuting = true;
            UpdateTextAlpha(direction);

            float angleStep = 360f / _rotatorObjects.Count;
            float targetZ = _rotationAxis.localEulerAngles.z + angleStep * direction;
            var sequence = DOTween.Sequence();
            sequence.Append(_rotationAxis.DOLocalRotate(new Vector3(0, 0, targetZ), _snapDuration));

            _rotatorObjects.ForEach(rotatorObject =>
            {
                float childTargetZ = rotatorObject.RectTransform.localEulerAngles.z - angleStep * direction;
                sequence.Insert(
                    0,
                    rotatorObject
                    .RectTransform
                    .DOLocalRotate(
                        new Vector3(
                            rotatorObject.RectTransform.localEulerAngles.x,
                            rotatorObject.RectTransform.localEulerAngles.y,
                            childTargetZ),
                        _snapDuration));
            });

            sequence.OnComplete(() => OnRotationCompleted(direction));

            return sequence.SetEase(_snapEase).SetUpdate(_ignoreTimeScale).WithCancellation(token);
        }

        private void RotateInstantly(int direction)
        {
            UpdateTextAlpha(direction);

            float angleStep = 360f / _rotatorObjects.Count;
            float targetZ = _rotationAxis.localEulerAngles.z + angleStep * direction;
            _rotationAxis.localEulerAngles = new Vector3(0, 0, targetZ);

            _rotatorObjects.ForEach(rotatorObject =>
            {
                float childTargetZ = rotatorObject.RectTransform.localEulerAngles.z - angleStep * direction;
                rotatorObject.RectTransform.localEulerAngles = new Vector3(
                    rotatorObject.RectTransform.localEulerAngles.x,
                    rotatorObject.RectTransform.localEulerAngles.y,
                    childTargetZ);
            });

            if (direction > 0)
            {
                var last = _rotatorObjects.Last();
                _rotatorObjects.RemoveAt(_rotatorObjects.Count - 1);
                _rotatorObjects.Insert(0, last);
            }
            else
            {
                var first = _rotatorObjects[0];
                _rotatorObjects.RemoveAt(0);
                _rotatorObjects.Add(first);
            }
        }

        private void UpdateTextAlpha(int direction)
        {
            _rotatorObjects.ForEach(obj => obj.Text.color = new Color(obj.Text.color.r, obj.Text.color.g, obj.Text.color.b, 0));

            int centerIndex = (CenterIndex - direction + _rotatorObjects.Count) % _rotatorObjects.Count;
            _rotatorObjects[centerIndex].Text.DOFade(1f, _snapDuration).SetUpdate(_ignoreTimeScale);

            for (int i = 1; i < 4; i++)
            {
                float alpha = 1.0f - (i * 0.3f);

                int rightIndex = (centerIndex + i) % _rotatorObjects.Count;
                _rotatorObjects[rightIndex].Text.DOFade(alpha, _snapDuration).SetUpdate(_ignoreTimeScale);

                int leftIndex = (centerIndex - i + _rotatorObjects.Count) % _rotatorObjects.Count;
                _rotatorObjects[leftIndex].Text.DOFade(alpha, _snapDuration).SetUpdate(_ignoreTimeScale);
            }
        }

        private void OnRotationCompleted(int direction)
        {
            if (direction > 0)
            {
                var last = _rotatorObjects.Last();
                _rotatorObjects.RemoveAt(_rotatorObjects.Count - 1);
                _rotatorObjects.Insert(0, last);
            }
            else
            {
                var first = _rotatorObjects[0];
                _rotatorObjects.RemoveAt(0);
                _rotatorObjects.Add(first);
            }

            AudioManager.Instance.Play("LockEngage", gameObject).SetVolume(0.5f);
            _onRotationComplete.OnNext(Unit.Default);
            _isTaskExecuting = false;
        }
    }
}