﻿using Assets.Scripts.Runtime.Logic.Parser.GameParser;
using NUnit.Framework;

public class ChessGameParserEditTests
{
    [Test]
    public void ShouldResolvePGN()
    {
        var input = @"1. e4 e5 2. Nf3 Nc6 3. Bb5 a6 4. Ba4 Nf6 5. O-O Be7 6. Re1 b5 7. Bb3 d6 8. c3
O-O 9. h3 Bb7 10. d4 Na5 11. Bc2 Nc4 12. b3 Nb6 13. Nbd2 Nbd7 14. b4 exd4 15.
cxd4 a5 16. bxa5 c5 17. e5 dxe5 18. dxe5 Nd5 19. Ne4 Nb4 20. Bb1 Rxa5 21. Qe2
Nb6 22. Nfg5 Bxe4 23. Qxe4 g6 24. Qh4 h5 25. Qg3 Nc4 26. Nf3 Kg7 27. Qf4 Rh8
28. e6 f5 29. Bxf5 Qf8 30. Be4 Qxf4 31. Bxf4 Re8 32. Rad1 Ra6 33. Rd7 Rxe6 34.
Ng5 Rf6 35. Bf3 Rxf4 36. Ne6+ Kf6 37. Nxf4 Ne5 38. Rb7 Bd6 39. Kf1 Nc2 40. Re4
Nd4 41. Rb6 Rd8 42. Nd5+ Kf5 43. Ne3+ Ke6 44. Be2 Kd7 45. Bxb5+ Nxb5 46. Rxb5
Kc6 47. a4 Bc7 48. Ke2 g5 49. g3 Ra8 50. Rb2 Rf8 51. f4 gxf4 52. gxf4 Nf7 53.
Re6+ Nd6 54. f5 Ra8 55. Rd2 Rxa4 56. f6 1-0";

        var result = ChessGameParser.ResolveTurnsInGame(input);

        Assert.AreEqual("1. e4 e5 ", result[0]);
        Assert.AreEqual("2. Nf3 Nc6 ", result[1]);
        Assert.AreEqual("13. Nbd2 Nbd7 ", result[12]);
        Assert.AreEqual("55. Rd2 Rxa4 ", result[54]);
        Assert.AreEqual("56. f6 1-0", result[55]);
    }
}
