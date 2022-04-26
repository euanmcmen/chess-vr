using Normal.Realtime;
using System.Collections;
using UnityEngine;

public class SimulationControlScript : RealtimeComponent<SimulationControlModel>
{
    private GameControlScript gameControlScipt;

    private SimulationBoardLinkScript simulationBoardLink;

    private void Awake()
    {
        gameControlScipt = GetComponent<GameControlScript>();
        simulationBoardLink = GetComponent<SimulationBoardLinkScript>();
    }

    //Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(6);

        StartSimulation();
    }

    protected override void OnRealtimeModelReplaced(SimulationControlModel previousModel, SimulationControlModel currentModel)
    {
        if (currentModel != null)
        {
            if (currentModel.isFreshModel)
            {
            }
        }
    }

    private void StartSimulation()
    {
        if (model.started)
            return;

        model.started = true;

        foreach (var piece in simulationBoardLink.BoardApi.GetAllPieces())
        {
            piece.GetComponent<RealtimeTransform>()
                .RequestOwnership();
        }

        Debug.Log("Starting simulation as host.");

        StartCoroutine(gameControlScipt.HandleGame());
    }
}