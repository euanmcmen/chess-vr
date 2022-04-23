using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Runtime.Logic;
using Assets.Scripts.Runtime.Logic.Resolvers;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class LineOfSightResolverEditTests
{
    [Test]
    public void ShouldResolveHorizontalCheck()
    {
        var origin = new ChessBoardPosition("a1");
        var destination = new ChessBoardPosition("a6");

        var result = LineOfSightResolver.ResolveBoardTilesInRange(origin, destination);

        Assert.AreEqual(4, result.Count);
        Assert.AreEqual("a2", result[0].Notation);
        Assert.AreEqual("a3", result[1].Notation);
        Assert.AreEqual("a4", result[2].Notation);
        Assert.AreEqual("a5", result[3].Notation);
    }

    [Test]
    public void ShouldResolveVerticalCheck()
    {
        var origin = new ChessBoardPosition("a1");
        var destination = new ChessBoardPosition("f1");

        var result = LineOfSightResolver.ResolveBoardTilesInRange(origin, destination);

        Assert.AreEqual(4, result.Count);
        Assert.AreEqual("b1", result[0].Notation);
        Assert.AreEqual("c1", result[1].Notation);
        Assert.AreEqual("d1", result[2].Notation);
        Assert.AreEqual("e1", result[3].Notation);
    }

    [Test]
    public void ShouldResolveDiagonalCheck()
    {
        var origin = new ChessBoardPosition("a1");
        var destination = new ChessBoardPosition("f6");

        var result = LineOfSightResolver.ResolveBoardTilesInRange(origin, destination);

        Assert.AreEqual(4, result.Count);
        Assert.AreEqual("b2", result[0].Notation);
        Assert.AreEqual("c3", result[1].Notation);
        Assert.AreEqual("d4", result[2].Notation);
        Assert.AreEqual("e5", result[3].Notation);
    }

    [Test]
    public void ShouldResolveF1ToB5()
    {
        var origin = new ChessBoardPosition("f1");
        var destination = new ChessBoardPosition("b5");

        var result = LineOfSightResolver.ResolveBoardTilesInRange(origin, destination);

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual("e2", result[0].Notation);
        Assert.AreEqual("d3", result[1].Notation);
        Assert.AreEqual("c4", result[2].Notation);
    }

    [Test]
    public void ShouldResolveB5toA4()
    {
        var origin = new ChessBoardPosition("b5");
        var destination = new ChessBoardPosition("a4");

        var result = LineOfSightResolver.ResolveBoardTilesInRange(origin, destination);

        Assert.AreEqual(0, result.Count);
    }

    [Test]
    public void ShouldResolveA8toA5()
    {
        var origin = new ChessBoardPosition("a8");
        var destination = new ChessBoardPosition("a5");

        var result = LineOfSightResolver.ResolveBoardTilesInRange(origin, destination);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("a6", result[0].Notation);
        Assert.AreEqual("a7", result[1].Notation);
    }
}
