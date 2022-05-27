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

    private event Action<bool> onRunningStateChanged;

    private event Action<bool> onRunningStateChangedClient;

    private void Awake()
    {
        gameControlScipt = GetComponent<GameControlScript>();
        simulationBoardLink = GetComponent<SimulationBoardLinkScript>();

        EventActionBinder.BindSubscribersToAction<IRunningStateChangedSubscriber>((implementation) => onRunningStateChanged += implementation.HandleRunningStateChanged);
        EventActionBinder.BindSubscribersToAction<IRunningStateChangedSubscriber>((implementation) => onRunningStateChangedClient += implementation.HandleRunningStateChangedClient);
    }

    //Start is called before the first frame update
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(10);

        if (model.simulationStarted)
        {
            Debug.LogFormat("I have joined a game in progress.  My ID is {0}", realtime.clientID);
            yield return new WaitForSeconds(7);

            Debug.Log("I am now taking ownership and pausing the simulation.");

            ToggleSimulationRunningState(false);

            yield return new WaitForSeconds(7);

            Debug.Log("... and continuing again.");

            ToggleSimulationRunningState(true);

            yield break;
        }

        Debug.LogFormat("I am the only one here and will start the simulation. My ID is {0}", realtime.clientID);

        model.simulationStarted = true;

        ToggleSimulationRunningState(true);

        //yield return new WaitForSeconds(4);

        //ToggleSimulationRunningState(false);

        //yield return new WaitForSeconds(5);

        //ToggleSimulationRunningState(true);

        //Replace with model.simulationStarted = true.
        //SimulationStarted will reflect whether the simulation has begun.
        //Then, model.isRunning will reflect whether the simulation is currently running or if it is not -- paused.
        //SimulationStarted may be used in future for a "reset" operation.
    }

    public void TestMessage(bool isRunning)
    {
        Debug.LogFormat("Setting running state to {0}", isRunning);
    }

    //private void StartSimulation()
    //{
    //    model.isRunning = true;
    //}

    //private void StopSimulation()
    //{
    //    model.isRunning = false;
    //}

    private void ToggleSimulationRunningState(bool value)
    {
        TakeOwnershipOfAllPieces();

        model.isRunning = value;

        // Only run this event for the owning client.
        onRunningStateChanged.Invoke(value);
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
        Debug.LogFormat("Attention all clients!  Simulation started state is now [{0}].", value);
    }

    private void HandleIsRunningDidChange(SimulationControlModel model, bool value)
    {
        Debug.LogFormat("Attention all clients!  Simulation running state is now [{0}].", value);

        onRunningStateChangedClient.Invoke(value);
    }

    private void TakeOwnershipOfAllPieces()
    {
        foreach (var piece in simulationBoardLink.BoardApi.GetAllActivePieces())
        {
            piece.GetComponent<RealtimeView>()
                .RequestOwnership();

            piece.GetComponent<RealtimeTransform>()
                .RequestOwnership();
        }
    }
}