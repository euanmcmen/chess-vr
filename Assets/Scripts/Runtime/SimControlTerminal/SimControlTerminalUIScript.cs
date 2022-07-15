using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimControlTerminalUIScript : MonoBehaviour, IRunningStateChangedSubscriber
{
    [SerializeField]
    private Button playButton;

    [SerializeField]
    private Button pauseButton;

    [SerializeField]
    private TMP_Text messageText;

    public void HandleRunningStateChanged(bool value)
    {
        playButton.interactable = !value;
        pauseButton.interactable = value;
        messageText.text = value ? "Running" : "Stopped";
    }

    public void HandleRunningStateChangedClient(bool value)
    {
    }
}