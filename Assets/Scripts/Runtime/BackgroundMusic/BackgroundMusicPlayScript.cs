using Assets.Scripts.Runtime.Logic;
using Normal.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackgroundMusicPlayScript : RealtimeComponent<BackgroundMusicPlayModel>
{
    [SerializeField]
    private BackgroundMusicSetSO musicSetSO;

    [SerializeField]
    private BackgroundMusicRepeatMode repeatMode;

    private List<BackgroundMusicPlaylistItem> playlistItems;
    private int playlistIndex;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        playlistIndex = 0;
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => realtime.connected);

        if (string.IsNullOrEmpty(model.playlistIds))
        {
            CreatePlaylist();
        }

        ReadPlaylist();
        PlayClip();
    }

    void Update()
    {
        if (playlistItems == null)
            return;

        if (audioSource.isPlaying)
            return;

        if (repeatMode == BackgroundMusicRepeatMode.RepeatAll)
        {
            SelectNextClip();
        }

        PlayClip();
    }

    private void CreatePlaylist()
    {
        var shuffledItems = new List<BackgroundMusicPlaylistItem>(musicSetSO.MusicItems
            .Shuffle()
            .Select(x => new BackgroundMusicPlaylistItem { Id = x.Id, Audio = x.Audio }));
        var playlistIds = string.Join(";", shuffledItems.Select(x => x.Id).ToArray());

        model.playlistIds = playlistIds;
    }

    private void ReadPlaylist()
    {
        playlistItems = new List<BackgroundMusicPlaylistItem>(model.playlistIds.Split(";")
            .Select(x => musicSetSO.MusicItems.Single(y => y.Id == x))
            .Select(x => new BackgroundMusicPlaylistItem() { Id = x.Id, Audio = x.Audio }));
    }

    private void SelectNextClip()
    {
        if (playlistIndex < (playlistItems.Count - 1))
        {
            playlistIndex++;
        }
        else
        {
            playlistIndex = 0;
        }
    }

    private void PlayClip()
    {
        audioSource.clip = playlistItems[playlistIndex].Audio;
        audioSource.Play();
    }
}
