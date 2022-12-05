using Normal.Realtime;
using System;
using UnityEngine;

public class RoomConnectionControlScript : MonoBehaviour
{
    private event Action<Realtime> onRoomConnected;
    private event Action<Realtime> onRoomDisconnected;

    private void Awake()
    {
        var realtimeComponent = FindObjectOfType<Realtime>();
        realtimeComponent.didConnectToRoom += RealtimeComponent_didConnectToRoom;
        realtimeComponent.didDisconnectFromRoom += RealtimeComponent_didDisconnectFromRoom;
    }

    // Start is called before the first frame update
    void Start()
    {
        EventActionBinder.BindSubscribersToAction<IRoomConnectedSubscriber>((implementation) => onRoomConnected += implementation.HandleRoomConnected);
        EventActionBinder.BindSubscribersToAction<IRoomDisconnectedSubscriber>((implementation) => onRoomDisconnected += implementation.HandleRoomDisconnected);
    }

    private void RealtimeComponent_didConnectToRoom(Realtime realtime)
    {
        Debug.LogFormat("Connected to room '{0}'.  Notifying subscribers...", realtime.room.name);
        onRoomConnected(realtime);
    }

    private void RealtimeComponent_didDisconnectFromRoom(Realtime realtime)
    {
        Debug.LogFormat("disconnected from room '{0}'.  Notifying subscribers...", realtime.room.name);
        onRoomDisconnected(realtime);
    }
}
