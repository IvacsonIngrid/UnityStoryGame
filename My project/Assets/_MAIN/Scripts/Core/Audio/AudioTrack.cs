using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Audio;

public class AudioTrack
{
    public const string TRACK_NAME_FORMAT = "Track - [{0}]"; // igy jelenik meg a Unity Editor-ban a hierarchia rendszerben
    public string name {  get; private set; } // zenefájl neve
    public string path { get; private set; } // zenefájl elérési útvonala

    public GameObject root => source.gameObject;

    private AudioChannel channel;
    private AudioSource source; // lehetővé teszi hangok lejátszását
    public bool loop => source.loop; // ismétlődő lejátszás
    public float volumeCap { get; private set; } // max hangerő szintje

    public float pitch { get { return source.pitch; } set { source.pitch = value; } } // hangmagasság

    public bool isPlaying => source.isPlaying; // ellenőrzi, lejátszás alatt van-e a hanganyag

    public float volume { get { return source.volume; } set { source.volume = value; } } // hankerő szintje

    public AudioTrack(AudioClip clip, bool loop, float startingVolume, float volumeCap, float pitch, AudioChannel channel, AudioMixerGroup mixer, string filePath)
    {
        // inicializálás: zene neve, elérési útja
        name = clip.name;
        path = filePath;
        this.channel = channel;
        this.volumeCap = volumeCap;

        // AudioSource létrehozása, tulajdonságainak beállitása: 
        source = CreateSource();
        source.clip = clip;
        source.loop = loop;
        source.volume = startingVolume;
        source.pitch = pitch;
        source.outputAudioMixerGroup = mixer;
        this.path = path;
    }

    // AudioSource létrehozása
    private AudioSource CreateSource()
    {
        GameObject go = new GameObject(string.Format(TRACK_NAME_FORMAT, name));
        go.transform.SetParent(channel.trackContainer); // lejátszási csatornához rendeli
        AudioSource source = go.AddComponent<AudioSource>(); // hozzáadás a környezethez is a megadott név alatt

        return source;
    }

    // lejátsza a zenét
    public void Play()
    {
        source.Play();
    }

    // leállitja a zenét
    public void Stop()
    {
        source.Stop();
    }
}
