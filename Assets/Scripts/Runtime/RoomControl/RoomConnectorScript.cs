using Normal.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomConnectorScript : MonoBehaviour
{
    private bool isConnecting;

    public void JoinRoomByName(string roomName)
    {
        if (isConnecting) return;

        Debug.LogFormat("Room connector routing you to room '{0}'", roomName);

        StartCoroutine(JoinRoomAndReloadScene(roomName));
    }

    private IEnumerator JoinRoomAndReloadScene(string roomName)
    {
        isConnecting = true;

        TryDisconnect();

        var reloadSceneOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        while (!reloadSceneOperation.isDone)
        {
            yield return null;
        }

        FindObjectOfType<Realtime>().Connect(roomName);
        isConnecting = false;
    }

    private void TryDisconnect()
    {
        var realtime = FindObjectOfType<Realtime>();

        if (realtime.connected)
        {
            realtime.Disconnect();
        }
    }

}
