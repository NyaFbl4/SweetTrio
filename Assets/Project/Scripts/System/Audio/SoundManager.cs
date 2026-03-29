using System;
using UnityEngine;
using VContainer.Unity;

namespace Project.Scripts.System.Audio
{
    public class SoundManager : IInitializable, IDisposable, ISoundManager
    {
        private const string SoundEnabledPrefsKey = "settings.sound.enabled";
        private const string MusicEnabledPrefsKey = "settings.music.enabled";
        private const int EnabledPrefsValue = 1;
        private const float DefaultMusicVolume = 0.6f;
        private const float DefaultSfxVolume = 1f;
        private const float DefaultPerClipVolume = 1f;

        private const string DessertSpawnPath = "Sounds/dessert_spawn";
        private const string LevelFailPath = "Sounds/level_fail";
        private const string LevelWinPath = "Sounds/level_win";
        private const string TapPickPath = "Sounds/tap_pick";
        private const string ComboChainPath = "Sounds/combo_chain";
        private const string BackgroundMusicPath = "Music/bgm_candy_relax";
        private const string SoundConfigPath = "Configs/SoundConfig";

        private readonly SoundConfig _soundConfig;
        private GameObject _audioRoot;
        private AudioSource _sfxSource;
        private AudioSource _musicSource;

        private AudioClip _dessertSpawn;
        private AudioClip _levelFail;
        private AudioClip _levelWin;
        private AudioClip _tapPick;
        private AudioClip _comboChain;
        private AudioClip _backgroundMusic;

        private bool _runtimeReady;
        private bool _settingsLoaded;
        private bool _clipsLoaded;
        private bool _isSoundEnabled = true;
        private bool _isMusicEnabled = true;

        public bool IsSoundEnabled
        {
            get
            {
                EnsureSettingsLoaded();
                return _isSoundEnabled;
            }
            private set => _isSoundEnabled = value;
        }

        public bool IsMusicEnabled
        {
            get
            {
                EnsureSettingsLoaded();
                return _isMusicEnabled;
            }
            private set => _isMusicEnabled = value;
        }

        public SoundManager(SoundConfig soundConfig = null)
        {
            _soundConfig = soundConfig != null ? soundConfig : Resources.Load<SoundConfig>(SoundConfigPath);
        }

        public void Initialize()
        {
            EnsureRuntimeReady();
        }

        public void Dispose()
        {
            if (_audioRoot != null)
            {
                UnityEngine.Object.Destroy(_audioRoot);
            }
        }

        public void SetSoundEnabled(bool isEnabled)
        {
            EnsureRuntimeReady();
            IsSoundEnabled = isEnabled;
            PlayerPrefs.SetInt(SoundEnabledPrefsKey, isEnabled ? EnabledPrefsValue : 0);
            PlayerPrefs.Save();
        }

        public void SetMusicEnabled(bool isEnabled)
        {
            EnsureRuntimeReady();
            IsMusicEnabled = isEnabled;
            PlayerPrefs.SetInt(MusicEnabledPrefsKey, isEnabled ? EnabledPrefsValue : 0);
            PlayerPrefs.Save();
            ApplyMusicState();
        }

        public void PlayDessertSpawn()
        {
            EnsureRuntimeReady();
            PlayOneShot(_dessertSpawn, ResolveDessertSpawnVolume());
        }

        public void PlayLevelFail()
        {
            EnsureRuntimeReady();
            PlayOneShot(_levelFail, ResolveLevelFailVolume());
        }

        public void PlayLevelWin()
        {
            EnsureRuntimeReady();
            PlayOneShot(_levelWin, ResolveLevelWinVolume());
        }

        public void PlayTapPick()
        {
            EnsureRuntimeReady();
            PlayOneShot(_tapPick, ResolveTapPickVolume());
        }

        public void PlayComboChain()
        {
            EnsureRuntimeReady();
            PlayOneShot(_comboChain, ResolveComboChainVolume());
        }

        private static AudioClip LoadClip(string path)
        {
            var clip = Resources.Load<AudioClip>(path);
            if (clip == null)
            {
                Debug.LogWarning($"SoundManager: clip '{path}' not found in Resources.");
            }

            return clip;
        }

        private void PlayOneShot(AudioClip clip, float perClipVolume)
        {
            if (!IsSoundEnabled || clip == null || _sfxSource == null)
                return;

            var volume = Mathf.Clamp01(ResolveSfxVolume() * Mathf.Max(0f, perClipVolume));
            _sfxSource.PlayOneShot(clip, volume);
        }

        private void EnsureSettingsLoaded()
        {
            if (_settingsLoaded)
                return;

            _settingsLoaded = true;
            IsSoundEnabled = PlayerPrefs.GetInt(SoundEnabledPrefsKey, EnabledPrefsValue) == EnabledPrefsValue;
            IsMusicEnabled = PlayerPrefs.GetInt(MusicEnabledPrefsKey, EnabledPrefsValue) == EnabledPrefsValue;
        }

        private void EnsureRuntimeReady()
        {
            if (_runtimeReady)
                return;

            EnsureSettingsLoaded();
            EnsureAudioSourceCreated();
            EnsureClipsLoaded();
            ApplyMusicState();
            _runtimeReady = true;
        }

        private void EnsureAudioSourceCreated()
        {
            if (_sfxSource != null && _musicSource != null)
                return;

            if (_audioRoot == null)
            {
                _audioRoot = new GameObject(nameof(SoundManager));
                UnityEngine.Object.DontDestroyOnLoad(_audioRoot);
            }

            _sfxSource = _audioRoot.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;
            _sfxSource.loop = false;
            _sfxSource.spatialBlend = 0f;

            _musicSource = _audioRoot.AddComponent<AudioSource>();
            _musicSource.playOnAwake = false;
            _musicSource.loop = true;
            _musicSource.spatialBlend = 0f;
            _musicSource.volume = ResolveMusicVolume();
        }

        private void EnsureClipsLoaded()
        {
            if (_clipsLoaded)
                return;

            _clipsLoaded = true;
            _dessertSpawn = ResolveClip(_soundConfig != null ? _soundConfig.DessertSpawn : null, DessertSpawnPath);
            _levelFail = ResolveClip(_soundConfig != null ? _soundConfig.LevelFail : null, LevelFailPath);
            _levelWin = ResolveClip(_soundConfig != null ? _soundConfig.LevelWin : null, LevelWinPath);
            _tapPick = ResolveClip(_soundConfig != null ? _soundConfig.TapPick : null, TapPickPath);
            _comboChain = ResolveClip(_soundConfig != null ? _soundConfig.ComboChain : null, ComboChainPath);
            _backgroundMusic = ResolveClip(_soundConfig != null ? _soundConfig.BackgroundMusic : null, BackgroundMusicPath);
        }

        private void ApplyMusicState()
        {
            if (_musicSource == null)
                return;

            _musicSource.volume = ResolveMusicVolume();

            if (!IsMusicEnabled || _backgroundMusic == null)
            {
                if (_musicSource.isPlaying)
                {
                    _musicSource.Stop();
                }

                return;
            }

            if (_musicSource.clip != _backgroundMusic)
            {
                _musicSource.clip = _backgroundMusic;
            }

            if (!_musicSource.isPlaying)
            {
                _musicSource.Play();
            }
        }

        private static AudioClip ResolveClip(AudioClip overrideClip, string fallbackPath)
        {
            return overrideClip != null ? overrideClip : LoadClip(fallbackPath);
        }

        private float ResolveMusicVolume()
        {
            return _soundConfig != null ? _soundConfig.MusicVolume : DefaultMusicVolume;
        }

        private float ResolveSfxVolume()
        {
            return _soundConfig != null ? _soundConfig.SfxVolume : DefaultSfxVolume;
        }

        private float ResolveDessertSpawnVolume()
        {
            return _soundConfig != null ? _soundConfig.DessertSpawnVolume : DefaultPerClipVolume;
        }

        private float ResolveLevelFailVolume()
        {
            return _soundConfig != null ? _soundConfig.LevelFailVolume : DefaultPerClipVolume;
        }

        private float ResolveLevelWinVolume()
        {
            return _soundConfig != null ? _soundConfig.LevelWinVolume : DefaultPerClipVolume;
        }

        private float ResolveTapPickVolume()
        {
            return _soundConfig != null ? _soundConfig.TapPickVolume : DefaultPerClipVolume;
        }

        private float ResolveComboChainVolume()
        {
            return _soundConfig != null ? _soundConfig.ComboChainVolume : DefaultPerClipVolume;
        }
    }
}
