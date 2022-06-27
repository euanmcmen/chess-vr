//using Normal.Realtime;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PieceMovementScript : RealtimeComponent<PieceMovementModel>, IRunningStateChangedSubscriber
//{
//    [SerializeField]
//    private PieceConfigSO pieceConfig;

//    private Dictionary<int, float> sequenceLerpTimes;

//    private bool isRunning;

//    private float initialY;

//    private void Awake()
//    {
//        initialY = transform.position.y;
//    }

//    public void HandleRunningStateChangedClient(bool value)
//    {
//        StopAllCoroutines();
//    }

//    public void HandleRunningStateChanged(bool value)
//    {
//        isRunning = value;
//    }

//    public IEnumerator HandleFloatToDestinationPosition(Vector3 targetTilePosition)
//    {
//        // Initialise the lerp times dictionary on each movement instruction.
//        sequenceLerpTimes = new Dictionary<int, float>()
//        {
//            {0, 0.0f },
//            {1, 0.0f },
//            {2, 0.0f }
//        };

//        // Load the model's saved value into the dictionary to be used when "resuming" that move.
//        if (model.currentLerpTime > 0.0f)
//        {
//            sequenceLerpTimes[model.moveSequenceIndex] = model.currentLerpTime;
//        }

//        var currentPosition = transform.position;
//        var destinationPosition = GetPiecePositionOnTilePosition(targetTilePosition);
//        var currentPositionFloating = GetFloatPositionForPosition(currentPosition);
//        var destinationPositionFloating = GetFloatPositionForPosition(destinationPosition);

//        var movementPartCompletedAfterSeconds = pieceConfig.PieceMovementCompletesAfterSeconds / 3;

//        // The model move sequence index will be loaded from the model's saved state.
//        // Skip forward to the move which should be executed, and proceed from there.
//        // e.g. if we've already floated into the air (index 0 complete) before pausing, resume from index 1.

//        if (model.moveSequenceIndex <= 0)
//        {
//            model.moveSequenceIndex = 0;

//            yield return StartCoroutine(HandleLerp(currentPosition, currentPositionFloating, movementPartCompletedAfterSeconds));
//        }

//        if (model.moveSequenceIndex <= 1)
//        {
//            model.moveSequenceIndex = 1;

//            yield return StartCoroutine(HandleLerp(currentPositionFloating, destinationPositionFloating, movementPartCompletedAfterSeconds));
//        }

//        if (model.moveSequenceIndex <= 2)
//        {
//            model.moveSequenceIndex = 2;

//            yield return StartCoroutine(HandleLerp(destinationPositionFloating, destinationPosition, movementPartCompletedAfterSeconds));
//        }

//        model.moveSequenceIndex = 0;
//        model.currentLerpTime = 0;

//        transform.position = destinationPosition;
//    }

//    private IEnumerator HandleLerp(Vector3 current, Vector3 target, float timeToComplete)
//    {
//        var lerpTime = sequenceLerpTimes[model.moveSequenceIndex];

//        while (lerpTime < 1)
//        {
//            lerpTime += Time.deltaTime / timeToComplete;
//            transform.position = Vector3.Lerp(current, target, lerpTime);

//            model.currentLerpTime = lerpTime;

//            yield return new WaitUntil(() => isRunning);
//        }

//        transform.position = target;

//        sequenceLerpTimes[model.moveSequenceIndex] = 0;
//    }

//    private Vector3 GetPiecePositionOnTilePosition(Vector3 position)
//    {
//        return new Vector3(position.x, initialY, position.z);
//    }

//    private Vector3 GetFloatPositionForPosition(Vector3 position)
//    {
//        return new Vector3(position.x, initialY + pieceConfig.PieceMovementFloatHeight, position.z);
//    }
//}