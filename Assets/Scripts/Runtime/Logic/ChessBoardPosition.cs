using System;

namespace Assets.Scripts.Runtime.Logic
{
    public class ChessBoardPosition
    {
        public ChessBoardColumnLetter ColumnLetter { get; private set; }
        public int RowNumber { get; private set; }
        public string Notation { get; private set; }
        public bool IsPartialNotation { get; private set; }

        public ChessBoardPosition(ChessBoardColumnLetter letter) : this(letter.ToString())
        {
        }

        public ChessBoardPosition(string notation)
        {
            IsPartialNotation = notation.Length switch
            {
                1 => true,
                2 => false,
                _ => throw new ArgumentException($"Invalid notation: {notation}"),
            };

            ColumnLetter = Enum.Parse<ChessBoardColumnLetter>(notation[0].ToString());

            if (!IsPartialNotation)
            {
                RowNumber = int.Parse(notation[1].ToString());
                Notation = notation;
            }
        }
    }
}