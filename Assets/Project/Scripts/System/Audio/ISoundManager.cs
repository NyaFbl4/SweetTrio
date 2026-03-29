namespace Project.Scripts.System.Audio
{
    public interface ISoundManager
    {
        bool IsSoundEnabled { get; }
        bool IsMusicEnabled { get; }
        float SoundVolume { get; }
        float MusicVolume { get; }

        void SetSoundEnabled(bool isEnabled);
        void SetMusicEnabled(bool isEnabled);
        void SetSoundVolume(float volume);
        void SetMusicVolume(float volume);
        void PlayDessertSpawn();
        void PlayLevelFail();
        void PlayLevelWin();
        void PlayTapPick();
        void PlayComboChain();
    }
}
