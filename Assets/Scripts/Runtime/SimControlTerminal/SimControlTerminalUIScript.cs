using System.Collections;
using Assets.Scripts.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class SimControlTerminalUIScript : MonoBehaviour, IRunningStateChangedSubscriber
{
    [SerializeField]
    private Button playButton;

    [SerializeField]
    private Button pauseButton;

    public void HandleRunningStateChanged(bool value)
    {
        Debug.LogFormat("Received value: {0}", value);
        playButton.interactable = !value;
        pauseButton.interactable = value;
    }

    public void HandleRunningStateChangedClient(bool value)
    {
    }
}