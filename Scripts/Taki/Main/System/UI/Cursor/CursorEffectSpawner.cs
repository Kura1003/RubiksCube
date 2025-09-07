using UnityEngine;

namespace Taki.Main.System
{
    public class CursorEffectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _cursorEffectPrefab;
        [SerializeField] private Canvas _targetCanvas;
        [SerializeField] private float _spawnInterval = 0.1f;

        private RectTransform _canvasRectTransform;
        private float _timeSinceLastSpawn;

        private void Awake()
        {
            _canvasRectTransform = _targetCanvas.GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            if (_targetCanvas.renderMode == RenderMode.ScreenSpaceCamera) return;

            HandleCursorEffectSpawn();
        }

        private void HandleCursorEffectSpawn()
        {
            if (!Input.GetMouseButton(0))
            {
                _timeSinceLastSpawn = _spawnInterval;
                return;
            }

            _timeSinceLastSpawn += Time.unscaledDeltaTime;
            if (_timeSinceLastSpawn >= _spawnInterval)
            {
                SpawnEffect();
            }
        }

        private void SpawnEffect()
        {
            _timeSinceLastSpawn = 0f;

            Vector2 mousePosition = Input.mousePosition;

            Camera cameraToUse =
                _targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay ?
                null : _targetCanvas.worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRectTransform,
                mousePosition,
                cameraToUse,
                out Vector2 localPoint);

            GameObject instance = Instantiate(
                _cursorEffectPrefab,
                _canvasRectTransform);

            RectTransform instanceRect = instance.GetComponent<RectTransform>();
            instanceRect.anchoredPosition = localPoint;
        }
    }
}
