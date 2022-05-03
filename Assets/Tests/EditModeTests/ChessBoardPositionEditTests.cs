using Assets.Scripts.Runtime.Logic;
using NUnit.Framework;

public class ChessBoardPositionEditTests
{
    [Test]
    public void CreatedWithFullNotationShouldResolveCorrectly()
    {
        var position = new ChessBoardPosition("b2");

        Assert.AreEqual("b2", position.Notation);
        Assert.AreEqual(ChessBoardColumnLetter.b, position.ColumnLetter);
        Assert.AreEqual(2, position.RowNumber);
    }

    [Test]
    public void CreatedWithPartialNotationShouldResolveCorrectly()
    {
        var position = new DisambiguationChessBoardPosition("b");

        Assert.AreEqual("b", position.Notation);
        Assert.AreEqual(ChessBoardColumnLetter.b, position.ColumnLetter);
    }

    [Test]
    public void CreatedWithLetterOnlyShouldResolveCorrectly()
    {
        var position = new DisambiguationChessBoardPosition(ChessBoardColumnLetter.b);

        Assert.AreEqual("b", position.Notation);
        Assert.AreEqual(ChessBoardColumnLetter.b, position.ColumnLetter);
    }
}
