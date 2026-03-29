using System;
using System.Reflection;
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
            if (showMethod == null)
            {
                LogMissingInterstitialMethodOnce();
                return;
            }

            try
            {
                showMethod.Invoke(null, null);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"YandexLevelStartAdService: failed to show interstitial ad. {exception.Message}");
            }
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
            Debug.LogWarning("YandexLevelStartAdService: Interstitial module is not enabled. Enable interstitial ads in PluginYG2.");
        }
#endif
    }
}
