using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public const string SFX_PARENT_NAME = "SFX"; // effektus hangok szülő obj.
    private const string SFX_NAME_FORMAT = "SFX - [{0}]"; // hang effektusok nevéhez sablon
    public const float TRACK_TRANSITION_SPEED = 1f; // csatornák közötti átmenet sebessége
    public static AudioManager instance {  get; private set; } // példány Singleton minta alapján

    public Dictionary <int, AudioChannel> channels = new Dictionary<int, AudioChannel>(); // csatornák tárolésa

    // különböző hangok keveréséért felel
    public AudioMixerGroup musicMixer;
    public AudioMixerGroup soundFixMixer;
    public AudioMixerGroup voicesMixer;

    private Transform sfxRoot;

    private void Awake()
    {
        if (instance == null)
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject); // ne álljon le váltáskor
            instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }

        sfxRoot = new GameObject(SFX_PARENT_NAME).transform;
        sfxRoot.SetParent(transform);
    }

    //  útvonal alapján keres, és lejátssza az effektust
    public AudioSource PlaySoundEffect(string filePath, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false)
    {
        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if (clip == null)
        {
            Debug.LogError($"Could not load audio file '{filePath}'");
            return null;
        }

        return PlaySoundEffect(clip, mixer, volume, pitch, loop);
    }

    // közvetlezül megkapja, létrehoz egy új elemet
    public AudioSource PlaySoundEffect(AudioClip clip, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false)
    {
        AudioSource effectSource = new GameObject(string.Format(SFX_NAME_FORMAT, clip.name)).AddComponent<AudioSource>();
        effectSource.transform.SetParent(sfxRoot);
        effectSource.transform.position = sfxRoot.position;

        effectSource.clip = clip;

        if (mixer == null)
            mixer = soundFixMixer;

        // paraméterek beállitása
        effectSource.outputAudioMixerGroup = mixer;
        effectSource.volume = volume;
        effectSource.spatialBlend = 0;
        effectSource.pitch = pitch;
        effectSource.loop = loop;

        // lejátszás
        effectSource.Play();

        // ha nem kell ismételni, akkor megszünteti
        if (!loop)
            Destroy(effectSource.gameObject, (clip.length / pitch) + 1);

        return effectSource;
    }

    // zene lejátszása, ha az útvonala ismert
    public AudioSource PlayVoice(string filePath, float volume = 1, float pitch = 1, bool loop = false)
    {
        return PlaySoundEffect(filePath, voicesMixer, volume, pitch, loop);
    }

    // zene lejátszása, ha közvetlen az elérés
    public AudioSource PlayVoice(AudioClip clip, float volume = 1, float pitch = 1, bool loop = false)
    {
        return PlaySoundEffect(clip, voicesMixer, volume, pitch, loop);
    }

    // leállitása
    public void StopSoundEffect(AudioClip clip) => StopSoundEffect(clip.name);  

    public void StopSoundEffect(string soundName)
    {
        soundName = soundName.ToLower();
        AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();

        foreach(var source in sources)
        {
            if (source.clip.name.ToLower() == soundName) // azonosit
            {
                Destroy(source.gameObject); // töröl
                return;
            }
        }
    }

    // útvonallal dolgozik, lejátsza a dalt
    public AudioTrack PlayTrack(string filePath, int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1f, float pitch = 1f)
    {
        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if (clip == null)
        {
            Debug.LogError($"Could not load audio file '{filePath}'.");
            return null;
        }

        return PlayTrack(clip, channel, loop, startingVolume, volumeCap, pitch, filePath);
    }

    // közvetlenül eléri, csatornát hoz létre és lejátsza
    public AudioTrack PlayTrack(AudioClip clip, int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1f, float pitch = 1f, string filePath = "")
    {
        AudioChannel audioChannel = TryGetChannel(channel, createIfDoesNotExist: true);
        AudioTrack track = audioChannel.PlayTrack(clip, loop, startingVolume, volumeCap, pitch, filePath);
        return null;
    }

    // hangsáv leállitása adott csatornán
    public void StopTrack(int channel)
    {
        AudioChannel aChannel = TryGetChannel(channel, createIfDoesNotExist: false);

        if (aChannel == null)
            return;

        aChannel.StopTrack();
    }

    // név alapján is keres
    public void StopTrack(string trackName)
    {
        trackName = trackName.ToLower();

        foreach(var channel in channels.Values)
        {
            if (channel.activeTrack != null && channel.activeTrack.name.ToLower() == trackName)
            {
                channel.StopTrack();
                return;
            }
        }
    }

    // lekéri az adott csatornát szám alapján azonositva
    public AudioChannel TryGetChannel(int channelNumber, bool createIfDoesNotExist =  false)
    {
        AudioChannel channel = null;

        if (channels.TryGetValue(channelNumber, out channel))
        {
            return channel;
        }
        else if (createIfDoesNotExist) // ha nem találta me, létrehoz egy újat
        {
            channel = new AudioChannel(channelNumber);
            channels.Add(channelNumber, channel);

            return channel;
        }

        return null;
    }
}
