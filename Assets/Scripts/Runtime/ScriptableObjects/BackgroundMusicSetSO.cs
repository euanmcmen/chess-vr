using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Music/Music Set", order = 1)]
public class BackgroundMusicSetSO : ScriptableObject
{
    [SerializeField]
    private List<AudioClip> clips;

    public List<AudioClip> Clips => clips;
}