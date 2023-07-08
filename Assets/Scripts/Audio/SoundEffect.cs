using UnityEngine;

namespace Audio
{
    [CreateAssetMenu()]
    public class SoundEffect : ScriptableObject
    {
        public AudioClip[] sfx;

        [HideInInspector] public float minVolume = 1;
        [HideInInspector] public float maxVolume = 1;
    
        [HideInInspector] public float minPitch = 1;
        [HideInInspector] public float maxPitch = 1;

        public void Play(AudioSource source)
        {
            if (sfx.Length == 0) return;

            source.clip = sfx.PickRandom();
            source.volume = Random.Range(minVolume, maxVolume);
            source.pitch = Random.Range(minPitch, maxPitch);
            // source.outputAudioMixerGroup = audioOutput;
            source.Play();
        }
    }
}
