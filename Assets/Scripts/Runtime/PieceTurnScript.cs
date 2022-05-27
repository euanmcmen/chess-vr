using Normal.Realtime;

public class PieceTurnScript : RealtimeComponent<PieceTurnModel>
{
    public bool MovementStarted => model.movementStarted;
    public bool ShouldMoveOnThisTurn => model.shouldMoveOnThisTurn;
    public int InTurnMoveIndex => model.inTurnMoveIndex;
    public bool ShouldBeCapturedOnThisTurn => model.shouldBeCapturedOnThisTurn;

    public void SetCapturedOnThisTurn()
    {
        model.shouldBeCapturedOnThisTurn = true;
    }

    public void SetMovesOnThisTurn(int inTurnMoveIndex)
    {
        model.shouldMoveOnThisTurn = true;
        model.inTurnMoveIndex = inTurnMoveIndex;
    }

    public void SetMovementStarted()
    {
        model.movementStarted = true;
    }

    public void SetMovementFinished()
    {
        model.movementStarted = false;
        model.shouldBeCapturedOnThisTurn = false;
        model.shouldMoveOnThisTurn = false;
        model.inTurnMoveIndex = 0;
    }
}