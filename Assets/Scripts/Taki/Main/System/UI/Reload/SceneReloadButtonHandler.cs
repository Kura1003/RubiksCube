using AnnulusGames.LucidTools.Audio;
using Cysharp.Threading.Tasks;
using Taki.Main.System.RubiksCube;
using Taki.Utility;
using VContainer;

namespace Taki.Main.System
{
    public sealed class SceneReloadButtonHandler : PointerEventHandler
    {
        [Inject] private readonly ButtonEntrypoint _buttonEntrypoint;

        [Inject] private readonly ISceneReloader _sceneReloader;
        [Inject] private readonly ICubeCancellationToken _cubeCancellationToken;
        [Inject] private readonly ICubeInteractionHandler _cubeInteractionHandler;

        protected override void OnClicked()
        {
            LucidAudio.StopAll();
            _cubeCancellationToken.CancelAndDispose();
            _buttonEntrypoint.DisposeHandlers();
            _cubeInteractionHandler.UnregisterEvents();

            _sceneReloader.ReloadScene(
                destroyCancellationToken)
                .SuppressCancellationThrow()
                .Forget();
        }

        protected override void OnPointerEntered()
        {

        }

        protected override void OnPointerExited()
        {

        }
    }
}