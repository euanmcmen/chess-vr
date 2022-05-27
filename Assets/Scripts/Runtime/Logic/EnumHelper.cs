namespace Assets.Scripts.Runtime.Logic
{
    public static class EnumHelper
    {
        public static ChessPieceTeam GetOtherTeam(ChessPieceTeam team) => (team == ChessPieceTeam.Light) ? ChessPieceTeam.Dark : ChessPieceTeam.Light;
    }
}
