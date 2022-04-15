using Assets.Scripts.Parser;
using NUnit.Framework;
using System.Linq;

public class ChessParserEditTests
{
    [Test]
    public void ShouldResolvePawnToE4()
    {
        var move = ChessParser.ResolveChessNotation(ChessPieceTeam.Light, "e4")
            .Single();

        Assert.AreEqual("e4", move.DestinationBoardPosition.Notation);
        Assert.AreEqual(ChessBoardColumnLetter.e, move.DestinationBoardPosition.ColumnLetter);
        Assert.AreEqual(ChessBoardColumnLetter.e, move.DisambiguationOriginBoardPosition.ColumnLetter);
        Assert.AreEqual(ChessPieceType.Pawn, move.PieceType);
    }

    [Test]
    public void ShouldResolveKnightToF3()
    {
        var move = ChessParser.ResolveChessNotation(ChessPieceTeam.Light, "Nf3")
            .Single();

        Assert.AreEqual("f3", move.DestinationBoardPosition.Notation);
        Assert.AreEqual(ChessPieceType.Knight, move.PieceType);
    }

    [Test]
    public void ShouldResolveBishopToB5()
    {
        var move = ChessParser.ResolveChessNotation(ChessPieceTeam.Light, "Bb5")
            .Single();

        Assert.AreEqual("b5", move.DestinationBoardPosition.Notation);
        Assert.AreEqual(ChessPieceType.Bishop, move.PieceType);
    }

    [Test]
    public void ShouldResolveBKnightToD2()
    {
        var move = ChessParser.ResolveChessNotation(ChessPieceTeam.Light, "Nbd2")
            .Single();

        Assert.AreEqual("d2", move.DestinationBoardPosition.Notation);
        Assert.AreEqual(ChessBoardColumnLetter.b, move.DisambiguationOriginBoardPosition.ColumnLetter);
        Assert.AreEqual(ChessPieceType.Knight, move.PieceType);
    }

    [Test]
    public void ShouldResolveH1RookToH6()
    {
        var move = ChessParser.ResolveChessNotation(ChessPieceTeam.Light, "Rh1h6")
            .Single();

        Assert.AreEqual("h6", move.DestinationBoardPosition.Notation);
        Assert.AreEqual("h1", move.DisambiguationOriginBoardPosition.Notation);
        Assert.AreEqual(ChessPieceType.Rook, move.PieceType);
    }

    [Test]
    public void ShouldResolveCPawnCaptureOnD4()
    {
        var moves = ChessParser.ResolveChessNotation(ChessPieceTeam.Light, "cxd4");

        //var captureEvent = moves.Single(x => x.PieceMoveType == ChessPieceMoveType.Capture);
        //Assert.AreEqual("d4", captureEvent.DestinationNotation);

        var pawnMove = moves.Single(x => x.PieceMoveType == ChessPieceMoveType.Move);
        Assert.AreEqual("d4", pawnMove.DestinationBoardPosition.Notation);
        Assert.AreEqual(ChessBoardColumnLetter.c, pawnMove.DisambiguationOriginBoardPosition.ColumnLetter);
        Assert.AreEqual(ChessPieceType.Pawn, pawnMove.PieceType);
    }


    [Test]
    public void ShouldResolveDarkTeamKingSideCastle()
    {
        var moves = ChessParser.ResolveChessNotation(ChessPieceTeam.Dark, "O-O");

        var rookMove = moves.Single(x => x.PieceType == ChessPieceType.Rook);
        var kingMove = moves.Single(x => x.PieceType == ChessPieceType.King);

        Assert.AreEqual("e8", kingMove.DisambiguationOriginBoardPosition.Notation);
        Assert.AreEqual("g8", kingMove.DestinationBoardPosition.Notation);
        Assert.AreEqual("h8", rookMove.DisambiguationOriginBoardPosition.Notation);
        Assert.AreEqual("f8", rookMove.DestinationBoardPosition.Notation);
    }
}
