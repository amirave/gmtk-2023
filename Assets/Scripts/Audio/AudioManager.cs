using System;
using System.Collections.Generic;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioClip[] _musicTracks;
        [SerializeField] private AudioMixer _mixer;
    
        private PlayerState _playerState;
  
        private Dictionary<string, SoundEffect> _sfxClips;
        private List<AudioSource> _sfxSources;
        private List<AudioMusicTrack> _bgmTracks;
    
        private AudioMixerGroup _mixerSFX;
        private AudioMixerGroup _mixerBGM;

        void Awake()
        {
            if (Instance == null)
                Instance = this;

            _playerState = new PlayerState()
            {
                highScore = 100,
                musicVolume = 100,
                sfxVolume = 100
            };
        }

        void Start()
        {
            _mixerSFX = _mixer.FindMatchingGroups("Master/SFX")[0];
            _mixerBGM = _mixer.FindMatchingGroups("Master/Music")[0];
        
            _sfxClips = new Dictionary<string, SoundEffect>();

            _sfxSources = new List<AudioSource>();

            _bgmTracks = new List<AudioMusicTrack>();

            foreach (var clip in _musicTracks)
                CreateMusicTrack(clip);
        
            LoadSounds();

            ToggleChannelInternal(AudioChannel.Music, _playerState.musicVolume, false);
            ToggleChannelInternal(AudioChannel.Fx, _playerState.sfxVolume, false);
        }
    
        private void LoadSounds()
        {
            foreach (var sfx in Resources.FindObjectsOfTypeAll<SoundEffect>())
            {
                print(sfx.name);
                var sfxId = sfx.name;
                _sfxClips.Add(sfxId, sfx);
            }
        }

        private void CreateMusicTrack(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning("[AudioSystem] trying to create a music track from an empty clip");
                return;
            }

            var obj = new GameObject("BGM_" + clip.name, typeof(AudioSource), typeof(AudioMusicTrack));
            obj.transform.SetParent(transform);

            var src = obj.GetComponent<AudioSource>();
            src.clip = clip;

            var track = obj.GetComponent<AudioMusicTrack>();
            track.Initialize(_mixerBGM);
        
            _bgmTracks.Add(track);
        }
    
        public void PlayMusicTrack(string id)
        {
            var found = false;
            
            foreach (var track in _bgmTracks)
            {
                if (track.id.Equals(id))
                {
                    track.FadeIn();
                    found = true;
                }
                else
                {
                    track.FadeOut();
                }
            }
            
            if ( found == false )
                Debug.LogWarning($"[{typeof(AudioManager)}] Could not play music track because track was not found - {id}");
        }
    
        public void PlayEffect(string id)
        {
            if (_sfxClips.ContainsKey(id) == false)
            {
                Debug.LogWarning($"[{typeof(AudioManager)}] Could not play effect because clip was not found - {id}");
                return;
            }

            var sfx = _sfxClips[id];
            var src = GetOrCreateSFXAudioSource();

            sfx.Play(src);
        }
    
        public async UniTask PlayEffectWithDelay(string id, float delay, CancellationToken cancellationToken)
        {
            var wasCancelled = await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: cancellationToken)
                .SuppressCancellationThrow();

            if (wasCancelled == true)
                return;

            PlayEffect(id);
        }
        
        private AudioSource GetOrCreateSFXAudioSource()
        {
            // Try reusing existing source
            for (var i = 0; i < _sfxSources.Count; i++)
            {
                var source = _sfxSources[i];
                if (source.isPlaying == false)
                    return source;
            }
            
            // Create new source
            var obj = new GameObject($"Voice{_sfxSources.Count + 1}", typeof(AudioSource));
            obj.transform.SetParent(transform);

            var src = obj.GetComponent<AudioSource>();

            src.playOnAwake = false;
            src.clip = null;
            src.loop = false;
            src.spatialize = false;
            src.outputAudioMixerGroup = _mixerSFX;
            
            _sfxSources.Add(src);
            return src;
        }

        public void ToggleChannelInternal(AudioChannel channel, float value, bool writeToProfile)
        {
            var paramName = channel == AudioChannel.Fx
                ? AudioNames.MIXER_PARAM_VOLUME_SFX
                : AudioNames.MIXER_PARAM_VOLUME_BGM;

            var paramValue = Mathf.Lerp(MagicNumbers.SOUND_DISABLED_VOLUME_DB, MagicNumbers.SOUND_ENABLED_VOLUME_DB, value);

            _mixer.SetFloat(paramName, paramValue);

            if (writeToProfile)
            {
                _playerState.musicVolume = GetChannelActive(AudioChannel.Music);
                _playerState.sfxVolume = GetChannelActive(AudioChannel.Fx);
            }
        }
    
        public float GetChannelActive(AudioChannel channel)
        {
            var paramName = channel == AudioChannel.Fx
                ? AudioNames.MIXER_PARAM_VOLUME_SFX
                : AudioNames.MIXER_PARAM_VOLUME_BGM;

            if (_mixer.GetFloat(paramName, out var value) == false)
                return 0;
            
            return value;
        }
    
        public void SetChannelActive(AudioChannel channel)
        {
            ToggleChannelInternal(channel, channel == AudioChannel.Fx ? _playerState.sfxVolume : _playerState.musicVolume, false);
        }
    }
}
