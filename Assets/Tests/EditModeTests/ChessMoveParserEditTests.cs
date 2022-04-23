using Assets.Scripts.Runtime.Logic.Parser.MoveParser;
using NUnit.Framework;
using System.Linq;

public class ChessMoveParserEditTests
{
    [Test]
    public void ShouldResolvePawnToE4()
    {
        var move = ChessMoveParser.ResolveChessNotation(ChessPieceTeam.Light, "e4")
            .Single();

        Assert.False(move.CaptureOnDestinationTile);
        Assert.AreEqual("e4", move.DestinationBoardPosition.Notation);
        Assert.AreEqual(ChessBoardColumnLetter.e, move.DestinationBoardPosition.ColumnLetter);
        Assert.AreEqual(ChessBoardColumnLetter.e, move.DisambiguationOriginBoardPosition.ColumnLetter);
        Assert.AreEqual(ChessPieceType.Pawn, move.PieceType);
    }

    [Test]
    public void ShouldResolveKnightToF3()
    {
        var move = ChessMoveParser.ResolveChessNotation(ChessPieceTeam.Light, "Nf3")
            .Single();

        Assert.False(move.CaptureOnDestinationTile);
        Assert.AreEqual("f3", move.DestinationBoardPosition.Notation);
        Assert.AreEqual(ChessPieceType.Knight, move.PieceType);
    }

    [Test]
    public void ShouldResolveBishopToB5()
    {
        var move = ChessMoveParser.ResolveChessNotation(ChessPieceTeam.Light, "Bb5")
            .Single();

        Assert.False(move.CaptureOnDestinationTile);
        Assert.AreEqual("b5", move.DestinationBoardPosition.Notation);
        Assert.AreEqual(ChessPieceType.Bishop, move.PieceType);
    }

    [Test]
    public void ShouldResolveBKnightToD2()
    {
        var move = ChessMoveParser.ResolveChessNotation(ChessPieceTeam.Light, "Nbd2")
            .Single();

        Assert.False(move.CaptureOnDestinationTile);
        Assert.AreEqual("d2", move.DestinationBoardPosition.Notation);
        Assert.AreEqual(ChessBoardColumnLetter.b, move.DisambiguationOriginBoardPosition.ColumnLetter);
        Assert.AreEqual(ChessPieceType.Knight, move.PieceType);
    }

    [Test]
    public void ShouldResolveH1RookToH6()
    {
        var move = ChessMoveParser.ResolveChessNotation(ChessPieceTeam.Light, "Rh1h6")
            .Single();

        Assert.False(move.CaptureOnDestinationTile);
        Assert.AreEqual("h6", move.DestinationBoardPosition.Notation);
        Assert.AreEqual("h1", move.DisambiguationOriginBoardPosition.Notation);
        Assert.AreEqual(ChessPieceType.Rook, move.PieceType);
    }

    [Test]
    public void ShouldResolveCPawnCaptureOnD4()
    {
        var move = ChessMoveParser.ResolveChessNotation(ChessPieceTeam.Light, "cxd4")
            .Single();

        Assert.True(move.CaptureOnDestinationTile);
        Assert.AreEqual("d4", move.DestinationBoardPosition.Notation);
        Assert.AreEqual(ChessBoardColumnLetter.c, move.DisambiguationOriginBoardPosition.ColumnLetter);
        Assert.AreEqual(ChessPieceType.Pawn, move.PieceType);
    }

    [Test]
    public void ShouldResolveRookCaptureOnA5()
    {
        var move = ChessMoveParser.ResolveChessNotation(ChessPieceTeam.Light, "Rxa5")
            .Single();

        Assert.True(move.CaptureOnDestinationTile);
        Assert.AreEqual("a5", move.DestinationBoardPosition.Notation);
        Assert.AreEqual(ChessPieceType.Rook, move.PieceType);
    }

    [Test]
    public void ShouldResolveDarkTeamKingSideCastle()
    {
        var moves = ChessMoveParser.ResolveChessNotation(ChessPieceTeam.Dark, "O-O");

        var rookMove = moves.Single(x => x.PieceType == ChessPieceType.Rook);
        var kingMove = moves.Single(x => x.PieceType == ChessPieceType.King);

        Assert.AreEqual("e8", kingMove.DisambiguationOriginBoardPosition.Notation);
        Assert.AreEqual("g8", kingMove.DestinationBoardPosition.Notation);
        Assert.AreEqual("h8", rookMove.DisambiguationOriginBoardPosition.Notation);
        Assert.AreEqual("f8", rookMove.DestinationBoardPosition.Notation);
    }
}
