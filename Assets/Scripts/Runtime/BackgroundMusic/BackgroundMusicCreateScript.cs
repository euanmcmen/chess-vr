using Normal.Realtime;
using UnityEngine;

public class BackgroundMusicCreateScript : MonoBehaviour, ISimulationStartedSubscriber
{
    [SerializeField]
    private GameObject backgroundMusicResource;
   
    public void HandleSimulationStarted()
    {
        SpawnBackgroundMusicObject();
    }

    private void SpawnBackgroundMusicObject()
    {
        var instantiationOptions = new Realtime.InstantiateOptions()
        {
            destroyWhenOwnerLeaves = false,
            destroyWhenLastClientLeaves = true,
            ownedByClient = false
        };

        Realtime.Instantiate(backgroundMusicResource.name, instantiationOptions);
    }
}

//