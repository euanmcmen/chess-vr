public static class ChessTurnParser
{
    private const string TurnRegex = @"^(?'turnNumber'\d*)\. (?'lightMoveNotation'\S*) (?'darkMoveNotation'\S*) $";
    
    public static ChessTurn ResolveChessTurn(string notation)
    {
        var matchKeys = RegexHelper.GetMatchCollection(notation, TurnRegex);

        var turn = new ChessTurn();
        turn.TurnNumber = int.Parse(matchKeys["turnNumber"]);
        turn.LightTeamMoveNotation = matchKeys["lightMoveNotation"];
        turn.DarkTeamMoveNotation = matchKeys["darkMoveNotation"];

        return turn;
    }
}
