using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using Taki.Audio;
using Taki.Utility;

namespace Taki.Main.View
{
    public class CameraRotator : MonoBehaviour, IUserInputLock
    {
        [SerializeField] private Transform _rotationAxis;
        [SerializeField] private float _rotationSpeed = 90.0f;
        [SerializeField] private float _angleThreshold = 30.0f;
        [SerializeField, Range(0f, 1f)] private float _decayRate = 0.9f;

        private float _totalAngleMoved = 0f;
        private Vector3 _currentRotationDirection = Vector3.zero;
        private float _currentRotationMagnitude = 0f;
        private bool _isInputActive = false;
        private bool _isInputLocked = false;

        public bool IsLocked => _isInputLocked;

        private static readonly Dictionary<KeyCode, Vector3> _keyDirectionMap =
            new Dictionary<KeyCode, Vector3>
        {
            { KeyCode.A, Vector3.forward },
            { KeyCode.D, Vector3.back },
            { KeyCode.W, Vector3.left },
            { KeyCode.S, Vector3.right }
        };

        public void SetInputLock(bool isLocked)
        {
            _isInputLocked = isLocked;
            if (_isInputLocked)
            {
                _currentRotationMagnitude = 0f;
            }
        }

        private void LateUpdate()
        {
            if (_isInputLocked)
            {
                return;
            }

            Vector3 inputDirection = _keyDirectionMap
                .Where(keyPair => Input.GetKey(keyPair.Key))
                .Aggregate(Vector3.zero, (current, keyPair) => current + keyPair.Value);

            _isInputActive = inputDirection.sqrMagnitude > 0.001f;

            if (_isInputActive)
            {
                _currentRotationDirection = inputDirection.normalized;
                _currentRotationMagnitude = _rotationSpeed;
            }

            else
            {
                ApplyDecay();
            }

            ApplyRotation();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                ResetCameraAngle();
            }
        }

        private void ApplyRotation()
        {
            float deltaTime = Time.deltaTime;
            float angleThisFrame = _currentRotationMagnitude * deltaTime;
            _rotationAxis.rotation *= Quaternion.AngleAxis(
                angleThisFrame,
                _currentRotationDirection);

            _totalAngleMoved += angleThisFrame;

            if (_totalAngleMoved >= _angleThreshold)
            {
                OnAngleThresholdExceeded();
                _totalAngleMoved = 0f;
            }
        }

        private void ApplyDecay()
        {
            _currentRotationMagnitude *= _decayRate;

            if (_currentRotationMagnitude < 0.01f)
            {
                _currentRotationMagnitude = 0f;
                _currentRotationDirection = Vector3.zero;
            }
        }

        private void ResetCameraAngle()
        {
            _rotationAxis.rotation = Quaternion.identity;
            _totalAngleMoved = 0f;
            _currentRotationDirection = Vector3.zero;
            _currentRotationMagnitude = 0f;
            _isInputActive = false;

            AudioManager.Instance.Play("”µŽžŒv", gameObject).SetVolume(0.5f);
        }

        private void OnAngleThresholdExceeded()
        {
            AudioManager.Instance.Play("GearTurn", gameObject).SetVolume(0.5f);
        }
    }
}