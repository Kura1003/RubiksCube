using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using R3;
using Taki.Audio;
using Taki.Main.Data.RubiksCube;
using Taki.Main.View;
using Taki.Utility;
using UnityEngine;
using VContainer;

namespace Taki.Main.System.RubiksCube
{
    public sealed class CubeActionController : MonoBehaviour, ICubeInteractionHandler
    {
        [SerializeField] private Transform _cubePivot;
        [SerializeField] private CameraShaker _cameraShaker;

        [SerializeField] private float _fastRotationDuration = 0.1f;
        [SerializeField] private int _shuffleCount = 3;
        [SerializeField] private int _fastShuffleCount = 50;

        [Serializable]
        private struct TaggedProvider
        {
            public CubeActionTag ActionTag;
            public BasePointerEventProvider Provider;
        }

        [SerializeField] private List<TaggedProvider> _taggedProviders = new();

        [Inject] private readonly CubeSettings _cubeSettings;

        [Inject] private readonly IRubiksCubeRotator _cubeRotator;
        [Inject] private readonly ICubeFactory _cubeFactory;
        [Inject] private readonly ICubeDataProvider _cubeDataProvider;
        [Inject] private readonly IRotationAxisProvider _rotationAxisProvider;
        [Inject] private readonly ICubeStateRestorer _cubeStateRestorer;
        [Inject] private readonly ICubeCancellationToken _cubeCancellationToken;

        private Dictionary<CubeActionTag, ICubeActionHandler> _actionHandlers;
        private bool _isTaskRunning = false;

        private CompositeDisposable _disposables = new();

        private void Awake()
        {
            CreateActionHandlers();
        }

        private void OnDestroy()
        {
            _disposables.Dispose();

            if (_actionHandlers != null)
            {
                _actionHandlers.Values
                    .ToList()
                    .ForEach(handler => handler.Dispose());

                _actionHandlers.Clear();
            }
        }

        private void CreateActionHandlers()
        {
            _actionHandlers = new Dictionary<CubeActionTag, ICubeActionHandler>
            {
                {
                    CubeActionTag.Shuffle,
                    new CubeShuffler(
                        _shuffleCount,
                        _cubeRotator)
                },
                {
                    CubeActionTag.FastShuffle,
                    new FastCubeShuffler(
                        _fastShuffleCount,
                        _cubeRotator,
                        _cubeStateRestorer)
                },
                {
                    CubeActionTag.Rebuild,
                    new CubeRebuilder(
                        _cubePivot,
                        _cameraShaker,
                        _cubeSettings,
                        _cubeFactory,
                        _cubeDataProvider,
                        _rotationAxisProvider)
                },
                {
                    CubeActionTag.Restore,
                    new CubeRestorer(
                        _cubeRotator)
                },
                {
                    CubeActionTag.FastRestore,
                    new FastCubeRestorer(
                        _cubeSettings,
                        _fastRotationDuration,
                        _cubeRotator)
                },
                {
                    CubeActionTag.SlideRotate,
                    new SlideRotator(
                        _cubeSettings,
                        _cubeRotator,
                        _cubeFactory,
                        _cubeCancellationToken)
                }
            };
        }

        public void RegisterEvents()
        {
            _taggedProviders
                .Where(tp => _actionHandlers.ContainsKey(tp.ActionTag))
                .ToList()
                .ForEach(tp =>
                {
                    _actionHandlers.TryGetValue(tp.ActionTag, out var handler);
                    tp.Provider.OnClicked
                        .Subscribe(_ =>
                        {
                            ExecuteActionTask(handler)
                            .SuppressCancellationThrow()
                            .Forget();
                        })
                        .AddTo(_disposables);

                    Debug.Log($"タグ: {tp.ActionTag} のアクションを " +
                              $"{tp.Provider.gameObject.name} に登録しました。");
                });
        }

        public void UnregisterEvents()
        {
            _disposables.Dispose();
            Debug.Log("すべてのキューブイベントの登録を解除しました。");
        }

        public UniTask ExecuteRebuild()
        {
            if (_actionHandlers.TryGetValue(CubeActionTag.Rebuild, out var handler))
            {
                return ExecuteActionTask(handler);
            }

            return UniTask.CompletedTask;
        }

        public UniTask ExecuteFastShuffle()
        {
            if (_actionHandlers.TryGetValue(CubeActionTag.FastShuffle, out var handler))
            {
                return ExecuteActionTask(handler);
            }

            return UniTask.CompletedTask;
        }

        private async UniTask ExecuteActionTask(ICubeActionHandler handler)
        {
            if (!(handler is CubeRebuilder) && _isTaskRunning)
            {
                AudioManager.Instance.Play("Block", gameObject).SetVolume(1f);
                return;
            }

            if(!(handler is CubeRebuilder))
            {
                AudioManager.Instance.Play("Click", gameObject).SetVolume(0.5f);
            }

            _isTaskRunning = true;
            await handler.Execute();
            _isTaskRunning = false;
        }
    }
}