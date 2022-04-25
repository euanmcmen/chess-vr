using System.Collections;
using UnityEngine;

public class SimulationControlScript : MonoBehaviour
{
    private GameControlScript gameControlScipt;

    private void Awake()
    {
        gameControlScipt = GetComponent<GameControlScript>();
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        //TODO - Simulation Data SO?
        yield return new WaitForSeconds(5);

        StartCoroutine(HandleSimulation());
    }

    public IEnumerator HandleSimulation()
    {
        yield return StartCoroutine(gameControlScipt.HandleGame());
    }
}