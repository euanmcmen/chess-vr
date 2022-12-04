using Normal.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class SceneConnectionControlScript : MonoBehaviour, IRoomConnectedSubscriber, IRoomDisconnectedSubscriber
{
    private Scene currentScene;

    // Start is called before the first frame update
    void Awake()
    {
        currentScene = SceneManager.GetActiveScene();
        Debug.LogFormat("Current scene name: {0}", currentScene.name);
    }

    public void HandleRoomConnected(Realtime realtime)
    {
        Debug.Log("SceneConnectionControlScript.HandleRoomConnected: Nothing here.");
    }

    public void HandleRoomDisconnected(Realtime realtime)
    {
        StartCoroutine(HandleRoomDisconnectedAsync(realtime));
    }

    private IEnumerator HandleRoomDisconnectedAsync(Realtime realtime)
    {
        Debug.Log("Deloading and waiting...");
        //var unloadResult = SceneManager.UnloadSceneAsync(currentScene);
        //yield return new WaitUntil(() => unloadResult.isDone);

        yield return realtime.disconnected;

        Debug.LogFormat("Loading scene name: {0}", currentScene.name);
        SceneManager.LoadScene(currentScene.name);
    }
}
