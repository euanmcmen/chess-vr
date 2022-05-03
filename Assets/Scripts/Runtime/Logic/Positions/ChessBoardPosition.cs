using System;

namespace Assets.Scripts.Runtime.Logic
{
    public class ChessBoardPosition : IChessBoardPosition
    {
        public ChessBoardColumnLetter ColumnLetter { get; private set; }
        public int RowNumber { get; private set; }
        public string Notation { get; private set; }

        public ChessBoardPosition(string notation)
        {
            if (notation.Length != 2) throw new ArgumentException($"Invalid notation: {notation}");

            ColumnLetter = Enum.Parse<ChessBoardColumnLetter>(notation[0].ToString());
            RowNumber = int.Parse(notation[1].ToString());
            Notation = notation;
        }
    }
}