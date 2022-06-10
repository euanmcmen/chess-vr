using Normal.Realtime;
using System;
using System.Collections;
using UnityEngine;

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
        yield return new WaitUntil(() => realtime.connected);

        if (!model.simulationStarted)
        {
            Debug.LogFormat("I am the first one here and will create the DGOs. My ID is {0}", realtime.clientID);
            gameControlScipt.CreateTurnData();
            model.simulationStarted = true;
            ToggleSimulationRunningState(false);
        }
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
        foreach (var piece in simulationBoardLink.BoardApi.GetAllPieces())
        {
            piece.GetComponent<RealtimeView>()
                .RequestOwnership();

            piece.GetComponent<RealtimeTransform>()
                .RequestOwnership();
        }
    }
}