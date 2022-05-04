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

    public void ToggleSimulationRunningState()
    {

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

    //TODO - Add change handlers for model.isRunning -- see GameScreenScript for flow.  change model property -> handle onchange event

    private void StartSimulation()
    {
        if (model.isRunning)
            return;

        model.isRunning = true;

        TakeOwnership();

        Debug.Log("Starting simulation as host.");

        StartCoroutine(gameControlScipt.HandleGame());
    }

    private void StopSimulation()
    {
        if (!model.isRunning)
            return;

        model.isRunning = false;

        TakeOwnership();

        //Dispatch an event to tell anything interested, "stop the simulation".
        //  The game parser should stop processing turns.
        //  The piece move script should stop processing moves.
        //  The piece's movement scripts should stop moving.
    }

    private void TakeOwnership()
    {
        foreach (var piece in simulationBoardLink.BoardApi.GetAllPieces())
        {
            piece.GetComponent<RealtimeTransform>()
                .RequestOwnership();
        }
    }
}