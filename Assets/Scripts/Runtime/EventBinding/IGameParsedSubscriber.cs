using System.Collections.Generic;

public interface IGameParsedSubscriber
{
    void HandleGameParsed(List<string> value);
}
