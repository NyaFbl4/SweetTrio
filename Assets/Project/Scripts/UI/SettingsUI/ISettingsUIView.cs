using System;
using Project.Scripts.Systems.UI;

namespace Project.Scripts.UI.SettingsUI
{
    public interface ISettingsUIView : ILayoutView
    {
        event Action CloseClicked;
        event Action MusicToggleClicked;
        event Action SoundToggleClicked;

        void SetMusicEnabled(bool isEnabled);
        void SetSoundEnabled(bool isEnabled);
    }
}
