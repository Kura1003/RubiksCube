using UnityEngine;

namespace Taki.Utility.Core
{
    internal static class App
    {
        internal static void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            const int screenWidth = 1248;
            const int screenHeight = 702;
            const bool isFullScreen = false;
            const int targetFrameRate = 120;
            const bool runInBackground = true;
            const bool logEnabled = false;

            if (!Application.isEditor)
            {
                Debug.unityLogger.logEnabled = logEnabled;
            }

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = targetFrameRate;
            Application.runInBackground = runInBackground;

            Screen.SetResolution(screenWidth, screenHeight, isFullScreen);
        }
    }
}