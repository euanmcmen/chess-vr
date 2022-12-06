using Normal.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinRoomScript : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputTextField;

    // Start is called before the first frame update
    void Start()
    {
        JoinRandomRoom();
    }

    public void JoinSpecificRoom()
    {
        Debug.LogFormat("Using specific name '{0}'.  Now attempting to connect!", inputTextField.text);

        FindObjectOfType<RoomConnectorScript>().JoinRoomByName(inputTextField.text);
    }

    public void JoinRandomRoom()
    {
        var roomName = GetRandomRoomName();
        Debug.LogFormat("Randomly generated room name '{0}'.  Now attempting to connect!", roomName);

        FindObjectOfType<RoomConnectorScript>().JoinRoomByName(roomName);
    }

    private string GetRandomRoomName()
    {
        return UnityEngine.Random.Range(100000, 999999).ToString();
    }
}
