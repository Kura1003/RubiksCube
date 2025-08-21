using Taki.Main.Data.RubiksCube;
using Taki.Main.System.RubiksCube;
using Taki.Main.View;
using Taki.Utility;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Taki.Main.System
{
    public sealed class MainSceneLifetimeScope : LifetimeScope
    {
        [SerializeField] private CircleTextRotator _circleTextRotator;

        [SerializeField] private Transform _cubePivot;
        [SerializeField] private CubeSettings _cubeSettings;
        [SerializeField] private RubiksCubeFactory _cubeFactory;
        [SerializeField] private CubeActionController _cubeActionController;

        [SerializeField] private ButtonEntrypoint _buttonEntrypoint;
        [SerializeField] private UserInputLockEntrypoint _lockEntrypoint;

        [SerializeField] private GUIScreenFader _guiScreenFader;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_circleTextRotator);

            builder.RegisterComponent(_cubeSettings);
            builder.RegisterComponent(_cubeFactory).AsImplementedInterfaces();

            var generationInfo = _cubeFactory.Create(
                _cubeSettings.PieceSpacing,
                _cubeSettings.CubeSize,
                _cubePivot);

            builder.Register<CubeController>(Lifetime.Singleton)
                .WithParameter(generationInfo.FaceManagersMap)
                .WithParameter(_cubeSettings.CubeSize)
                .AsImplementedInterfaces();

            builder.Register<RotationAxisManager>(Lifetime.Singleton)
                .WithParameter(generationInfo.AxisInfoMap)
                .WithParameter(_cubePivot)
                .AsImplementedInterfaces();

            builder.Register<RubiksCubeRotationHandler>
                (Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<RubikCubeSizeManager>
                (Lifetime.Singleton).AsImplementedInterfaces();

            builder.RegisterComponent(_cubeActionController).AsImplementedInterfaces();

            builder.RegisterComponent(_buttonEntrypoint);
            builder.RegisterComponent(_lockEntrypoint);

            builder.RegisterComponent(_guiScreenFader).AsImplementedInterfaces();
            builder.RegisterEntryPoint<GUIScreenFaderInitializer>(Lifetime.Singleton);

            builder.Register<SceneReloader>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<PauseService>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.RegisterEntryPoint<MainSceneEntrypoint>(Lifetime.Singleton);
        }
    }
}