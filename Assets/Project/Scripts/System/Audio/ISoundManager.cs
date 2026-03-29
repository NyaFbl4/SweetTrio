namespace Project.Scripts.System.Audio
{
    public interface ISoundManager
    {
        bool IsSoundEnabled { get; }
        bool IsMusicEnabled { get; }

        void SetSoundEnabled(bool isEnabled);
        void SetMusicEnabled(bool isEnabled);
        void PlayDessertSpawn();
        void PlayLevelFail();
        void PlayLevelWin();
        void PlayTapPick();
        void PlayComboChain();
    }
}
