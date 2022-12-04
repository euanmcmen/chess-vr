using Normal.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomTerminalUIScript : MonoBehaviour, IRoomConnectedSubscriber, IRoomDisconnectedSubscriber
{
    [SerializeField]
    private TMP_Text messageText;

    public void HandleRoomConnected(Realtime realtime)
    {
        Debug.LogFormat("And updating the displayed room to: '{0}'", realtime.room.name);
        messageText.text = realtime.room.name;
    }

    public void HandleRoomDisconnected(Realtime realtime)
    {
        Debug.Log("Disconnected.  Now in no room.");
        messageText.text = "None";
    }
}
