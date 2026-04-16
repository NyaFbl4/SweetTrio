using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
#if PLUGIN_YG_2
using YG;
#endif

namespace Project.Scripts.System.Ads
{
    public class YandexLevelStartAdService : ILevelStartAdService
    {
        private const string InterstitialMethodName = "InterstitialAdvShow";

        private MethodInfo _showInterstitialMethod;
        private bool _methodResolved;
        private bool _isMissingMethodLogged;

        public void ShowLevelStartAd()
        {
#if PLUGIN_YG_2
            var showMethod = ResolveShowMethod();
            if (showMethod != null)
            {
                try
                {
                    showMethod.Invoke(null, null);
                    return;
                }
                catch (Exception exception)
                {
                    Debug.LogWarning($"YandexLevelStartAdService: failed to show interstitial ad via PluginYG2 module. {exception.Message}");
                }
            }

            if (ProjectInterstitialBridge.TryShowInterstitial())
                return;

            if (showMethod == null)
            {
                LogMissingInterstitialMethodOnce();
                return;
            }

            Debug.LogWarning("YandexLevelStartAdService: failed to show interstitial ad via both PluginYG2 module and direct Yandex SDK bridge.");
#endif
        }

#if PLUGIN_YG_2
        private MethodInfo ResolveShowMethod()
        {
            if (_methodResolved)
                return _showInterstitialMethod;

            _methodResolved = true;
            _showInterstitialMethod = typeof(YG2).GetMethod(InterstitialMethodName, BindingFlags.Public | BindingFlags.Static);
            return _showInterstitialMethod;
        }

        private void LogMissingInterstitialMethodOnce()
        {
            if (_isMissingMethodLogged)
                return;

            _isMissingMethodLogged = true;
            Debug.LogWarning("YandexLevelStartAdService: Interstitial module is not enabled. Using direct Yandex SDK fallback bridge.");
        }
#endif

        private static class ProjectInterstitialBridge
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            [DllImport("__Internal")]
            private static extern int Project_ShowInterstitialAdv();
#endif

            public static bool TryShowInterstitial()
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                try
                {
                    return Project_ShowInterstitialAdv() == 1;
                }
                catch (Exception exception)
                {
                    Debug.LogWarning($"YandexLevelStartAdService: failed to call direct interstitial bridge. {exception.Message}");
                    return false;
                }
#else
                return false;
#endif
            }
        }
    }
}
