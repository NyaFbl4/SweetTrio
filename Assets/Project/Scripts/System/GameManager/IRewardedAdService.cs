using System;

namespace Project.Scripts.GameManager
{
    public interface IRewardedAdService
    {
        bool IsShowing { get; }
        void Show(string rewardId, Action onRewarded, Action onFailedOrClosed);
    }
}