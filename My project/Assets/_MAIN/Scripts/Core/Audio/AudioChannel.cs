using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioChannel
{
    private const string TRACK_CONTAINER_NAME_FORMAT = "Channel - [{0}]"; // csatorna megjelenik a környezetben
    public int channelIndex {  get; private set; } // csatorna indexe
    public Transform trackContainer { get; private set; } = null; // referencia, amely tartalmazza a zeneszámokat

    public AudioTrack activeTrack { get; private set; } = null; // aktiv, lejátszás alatti zene

    private List<AudioTrack> tracks = new List<AudioTrack>(); // összes lejátszási zenét tartalmazza

    bool isLevelingVolume => co_volumeLeveling != null; // hangerő szintézis aktiv-e
    Coroutine co_volumeLeveling = null; // hangerőért felel
    public AudioChannel(int channel)
    {
        channelIndex = channel; // csatorna indexe

        trackContainer = new GameObject(string.Format(TRACK_CONTAINER_NAME_FORMAT, channel)).transform;
        trackContainer.SetParent(AudioManager.instance.transform); // szülőnek állitja be az AudioManager példányát
    }

    public AudioTrack PlayTrack(AudioClip clip, bool loop, float startingVolume, float volumeCap, float pitch, string filePath)
    {
        if (TryGetTrack(clip.name, out AudioTrack existingTrack)) // ha létezik a zeneszám
        {
            if (!existingTrack.isPlaying) // ha még nincs lejátszás alatt
                existingTrack.Play(); // lejátsza

            SetAsActiveTrack(existingTrack);

            return existingTrack;
        }

        // létrehozza, mivel nem létezett, és lejátsza
        AudioTrack track = new AudioTrack(clip, loop, startingVolume, volumeCap, pitch, this, AudioManager.instance.musicMixer, filePath);
        track.Play();

        SetAsActiveTrack(track);

        return track;
    }

    // a zeneszám kereséséhez - ez keresi a listában a név alapján
    public bool TryGetTrack(string trackName, out AudioTrack value)
    {
        trackName = trackName.ToLower();

        foreach(var track in tracks)
        {
            if (track.name.ToLower() == trackName)
            {
                value = track;
                return true;
            }
        }

        value = null;
        return false;
    }

    // aktivvá alakitja a zenét
    private void SetAsActiveTrack(AudioTrack track)
    {
        if (!tracks.Contains(track)) // hozzáadja, ha még nem része
            tracks.Add(track);

        activeTrack = track; // aktivvá teszi

        TryStartVolumeLeveling();
    }

    // hangerő szintézis elinditása
    private void TryStartVolumeLeveling()
    {
        if (!isLevelingVolume)
            co_volumeLeveling = AudioManager.instance.StartCoroutine(VolumeLeveling());
    }

    private IEnumerator VolumeLeveling()
    {
        while ((activeTrack != null && (tracks.Count > 1 || activeTrack.volume != activeTrack.volumeCap)) || (activeTrack == null && tracks.Count > 0))
        {
            for (int i = tracks.Count - 1; i >= 0; i--) // hangerő szintézis minden dal esetén
            {
                AudioTrack track = tracks[i];
                float targetVolume = activeTrack == track ? track.volumeCap : 0;

                if (track == activeTrack && track.volume == targetVolume) 
                    continue;

                track.volume = Mathf.MoveTowards(track.volume, targetVolume, AudioManager.TRACK_TRANSITION_SPEED * Time.deltaTime);

                if (track != activeTrack && track.volume == 0) // ha a hangerő nem aktiv, hangerő 0, akkor automatikusan törli 
                {
                    DestroyTrack(track);
                }
            }

            yield return null;
        }

        co_volumeLeveling = null;
    }

    // törölni zenét a listából
    private void DestroyTrack(AudioTrack track)
    {
        if (tracks.Contains(track))
            tracks.Remove(track);

        Object.Destroy(track.root);
    }

    // zeneszám leállitása
    public void StopTrack(bool immediate = false)
    {
        if (activeTrack == null)
            return;

        if (immediate) // azonnali leállitás
        {
            DestroyTrack(activeTrack);
            activeTrack = null;
        }
        else // hangerő szintézis elinditása
        {
            activeTrack = null;
            TryStartVolumeLeveling();
        }
    }
}
