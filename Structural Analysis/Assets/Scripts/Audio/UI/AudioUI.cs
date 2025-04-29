using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioUI : MonoBehaviour
{
    protected AudioSource _audio;
    protected virtual void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _audio.playOnAwake = false;
    }
    protected void AudioPlay()
    {
        if (gameObject.activeInHierarchy)
        {
            _audio.Play();
            return;
        }
        GameObject clone = new ();
        AudioSource soundClone = clone.AddComponent<AudioSource>();
        soundClone.playOnAwake = false;
        soundClone.outputAudioMixerGroup = _audio.outputAudioMixerGroup;
        soundClone.clip = _audio.clip;
        soundClone.volume = _audio.volume;
        soundClone.Play();
        Destroy(clone, soundClone.clip.length);
    }
}
