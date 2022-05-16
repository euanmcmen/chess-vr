using System.Collections;
using UnityEngine;

public class PieceMovementScript : MonoBehaviour, IRunningStateChangedSubscriber
{
    [SerializeField]
    private PieceConfigSO pieceConfig;

    private bool isRunning;

    public void HandleRunningStateChanged(bool value)
    {
        isRunning = value;
    }

    public IEnumerator HandleFloatToDestinationPosition(Vector3 destinationPosition)
    {
        isRunning = true;

        var currentPosition = transform.position;
        var currentPositionFloating = GetFloatPositionForPosition(currentPosition);
        var destinationPositionFloating = GetFloatPositionForPosition(destinationPosition);

        var movementPartCompletedAfterSeconds = pieceConfig.PieceMovementCompletesAfterSeconds / 3;

        yield return StartCoroutine(HandleLerp(currentPosition, currentPositionFloating, movementPartCompletedAfterSeconds));

        yield return StartCoroutine(HandleLerp(currentPositionFloating, destinationPositionFloating, movementPartCompletedAfterSeconds));

        yield return StartCoroutine(HandleLerp(destinationPositionFloating, destinationPosition, movementPartCompletedAfterSeconds));

        transform.position = destinationPosition;
    }

    private IEnumerator HandleLerp(Vector3 current, Vector3 target, float timeToComplete)
    {
        var lerpTime = 0.0f;

        while (lerpTime < 1)
        {
            lerpTime += Time.deltaTime / timeToComplete;
            transform.position = Vector3.Lerp(current, target, lerpTime);
            //yield return null;
            yield return new WaitUntil(() => isRunning);
        }
    }

    private Vector3 GetFloatPositionForPosition(Vector3 position)
    {
        return new Vector3(position.x, position.y + pieceConfig.PieceMovementFloatHeight, position.z);
    }
}