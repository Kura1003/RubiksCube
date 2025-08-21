using R3;
using UnityEngine;
using Taki.Utility.Core;

namespace Taki.Utility
{
    [RequireComponent(typeof(BasePointerEventProvider))]
    public abstract class PointerEventHandler : MonoBehaviour
    {
        [SerializeField] private bool _autoInitializeOnAwake = false;

        private IPointerEvents _pointerEvents;
        private CompositeDisposable _disposables;

        protected virtual void Awake()
        {
            if (_autoInitializeOnAwake)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            _pointerEvents = GetComponent<IPointerEvents>();
            Thrower.IfNull(_pointerEvents, nameof(_pointerEvents));

            _disposables = new CompositeDisposable();

            _pointerEvents.OnClicked
                .Subscribe(_ => OnClicked())
                .AddTo(_disposables);

            _pointerEvents.OnPointerEntered
                .Subscribe(_ => OnPointerEntered())
                .AddTo(_disposables);

            _pointerEvents.OnPointerExited
                .Subscribe(_ => OnPointerExited())
                .AddTo(_disposables);

            Debug.Log(
                $"{gameObject.name} �ɃA�^�b�`���ꂽ " +
                $"{GetType().Name} �̏��������������܂����B");
        }

        protected virtual void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }

        protected abstract void OnClicked();
        protected abstract void OnPointerEntered();
        protected abstract void OnPointerExited();
    }
}