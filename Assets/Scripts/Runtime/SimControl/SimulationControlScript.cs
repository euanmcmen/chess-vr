using Normal.Realtime;
using System;
using System.Collections;
using UnityEngine;

public class SimulationControlScript : RealtimeComponent<SimulationControlModel>
{
    private GameSetupControlScript gameSetupControlScript;
    private SimulationBoardLinkScript simulationBoardLink;

    private event Action<bool> onRunningStateChanged;

    private event Action<bool> onRunningStateChangedClient;

    private event Action onSimulationStarted;

    private void Awake()
    {
        gameSetupControlScript = GetComponent<GameSetupControlScript>();
        simulationBoardLink = GetComponent<SimulationBoardLinkScript>();
    }

    //Start is called before the first frame update
    private IEnumerator Start()
    {
        EventActionBinder.BindSubscribersToAction<IRunningStateChangedSubscriber>((implementation) => onRunningStateChanged += implementation.HandleRunningStateChanged);
        EventActionBinder.BindSubscribersToAction<IRunningStateChangedSubscriber>((implementation) => onRunningStateChangedClient += implementation.HandleRunningStateChangedClient);
        EventActionBinder.BindSubscribersToAction<ISimulationStartedSubscriber>((implementation) => onSimulationStarted += implementation.HandleSimulationStarted);

        yield return new WaitUntil(() => realtime.connected);

        if (!model.simulationStarted)
        {
            Debug.LogFormat("I am the first one here and will create the DGOs. My ID is {0}", realtime.clientID);

            yield return StartCoroutine(gameSetupControlScript.CreateTurnData());

            StartSimulation();

            ToggleSimulationRunningState(false);

        }
    }

    private void StartSimulation()
    {
        model.simulationStarted = true;
        onSimulationStarted.Invoke();
    }

    public void ToggleSimulationRunningState(bool value)
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
        foreach (var piece in simulationBoardLink.BoardApi.GetAllPieces(activeOnly: false))
        {
            piece.GetComponent<RealtimeView>()
                .RequestOwnership();

            piece.GetComponent<RealtimeTransform>()
                .RequestOwnership();
        }
    }
}