using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Runtime.Logic
{
    public class DisambiguationChessBoardPosition : IChessBoardPosition
    {
        public ChessBoardColumnLetter ColumnLetter { get; private set; }

        public string Notation => ColumnLetter.ToString();

        public DisambiguationChessBoardPosition(string notation)
        {
            if (notation.Length != 1) throw new ArgumentException($"Invalid notation: {notation}");

            ColumnLetter = Enum.Parse<ChessBoardColumnLetter>(notation[0].ToString());
        }

        public DisambiguationChessBoardPosition(ChessBoardColumnLetter letter)
        {
            ColumnLetter = letter;
        }
    }
}
