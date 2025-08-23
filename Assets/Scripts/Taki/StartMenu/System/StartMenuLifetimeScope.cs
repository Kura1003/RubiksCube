using Taki.Main.System;
using Taki.Main.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Taki.StartMenu.System
{
    public sealed class StartMenuLifetimeScope : LifetimeScope
    {
        [SerializeField] private GUIScreenFader _guiScreenFader;
        [SerializeField] private TextTyper _titleTextTyper;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_guiScreenFader).AsImplementedInterfaces();
            builder.RegisterEntryPoint<GUIScreenFaderInitializer>(Lifetime.Singleton);

            builder.RegisterComponent(_titleTextTyper);

            builder.Register<MainSceneLoader>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.RegisterEntryPoint<StartMenuEntrypoint>(Lifetime.Singleton);
        }
    }
}
