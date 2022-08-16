using Assets.Scripts.Runtime.Logic;

public interface ITurnSetParsedSubscriber
{
    void HandleTurnSetParsedEvent(ChessTurnSet chessTurnSet);
}
