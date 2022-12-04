using Normal.Realtime;
using UnityEngine;

public class JoinRandomRoomScript : MonoBehaviour
{
    private Realtime realtimeComponent;

    private void Awake()
    {
        realtimeComponent = FindObjectOfType<Realtime>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //JoinRandomRoom();
    }

    public void JoinRandomRoom()
    {
        if (realtimeComponent.connected)
        {
            realtimeComponent.Disconnect();
        }

        var roomName = GetRandomRoomName();
        Debug.LogFormat("Randomly generated room name '{0}'.  Now attempting to connect!", roomName);

        realtimeComponent.Connect(roomName);
    }

    private string GetRandomRoomName()
    {
        return UnityEngine.Random.Range(100000, 999999).ToString();
    }
}
