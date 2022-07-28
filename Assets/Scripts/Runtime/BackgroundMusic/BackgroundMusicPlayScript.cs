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

        Debug.LogFormat("Playlist Ids: {0}", model.playlistIds);

        if (string.IsNullOrEmpty(model.playlistIds))
        {
            CreatePlaylist();
        }

        Debug.LogFormat("Playlist Ids: {0}", model.playlistIds);

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

    protected override void OnRealtimeModelReplaced(BackgroundMusicPlayModel previousModel, BackgroundMusicPlayModel currentModel)
    {
        if (previousModel != null)
        {
            previousModel.playlistIdsDidChange -= HandlePlaylistIdsDidChange;
        }

        if (currentModel != null)
        {
            if (currentModel.isFreshModel)
            {

            }

            currentModel.playlistIdsDidChange += HandlePlaylistIdsDidChange;
        }
    }

    private void HandlePlaylistIdsDidChange(BackgroundMusicPlayModel model, string value)
    {
        //if (string.IsNullOrEmpty(model.playlistIds))
        //{
        //    Debug.LogWarning("TRIED TO READ Playlist too early");
        //    return;
        //}

        //ReadPlaylist();
        //PlayClip();
    }

    private void CreatePlaylist()
    {
        if (!string.IsNullOrEmpty(model.playlistIds))
        {
            Debug.LogWarning("TRIED TO CREATE Playlist too late.");
            return;
        }

        var shuffledItems = new List<BackgroundMusicPlaylistItem>(musicSetSO.MusicItems
            .Shuffle()
            .Select(x => new BackgroundMusicPlaylistItem { Id = x.Id, Audio = x.Audio }));
        var playlistIds = string.Join(";", shuffledItems.Select(x => x.Id).ToArray());

        model.playlistIds = playlistIds;

        Debug.LogFormat("CREATED Playlist: {0}", model.playlistIds);
    }

    private void ReadPlaylist()
    {
        if (string.IsNullOrEmpty(model.playlistIds))
        {
            Debug.LogWarning("TRIED TO READ Playlist too early");
            return;
        }

        if (playlistItems != null)
        {
            Debug.LogWarning("TRIED TO READ Playlist too early 2");
            return;
        }

        Debug.LogFormat("READ Playlist: {0}", model.playlistIds);

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
