using System;
using YG;

namespace Project.Scripts.GameManager
{
    public class YandexRewardedAdService : IRewardedAdService, IDisposable
    {
        private string _pendingRewardId;
        private Action _onRewarded;
        private Action _onFailedOrClosed;

        public bool IsShowing { get; private set; }

        public void Show(string rewardId, Action onRewarded, Action onFailedOrClosed)
        {
            if (IsShowing)
                return;

            IsShowing = true;
            _pendingRewardId = rewardId;
            _onRewarded = onRewarded;
            _onFailedOrClosed = onFailedOrClosed;


            YG2.onRewardAdv += HandleReward;
            YG2.onCloseRewardedAdv += HandleClosed;
            YG2.onErrorRewardedAdv += HandleError;
            YG2.RewardedAdvShow(rewardId);
        }

        private void HandleReward(string rewardId)
        {
            if (rewardId != _pendingRewardId)
                return;

            var callback = _onRewarded;
            Cleanup();
            callback?.Invoke();
        }

        private void HandleClosed()
        {
            CompleteWithoutReward();
        }

        private void HandleError()
        {
            CompleteWithoutReward();
        }

        private void CompleteWithoutReward()
        {
            var callback = _onFailedOrClosed;
            Cleanup();
            callback?.Invoke();
        }

        private void Cleanup()
        {
#if PLUGIN_YG_2 && RewardedAdv_yg
        YG2.onRewardAdv -= HandleReward;
        YG2.onCloseRewardedAdv -= HandleClosed;
        YG2.onErrorRewardedAdv -= HandleError;
#endif
            IsShowing = false;
            _pendingRewardId = null;
            _onRewarded = null;
            _onFailedOrClosed = null;
        }

        public void Dispose()
        {
            Cleanup();
        }
    }
}