namespace Assets.Scripts.Runtime.Logic
{
    public interface IChessBoardPosition
    {
        string Notation { get; }

        ChessBoardColumnLetter ColumnLetter { get; }
    }
}