using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.LineOfSight
{
    public static class LineOfSightResolver
    {
        public static List<ChessBoardPosition> GetBoardTileNotationInRange(ChessBoardPosition startPosition, ChessBoardPosition endPosition)
        {
            var result = new List<ChessBoardPosition>();

            // True if comparing along columns: pieces between a1, a2, a3, a4
            // i.e. Horizontal movement along the board, moving through columns.
            bool isColumnTraversal = startPosition.ColumnLetter == endPosition.ColumnLetter;

            // True if comparing up rows: pieces between a1, b1, c1, d1
            // i.e. Vertical movement up the board, moving through rows.
            bool isRowTraversal = !isColumnTraversal && startPosition.RowNumber == endPosition.RowNumber;

            // True if comparing up rows and along columns at the same time: pieces between a1, b2, c3, d4
            bool isDiagonalTraversal = !isColumnTraversal && !isRowTraversal;

            if (isColumnTraversal)
            {
                result.AddRange(GetColumnTraversalPositions(startPosition, endPosition));
            }
            else if (isRowTraversal)
            {
                result.AddRange(GetRowTraversalPositions(startPosition, endPosition));
            }
            else if (isDiagonalTraversal)
            {
                result.AddRange(GetDiagonalTraversalPositions(startPosition, endPosition));
            }

            return result;
        }

        private static List<ChessBoardColumnLetter> GetColumnLetters(ChessBoardPosition startPosition, ChessBoardPosition endPosition)
        {
            var columns = new List<ChessBoardColumnLetter>(2) { startPosition.ColumnLetter, endPosition.ColumnLetter };
            columns.Sort();
            return columns;
        }

        private static List<int> GetRowNumbers(ChessBoardPosition startPosition, ChessBoardPosition endPosition)
        {
            var rows = new List<int>(2) { startPosition.RowNumber, endPosition.RowNumber };
            rows.Sort();
            return rows;
        }

        private static List<ChessBoardPosition> GetColumnTraversalPositions(ChessBoardPosition startPosition, ChessBoardPosition endPosition)
        {
            var columns = GetColumnLetters(startPosition, endPosition);
            var rows = GetRowNumbers(startPosition, endPosition);

            List<ChessBoardPosition> result = new List<ChessBoardPosition>();

            for (var column = columns[0]; column <= columns[1]; column++)
            {
                for (var row = rows[0] + 1; row < rows[1]; row++)
                {
                    result.Add(new ChessBoardPosition($"{column}{row}"));
                }
            }

            return result;
        }

        private static List<ChessBoardPosition> GetRowTraversalPositions(ChessBoardPosition startPosition, ChessBoardPosition endPosition)
        {
            var columns = GetColumnLetters(startPosition, endPosition);
            var rows = GetRowNumbers(startPosition, endPosition);

            List<ChessBoardPosition> result = new List<ChessBoardPosition>();

            for (var row = rows[0]; row <= rows[1]; row++)
            {
                for (var column = columns[0] + 1; column < columns[1]; column++)
                {
                    result.Add(new ChessBoardPosition($"{column}{row}"));
                }
            }

            return result;
        }

        private static List<ChessBoardPosition> GetDiagonalTraversalPositions(ChessBoardPosition startPosition, ChessBoardPosition endPosition)
        {
            var columns = new List<ChessBoardColumnLetter>(2) { startPosition.ColumnLetter, endPosition.ColumnLetter };
            var rows = new List<int>(2) { startPosition.RowNumber, endPosition.RowNumber };

            bool columnAscending = columns[0] < columns[1];
            bool rowAscending = rows[0] < rows[1];

            List<ChessBoardPosition> result = new List<ChessBoardPosition>();

            var columnLetters = new List<ChessBoardColumnLetter>();
            var rowNumbers = new List<int>();

            if (columnAscending)
            {
                for (var column = columns[0] + 1; column < columns[1]; column++)
                {
                    columnLetters.Add(column);
                }
            }
            else
            {
                for (var column = columns[0] - 1; column > columns[1]; column--)
                {
                    columnLetters.Add(column);
                }
            }

            if (rowAscending)
            {
                for (var row = rows[0] + 1; row < rows[1]; row++)
                {
                    rowNumbers.Add(row);
                }
            }
            else
            {
                for (var row = rows[0] - 1; row > rows[1]; row--)
                {
                    rowNumbers.Add(row);
                }
            }

            for (int i = 0; i < columnLetters.Count || i < rowNumbers.Count; i++)
            {
                result.Add(new ChessBoardPosition($"{columnLetters[i]}{rowNumbers[i]}"));
            }

            return result;
        }
    }
}
