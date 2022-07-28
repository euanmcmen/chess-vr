using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Music/Music", order = 1)]
public class BackgroundMusicSO : ScriptableObject
{
    [SerializeField]
    private string id;

    [SerializeField]
    private AudioClip audio;

    public string Id => id;

    public AudioClip Audio => audio;
}