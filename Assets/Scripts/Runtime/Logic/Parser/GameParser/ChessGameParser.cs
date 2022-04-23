using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Assets.Scripts.Runtime.Logic.Parser.GameParser
{
    public static class ChessGameParser
    {
        private const string SplitTurnsRegex = @"\d*\.\s\S*\s\S*\s?";

        public static List<string> ResolveTurnsInGame(string pgn)
        {
            List<string> result = new();
            var processedPgn = pgn.Replace(Environment.NewLine, " ");
            var matches = Regex.Matches(processedPgn, SplitTurnsRegex);

            foreach (Match match in matches)
            {
                result.Add(match.Value);
            }

            return result;
        }
    }
}