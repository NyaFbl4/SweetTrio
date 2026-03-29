using UnityEngine;

namespace Project.Scripts.System.Audio
{
    [CreateAssetMenu(menuName = "Configs/Sound Config", fileName = "SoundConfig")]
    public class SoundConfig : ScriptableObject
    {
        [Header("Global Volumes")]
        [SerializeField, Range(0f, 1f)] private float _musicVolume = 0.6f;
        [SerializeField, Range(0f, 1f)] private float _sfxVolume = 1f;

        [Header("Music Clip (Optional Override)")]
        [SerializeField] private AudioClip _backgroundMusic;

        [Header("Sfx Clips (Optional Overrides)")]
        [SerializeField] private AudioClip _dessertSpawn;
        [SerializeField] private AudioClip _levelFail;
        [SerializeField] private AudioClip _levelWin;
        [SerializeField] private AudioClip _tapPick;
        [SerializeField] private AudioClip _comboChain;

        [Header("Per Sfx Multipliers")]
        [SerializeField, Range(0f, 2f)] private float _dessertSpawnVolume = 1f;
        [SerializeField, Range(0f, 2f)] private float _levelFailVolume = 1f;
        [SerializeField, Range(0f, 2f)] private float _levelWinVolume = 1f;
        [SerializeField, Range(0f, 2f)] private float _tapPickVolume = 1f;
        [SerializeField, Range(0f, 2f)] private float _comboChainVolume = 1f;

        public float MusicVolume => Mathf.Clamp01(_musicVolume);
        public float SfxVolume => Mathf.Clamp01(_sfxVolume);

        public AudioClip BackgroundMusic => _backgroundMusic;
        public AudioClip DessertSpawn => _dessertSpawn;
        public AudioClip LevelFail => _levelFail;
        public AudioClip LevelWin => _levelWin;
        public AudioClip TapPick => _tapPick;
        public AudioClip ComboChain => _comboChain;

        public float DessertSpawnVolume => Mathf.Max(0f, _dessertSpawnVolume);
        public float LevelFailVolume => Mathf.Max(0f, _levelFailVolume);
        public float LevelWinVolume => Mathf.Max(0f, _levelWinVolume);
        public float TapPickVolume => Mathf.Max(0f, _tapPickVolume);
        public float ComboChainVolume => Mathf.Max(0f, _comboChainVolume);
    }
}
