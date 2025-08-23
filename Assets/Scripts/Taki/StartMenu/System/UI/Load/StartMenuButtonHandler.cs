using AnnulusGames.LucidTools.Audio;
using VContainer;
using Taki.Utility;
using Cysharp.Threading.Tasks;
using Taki.Audio;

namespace Taki.StartMenu.System
{
    public sealed class StartMenuButtonHandler : PointerEventHandler
    {
        [Inject] private readonly IMainSceneLoader _sceneLoader;
        private bool _isClicked = false;

        protected override void OnClicked()
        {
            if (_isClicked || !_sceneLoader.IsPreloaded)
            {
                AudioManager
                    .Instance
                    .Play(
                    "Block",
                    gameObject)
                    .SetVolume(1f);

                return;
            }

            AudioManager
                .Instance
                .Play(
                "Click",
                gameObject)
                .SetVolume(0.5f);

            _isClicked = true;
            LucidAudio.StopAll();

            _sceneLoader
                .LoadMainScene(
                    destroyCancellationToken)
                .SuppressCancellationThrow()
                .Forget();
        }

        protected override void OnPointerEntered() { }

        protected override void OnPointerExited() { }
    }
}
