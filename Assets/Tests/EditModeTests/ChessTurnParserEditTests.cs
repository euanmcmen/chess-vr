using Assets.Scripts.Runtime.Logic.Parser.TurnParser;
using NUnit.Framework;

public class ChessTurnParserEditTests
{
    [Test]
    public void ShouldResolveChessTurnFromInput()
    {
        var input = "1. e4 e5 ";

        var result = ChessTurnParser.ResolveChessTurn(input);

        Assert.AreEqual(1, result.TurnNumber);
        Assert.AreEqual("e4", result.LightTeamMoveNotation);
        Assert.AreEqual("e5", result.DarkTeamMoveNotation);
    }
}
