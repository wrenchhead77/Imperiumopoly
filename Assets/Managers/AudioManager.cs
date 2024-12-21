using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public enum eMixers {  music, effects}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [NamedArray(typeof(eMixers))] public AudioMixerGroup[] mixers;
    [NamedArray(typeof(eMixers))] public float[] volume = { 0f, 0f };
    [NamedArray(typeof(eMixers))] public string[] strMixers = { "MusicVol", "EffectsVol" };

    [SerializeField] private AudioSource BGM;
    [SerializeField] private AudioSource effects;


    private void Awake()
    {
        Instance = this;
    }

    public void SetMixerLevel (eMixers _mixer, float _soundLevel)
    {
        mixers[(int)_mixer].audioMixer.SetFloat(strMixers[(int)_mixer], Mathf.Log10(_soundLevel)* 20f);
        volume[(int)_mixer] = _soundLevel;
    }

}
