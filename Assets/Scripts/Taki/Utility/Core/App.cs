using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Taki.Utility.Core
{
    internal static class App
    {
        internal static void Exit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#elif !UNITY_WEBGL 
            Application.Quit();
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
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
        }
    }
}