using System;
using System.Collections.Generic;
using Zaaml.Core;
using Range = Zaaml.Core.Range;

namespace Zaaml.UI.Panels.TablePanel
{
	public class CellRange : ICellRange
	{
		#region Ctors

		public CellRange()
		{
		}

		public CellRange(int column, int row, int columnSpan, int rowSpan)
		{
			Column = column;
			Row = row;
			ColumnSpan = columnSpan;
			RowSpan = rowSpan;
		}

		public static CellRange CreateCellRange(Range<int> columnRange, Range<int> rowRange)
		{
			return new CellRange(columnRange.Minimum, rowRange.Minimum, columnRange.Maximum - columnRange.Minimum + 1, rowRange.Maximum - rowRange.Minimum + 1);
		}

		#endregion

		#region Properties

		public int Column { get; set; }
		public int ColumnSpan { get; set; }
		public int Row { get; set; }
		public int RowSpan { get; set; }

		#endregion
	}

	public static class CellRangeExtensions
	{
		public static ICellRange Intersect(this ICellRange cellRange1, ICellRange cellRange2)
		{
			return CellRangeUtils.Intersect(cellRange1, cellRange2);
		}

		public static bool IntersectsWith(this ICellRange cellRange1, ICellRange cellRange2)
		{
			return CellRangeUtils.IntersectsWith(cellRange1, cellRange2);
		}

		public static int GetTopRow(this ICellRange self)
		{
			return CellRangeUtils.GetTopRow(self);
		}
		public static int GetBottomRow(this ICellRange self)
		{
			return CellRangeUtils.GetBottomRow(self);
		}
		public static int GetLeftColumn(this ICellRange self)
		{
			return CellRangeUtils.GetLeftColumn(self);
		}
		public static int GetRightColumn(this ICellRange self)
		{
			return CellRangeUtils.GetRightColumn(self);
		}

		public static bool Contains(this ICellRange self, ICellRange cellRange)
		{
			return CellRangeUtils.Contains(self, cellRange);
		}

		public static CellRange GetBoundingCellRange(this IEnumerable<ICellRange> cellRanges)
		{
			return CellRangeUtils.GetBoundingCellRange(cellRanges);
		}
	}

	public static class CellRangeUtils
	{
		public static ICellRange Intersect(ICellRange cellRange1, ICellRange cellRange2)
		{
			var columnRange1 = new Range<int>(cellRange1.Column, cellRange1.Column + cellRange1.ColumnSpan - 1);
			var columnRange2 = new Range<int>(cellRange2.Column, cellRange2.Column + cellRange2.ColumnSpan - 1);

			var columnIntersection = Range.Intersect(columnRange1, columnRange2);

			if (columnIntersection.IsEmpty)
				return null;

			var rowRange1 = new Range<int>(cellRange1.Row, cellRange1.Row + cellRange1.RowSpan - 1);
			var rowRange2 = new Range<int>(cellRange2.Row, cellRange2.Row + cellRange2.RowSpan - 1);

			var rowIntersection = Range.Intersect(rowRange1, rowRange2);

			if (rowIntersection.IsEmpty)
				return null;

			return CellRange.CreateCellRange(columnIntersection, rowIntersection);
		}

		internal static bool IntersectsWith(ICellRange cellRange1, ICellRange cellRange2)
		{
			var columnRange1 = new Interval<int>(cellRange1.Column, cellRange1.Column + cellRange1.ColumnSpan - 1);
			var columnRange2 = new Interval<int>(cellRange2.Column, cellRange2.Column + cellRange2.ColumnSpan - 1);

			var columnIntersection = Interval.Intersect(columnRange1, columnRange2);

			if (columnIntersection.IsEmpty)
				return false;

			var rowRange1 = new Interval<int>(cellRange1.Row, cellRange1.Row + cellRange1.RowSpan - 1);
			var rowRange2 = new Interval<int>(cellRange2.Row, cellRange2.Row + cellRange2.RowSpan - 1);

			var rowIntersection = Interval.Intersect(rowRange1, rowRange2);

			if (rowIntersection.IsEmpty)
				return false;

			return true;
		}

		internal static int GetTopRow(ICellRange cellRange)
		{
			return cellRange.Row;
		}
		internal static int GetBottomRow(ICellRange cellRange)
		{
			return cellRange.Row + cellRange.RowSpan - 1;
		}
		internal static int GetLeftColumn(ICellRange cellRange)
		{
			return cellRange.Column;
		}
		internal static int GetRightColumn(ICellRange cellRange)
		{
			return cellRange.Column + cellRange.ColumnSpan - 1;
		}

		internal static bool Contains(ICellRange self, ICellRange cellRange)
		{
			var selfColumnRange = Interval.Create(self.Column, self.Column + self.ColumnSpan - 1);
			var columnRange = Interval.Create(cellRange.Column, cellRange.Column + cellRange.ColumnSpan - 1);

			var selfRowRange = Interval.Create(self.Row, self.Row + self.RowSpan - 1);
			var rowRange = Interval.Create(cellRange.Row, cellRange.Row + cellRange.RowSpan - 1);

			return selfColumnRange.Contains(columnRange) && selfRowRange.Contains(rowRange);
		}

		public static CellRange GetBoundingCellRange(IEnumerable<ICellRange> cellRanges)
		{
			var leftColumn = int.MaxValue;
			var rightColumn = int.MinValue;
			var topRow = int.MaxValue;
			var bottomRow = int.MinValue;

			foreach (var cellRange in cellRanges)
			{
				leftColumn = Math.Min(leftColumn, cellRange.Column);
				rightColumn = Math.Max(rightColumn, cellRange.Column + cellRange.ColumnSpan - 1);
				topRow = Math.Min(topRow, cellRange.Row);
				bottomRow = Math.Max(bottomRow, cellRange.Row + cellRange.RowSpan - 1);
			}

			return new CellRange(leftColumn, topRow, rightColumn - leftColumn + 1, bottomRow - topRow + 1);
		}

		public static bool AreEqual(ICellRange cellRange1, ICellRange cellRange2)
		{
			if (ReferenceEquals(cellRange1, cellRange2))
				return true;

			if (cellRange1 == null && cellRange2 == null)
				return true;

			if (cellRange1 == null || cellRange2 == null)
				return false;

			return cellRange1.Column == cellRange2.Column &&
						 cellRange1.Row == cellRange2.Row &&
						 cellRange1.ColumnSpan == cellRange2.ColumnSpan &&
						 cellRange1.RowSpan == cellRange2.RowSpan;
		}
	}
}