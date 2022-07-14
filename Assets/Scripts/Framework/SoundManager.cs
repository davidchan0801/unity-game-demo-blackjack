using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    static public SoundManager Instance;

    [Serializable]
    public class SoundEntry
    {
        public string name;
        public AudioClip clip;
    }
    [SerializeField] protected SoundEntry[] sounds;
    [SerializeField] protected AudioSource[] channel1AudioSources;
    [SerializeField] protected AudioSource[] channel2AudioSources;
    [SerializeField] protected AudioSource[] specficAudioSources;
    [SerializeField] protected AudioSource bgm;

    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
    private int channel1Index;
    private int channel2Index;

    private bool bgmMuted = false;
    private bool sfxMuted = false;
    private float volume = 1.0f;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        foreach (SoundEntry entry in sounds)
        {
            audioClips.Add(entry.name, entry.clip);
        }
        bgmMuted = PlayerPrefs.GetInt("bgmmuted") == 1;
        sfxMuted = PlayerPrefs.GetInt("sfxmuted") == 1;
        volume = PlayerPrefs.GetFloat("float", 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsBGMMuted {
        get {
            return bgmMuted;
        }
        set {
            bgmMuted = value;
            PlayerPrefs.SetInt("bgmmuted", value?1:0);
            if (bgmMuted)
                SoundManager.Instance.StopBGMMusic();
            else
                SoundManager.Instance.PlayBGMMusic();
        }
    }

    public bool IsSFXMuted {
        get {
            return sfxMuted;
        }
        set {
            sfxMuted = value;
            PlayerPrefs.SetInt("sfxmuted", value?1:0);
        }
    }

    public float Volume {
        get {
            return volume;
        }
        set {
            volume = value;
            PlayerPrefs.SetFloat("volume", value);
            bgm.volume = volume;
        }
    }

    public void Play(int channel, string sfxName)
    {
        AudioSource[] channelAudioSources = null;
        int channelIndex = -1;
        switch (channel)
        {
            case 0:
                channelAudioSources = channel1AudioSources;
                channelIndex = channel1Index;
                break;
            case 1:
                channelAudioSources = channel2AudioSources;
                channelIndex = channel2Index;
                break;
        }
            
        if (!sfxMuted) {
            if (audioClips.ContainsKey(sfxName))
            {
                channelAudioSources[channelIndex].clip = audioClips[sfxName];
                channelAudioSources[channelIndex].Play();
                channelAudioSources[channelIndex].volume = volume;
                channelIndex++;
                if (channelIndex >= channelAudioSources.Length)
                    channelIndex = 0;
            }
            else
            {
                AudioClip clip = Resources.Load<AudioClip>("SFX/" + sfxName);
                if (clip != null) {
                    audioClips.Add(sfxName, clip);
                    Play(channelIndex, sfxName);
                }
                else
                    Debug.Log("Can not add " + sfxName);
            }
        }
        switch (channel)
        {
            case 0:
                channel1Index = channelIndex;
                break;
            case 1:
                channel2Index = channelIndex;
                break;
        }
    }

    public void PlaySpecficChannel(int index, string sfxName)
    {
        if (audioClips.ContainsKey(sfxName))
        {
            specficAudioSources[index].clip = audioClips[sfxName];
            specficAudioSources[index].volume = volume;
            specficAudioSources[index].Play();
        }
        else
        {
            AudioClip clip = Resources.Load<AudioClip>("SFX/" + sfxName);
            if (clip != null)
            {
                audioClips.Add(sfxName, clip);
                PlaySpecficChannel(index, sfxName);
            }
            else
                Debug.Log("Can not add " + sfxName);
        }
    }

    public bool IsSpecficChannelPlaying(int index)
    {
        return specficAudioSources[index].isPlaying;
    }

    public void PlayBGMMusic(string sfxName) {
        if (!bgmMuted) {
            if (audioClips.ContainsKey(sfxName)) {
                bgm.clip = audioClips[sfxName];
                bgm.volume = volume;
                bgm.Play();
            }
        }
    }

    public void PlayBGMMusic() {
        bgm.Play();
    }

    public void StopBGMMusic() {
        bgm.Stop();
    }
}
