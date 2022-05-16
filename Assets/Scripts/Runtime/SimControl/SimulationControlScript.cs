using Normal.Realtime;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SimulationControlScript : RealtimeComponent<SimulationControlModel>
{
    private GameControlScript gameControlScipt;

    private SimulationBoardLinkScript simulationBoardLink;

    public event Action<bool> onRunningStateChanged;

    private void Awake()
    {
        gameControlScipt = GetComponent<GameControlScript>();
        simulationBoardLink = GetComponent<SimulationBoardLinkScript>();

        EventActionBinder.BindSubscribersToAction<IRunningStateChangedSubscriber>((implementation) => onRunningStateChanged += implementation.HandleRunningStateChanged);
    }

    //Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(3);

        model.simulationStarted = true;

        //Replace with model.simulationStarted = true.
        //SimulationStarted will reflect whether the simulation has begun.
        //Then, model.isRunning will reflect whether the simulation is currently running or if it is not -- paused.
        //SimulationStarted may be used in future for a "reset" operation.

        yield return new WaitForSeconds(4);

        model.isRunning = false;

        yield return new WaitForSeconds(5);

        model.isRunning = true;
    }

    public void TestMessage(bool isRunning)
    {
        Debug.LogFormat("Setting running state to {0}", isRunning);
    }

    protected override void OnRealtimeModelReplaced(SimulationControlModel previousModel, SimulationControlModel currentModel)
    {
        if (previousModel != null)
        {
            previousModel.simulationStartedDidChange -= HandleSimulationStartedDidChange;
            previousModel.isRunningDidChange -= HandleIsRunningDidChange;
        }

        if (currentModel != null)
        {
            if (currentModel.isFreshModel)
            {

            }

            currentModel.simulationStartedDidChange += HandleSimulationStartedDidChange;
            currentModel.isRunningDidChange += HandleIsRunningDidChange;
        }
    }

    private void HandleSimulationStartedDidChange(SimulationControlModel model, bool value)
    {
        // Cannot set to false at the moment.
        if (!value)
            return;

        TakeOwnership();

        Debug.Log("Starting simulation as host.");

        StartCoroutine(gameControlScipt.HandleGame());

        model.isRunning = true;
    }

    private void HandleIsRunningDidChange(SimulationControlModel model, bool value)
    {
        TakeOwnership();

        Debug.LogFormat("Updating simulation running state to [{0}] as host.", value);

        onRunningStateChanged.Invoke(value);

        //if (newIsRunningValue)
        //{
        //    Debug.Log("Running simulation as host.");
        //}
        //else
        //{
        //    Debug.Log("Stopping simulation as host.");

        //    //Dispatch an event to tell anything interested, "stop the simulation".
        //    //  The game parser should stop processing turns.
        //    //  The piece move script should stop processing moves.
        //    //  The piece's movement scripts should stop moving.
        //}
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