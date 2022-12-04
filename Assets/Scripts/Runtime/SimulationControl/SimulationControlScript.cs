using Normal.Realtime;
using System;
using System.Collections;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

public class SimulationControlScript : RealtimeComponent<SimulationControlModel>, IRoomConnectedSubscriber, IRoomDisconnectedSubscriber
{
    private ChessGameSetupControlScript gameSetupControlScript;
    private SimulationBoardLinkScript simulationBoardLink;

    private event Action<bool> onRunningStateChanged;

    private event Action<bool> onRunningStateChangedClient;

    private event Action onSimulationStarted;

    private void Awake()
    {
        gameSetupControlScript = GetComponent<ChessGameSetupControlScript>();
        simulationBoardLink = GetComponent<SimulationBoardLinkScript>();
    }

    //Start is called before the first frame update
    private void Start()
    {
        EventActionBinder.BindSubscribersToAction<IRunningStateChangedSubscriber>((implementation) => onRunningStateChanged += implementation.HandleRunningStateChanged);
        EventActionBinder.BindSubscribersToAction<IRunningStateChangedSubscriber>((implementation) => onRunningStateChangedClient += implementation.HandleRunningStateChangedClient);
        EventActionBinder.BindSubscribersToAction<ISimulationStartedSubscriber>((implementation) => onSimulationStarted += implementation.HandleSimulationStarted);
    }

    public void HandleRoomConnected(Realtime realtime)
    {
        StartCoroutine(HandleRoomConnectedAsync(realtime));
    }

    public void HandleRoomDisconnected(Realtime realtime)
    {
        model.simulationStarted = false;
        ToggleSimulationRunningState(false);
    }

    private IEnumerator HandleRoomConnectedAsync(Realtime realtime)
    {
        Debug.LogFormat("Are we connected? '{0}'", model.simulationStarted);

        if (!model.simulationStarted)
        {
            Debug.LogFormat("I am the first one here and will create the DGOs. My ID is {0}", realtime.clientID);

            yield return StartCoroutine(gameSetupControlScript.CreateTurnData());

            StartSimulation();

            ToggleSimulationRunningState(false);
        }
        else
        {
            yield return null;
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