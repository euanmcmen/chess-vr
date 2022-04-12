using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ChessParserTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void ShouldResolvePawnToE4()
    {
        var move = ChessMoveParser.ResolveChessNotation(ChessPieceTeam.Light, "e4")
            .Single();

        Assert.AreEqual("e4", move.DestinationNotation);
        Assert.AreEqual(ChessBoardColumnLetter.e, move.DestinationColumnLetter);
        Assert.AreEqual(ChessBoardColumnLetter.e, move.DisambiguationOriginColumnLetter);
        Assert.AreEqual(ChessPieceType.Pawn, move.PieceType);
    }

    [Test]
    public void ShouldResolveKnightToF3()
    {
        var move = ChessMoveParser.ResolveChessNotation(ChessPieceTeam.Light, "Nf3")
            .Single();

        Assert.AreEqual("f3", move.DestinationNotation);
        Assert.AreEqual(ChessPieceType.Knight, move.PieceType);
    }

    [Test]
    public void ShouldResolveBishopToB5()
    {
        var move = ChessMoveParser.ResolveChessNotation(ChessPieceTeam.Light, "Bb5")
            .Single();

        Assert.AreEqual("b5", move.DestinationNotation);
        Assert.AreEqual(ChessPieceType.Bishop, move.PieceType);
    }

    [Test]
    public void ShouldResolveBKnightToD2()
    {
        var move = ChessMoveParser.ResolveChessNotation(ChessPieceTeam.Light, "Nbd2")
            .Single();

        Assert.AreEqual("d2", move.DestinationNotation);
        Assert.AreEqual(ChessBoardColumnLetter.b, move.DisambiguationOriginColumnLetter);
        Assert.AreEqual(ChessPieceType.Knight, move.PieceType);
    }

    [Test]
    public void ShouldResolveCPawnCaptureOnD4()
    {
        var moves = ChessMoveParser.ResolveChessNotation(ChessPieceTeam.Light, "cxd4");

        var captureEvent = moves.Single(x => x.PieceMoveType == ChessPieceMoveType.Capture);
        Assert.AreEqual("d4", captureEvent.DestinationNotation);

        var pawnMove = moves.Single(x => x.PieceMoveType == ChessPieceMoveType.Move);
        Assert.AreEqual("d4", pawnMove.DestinationNotation);
        Assert.AreEqual(ChessBoardColumnLetter.c, pawnMove.DisambiguationOriginColumnLetter);
        Assert.AreEqual(ChessPieceType.Pawn, pawnMove.PieceType);
    }
}
