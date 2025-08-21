using UnityEngine;
using Taki.Utility.Core;
using System;

namespace Taki.Main.System
{
    public sealed class ChangeMonitor : Singleton<ChangeMonitor>
    {
        private Vector2 _lastMousePosition;
        private Quaternion _lastCameraRotation;

        private Transform _mainCameraTransform;

        [Flags]
        private enum ChangeFlags
        {
            None = 0,
            Mouse = 1 << 0,
            Camera = 1 << 1
        }

        private ChangeFlags _currentChanges = ChangeFlags.None;

        public bool HasBothChanged => 
            _currentChanges.HasFlag(ChangeFlags.Mouse) 
            && _currentChanges.HasFlag(ChangeFlags.Camera);

        public bool HasEitherChanged => _currentChanges != ChangeFlags.None;
        public bool HasMouseChanged => _currentChanges.HasFlag(ChangeFlags.Mouse);
        public bool HasCameraChanged => _currentChanges.HasFlag(ChangeFlags.Camera);

        private void Start()
        {
            _lastMousePosition = Input.mousePosition;
            _mainCameraTransform = Camera.main.transform;
            _lastCameraRotation = _mainCameraTransform.rotation;
        }

        private void Update()
        {
            ChangeFlags newChanges = ChangeFlags.None;

            if ((Vector2)Input.mousePosition != _lastMousePosition)
            {
                newChanges |= ChangeFlags.Mouse;
                _lastMousePosition = Input.mousePosition;
            }

            if (_mainCameraTransform.rotation != _lastCameraRotation)
            {
                newChanges |= ChangeFlags.Camera;
                _lastCameraRotation = _mainCameraTransform.rotation;
            }

            _currentChanges = newChanges;
        }

        public void ResetFlags()
        {
            _currentChanges = ChangeFlags.None;
        }
    }
}