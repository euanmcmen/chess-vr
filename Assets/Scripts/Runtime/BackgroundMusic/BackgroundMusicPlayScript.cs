using Assets.Scripts.Runtime.Logic;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackgroundMusicPlayScript : MonoBehaviour
{
    [SerializeField]
    private BackgroundMusicSetSO musicSO;

    [SerializeField]
    private BackgroundMusicRepeatMode repeatMode;

    private List<AudioClip> clips;
    private int clipIndex;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        clips = musicSO.Clips.Shuffle().ToList();
    }

    void Start()
    {
        clipIndex = 0;
        PlayClip();
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            if (repeatMode == BackgroundMusicRepeatMode.RepeatAll)
            {
                SelectNextClip();
            }

            PlayClip();
        }
    }

    private void SelectNextClip()
    {
        if (clipIndex < (clips.Count - 1))
        {
            clipIndex++;
        }
        else
        {
            clipIndex = 0;
        }
    }

    private void PlayClip()
    {
        audioSource.clip = clips[clipIndex];
        audioSource.Play();
    }
}
