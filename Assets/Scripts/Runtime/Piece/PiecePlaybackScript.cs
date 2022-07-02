using Normal.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecePlaybackScript : RealtimeComponent<PiecePlaybackModel>, IRunningStateChangedSubscriber
{
    private PieceConfigDataScript pieceConfigDataScript;
    private PiecePlaybackSfxScript piecePlaybackSfxScript;

    private Dictionary<int, float> sequenceLerpTimes;

    private bool isRunning;

    private float initialY;

    private void Awake()
    {
        piecePlaybackSfxScript = GetComponent<PiecePlaybackSfxScript>();
        pieceConfigDataScript = GetComponent<PieceConfigDataScript>();

        initialY = transform.position.y;
    }

    public void HandleRunningStateChangedClient(bool value)
    {
        StopAllCoroutines();
    }

    public void HandleRunningStateChanged(bool value)
    {
        isRunning = value;
    }

    public IEnumerator HandleFloatToDestinationPosition(Vector3 targetTilePosition)
    {
        InitialiseLerp();

        var currentPosition = transform.position;
        var destinationPosition = GetPiecePositionOnTilePosition(targetTilePosition);
        var currentPositionFloating = GetFloatPositionForPosition(currentPosition);
        var destinationPositionFloating = GetFloatPositionForPosition(destinationPosition);

        var movementPartCompletedAfterSeconds = pieceConfigDataScript.PieceConfig.PieceMovementCompletesAfterSeconds / 3;

        // The model move sequence index will be loaded from the model's saved state.
        // Skip forward to the move which should be executed, and proceed from there.
        // e.g. if we've already floated into the air (index 0 complete) before pausing, resume from index 1.

        yield return StartCoroutine(HandleLerpSequenceStep(0, currentPosition, currentPositionFloating, movementPartCompletedAfterSeconds));

        yield return StartCoroutine(HandleLerpSequenceStep(1, currentPositionFloating, destinationPositionFloating, movementPartCompletedAfterSeconds));

        yield return StartCoroutine(HandleLerpSequenceStep(2, destinationPositionFloating, destinationPosition, movementPartCompletedAfterSeconds));

        HandleMovementFinished();

        model.moveSequenceIndex = 0;
        model.currentLerpTime = 0;

        transform.position = destinationPosition;
    }

    private void HandleMovementFinished()
    {
        piecePlaybackSfxScript.PlayLandingSoundEffect();
    }

    private void InitialiseLerp()
    {
        // Initialise the lerp times dictionary on each movement instruction.
        sequenceLerpTimes = new Dictionary<int, float>()
        {
            {0, 0.0f },
            {1, 0.0f },
            {2, 0.0f }
        };

        // Load the model's saved value into the dictionary to be used when "resuming" that move.
        if (model.currentLerpTime > 0.0f)
        {
            sequenceLerpTimes[model.moveSequenceIndex] = model.currentLerpTime;
        }
    }

    private IEnumerator HandleLerpSequenceStep(int sequenceIndex, Vector3 current, Vector3 target, float timeToComplete)
    {
        if (model.moveSequenceIndex <= sequenceIndex)
        {
            model.moveSequenceIndex = sequenceIndex;

            yield return StartCoroutine(HandleLerp(current, target, timeToComplete));
        }

        yield return null;
    }

    private IEnumerator HandleLerp(Vector3 current, Vector3 target, float timeToComplete)
    {
        var lerpTime = sequenceLerpTimes[model.moveSequenceIndex];

        while (lerpTime < 1)
        {
            lerpTime += Time.deltaTime / timeToComplete;
            transform.position = Vector3.Lerp(current, target, lerpTime);

            model.currentLerpTime = lerpTime;

            yield return new WaitUntil(() => isRunning);
        }

        transform.position = target;

        sequenceLerpTimes[model.moveSequenceIndex] = 0;
    }

    private Vector3 GetPiecePositionOnTilePosition(Vector3 position)
    {
        return new Vector3(position.x, initialY, position.z);
    }

    private Vector3 GetFloatPositionForPosition(Vector3 position)
    {
        return new Vector3(position.x, initialY + pieceConfigDataScript.PieceConfig.PieceMovementFloatHeight, position.z);
    }
}
