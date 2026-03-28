using System;
using Project.Scripts.Systems.UI;

namespace Project.Scripts.UI.RulesUI
{
    public interface IRulesUIView : ILayoutView
    {
        event Action CloseClicked;
        void SetRulesScoreThresholds(int oneStarScore, int twoStarsScore, int threeStarsScore);
    }
}
