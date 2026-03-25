using System;
using Project.Scripts.Systems.UI;
using UnityEngine;

namespace Project.Scripts.UI.LevelUI
{
    public interface ILevelUIView : ILayoutView
    {
        event Action ShuffleButtonClicked;
        event Action ExitToMenuClicked;
        event Action PauseButtonClicked;

        void SetCounter(int value);
        void SetCounterText(string text);
        void SetTotalDessertsText(string text);
        void SetTimerText(string text);
        void SetProgress(float value01);
        void SetBonusDessertSprite(Sprite sprite);
        void SetBonusMultiplierText(string text);
    }
}
