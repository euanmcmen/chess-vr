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
        Assert.AreEqual(false, position.IsPartialNotation);
    }

    [Test]
    public void CreatedWithPartialNotationShouldResolveCorrectly()
    {
        var position = new ChessBoardPosition("b");

        Assert.AreEqual(null, position.Notation);
        Assert.AreEqual(ChessBoardColumnLetter.b, position.ColumnLetter);
        Assert.AreEqual(0, position.RowNumber);
        Assert.AreEqual(true, position.IsPartialNotation);
    }

    [Test]
    public void CreatedWithLetterOnlyShouldResolveCorrectly()
    {
        var position = new ChessBoardPosition(ChessBoardColumnLetter.b);

        Assert.AreEqual(null, position.Notation);
        Assert.AreEqual(ChessBoardColumnLetter.b, position.ColumnLetter);
        Assert.AreEqual(0, position.RowNumber);
        Assert.AreEqual(true, position.IsPartialNotation);
    }
}
