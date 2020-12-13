using System;
using System.Linq;
using System.Windows;
using Zaaml.Core;

namespace Zaaml.UI.Panels.TablePanel
{
	public class TableDefinitionView
	{
		#region Fields

		private TableBorder _border;
		private TableDefinitionArray _columns;
		private TableDefinitionArray _rows;

		#endregion

		#region Events

		public event EventHandler Changed;

		#endregion

		#region Ctors

		public TableDefinitionView(int columnCount, int rowCount, float columnWidth, float rowHeight, TableBorder border)
		{
			_border = border;
			_columns = new TableDefinitionArray(columnCount, columnWidth, GetColumnLines(border));
			_rows = new TableDefinitionArray(rowCount, rowHeight, GetRowLines(border));
		}

		#endregion

		#region Properties

		public TableBorder Border
		{
			get => _border;
			set
			{
				if (_border == value)
					return;

				_border = value;

				_columns.Lines = GetColumnLines(_border);
				_rows.Lines = GetRowLines(_border);

				OnChanged();
			}
		}

		public int ColumnsCount => _columns.Count;

		public float Height => _rows.FullLength;

		public int RowsCount => _rows.Count;

		public float Width => _columns.FullLength;

		#endregion

		#region Methods

		private bool CheckColumn(int column)
		{
			return column >= 0 && column < ColumnsCount;
		}

		private bool CheckRow(int row)
		{
			return row >= 0 && row < RowsCount;
		}

		public Rect GetCellBounds(ICellRange cellRange)
		{
			return GetCellBounds(cellRange.Column, cellRange.Row, cellRange.ColumnSpan, cellRange.RowSpan);
		}

		public Rect GetCellBounds(int column, int row, int columnSpan = 1, int rowSpan = 1)
		{
			var rightColumn = Math.Min(column + columnSpan - 1, ColumnsCount - 1);
			var bottomRow = Math.Min(row + rowSpan - 1, RowsCount - 1);

			if (!CheckColumn(column) || !CheckColumn(rightColumn) || !CheckRow(row) || !CheckRow(bottomRow))
				return Rect.Empty;

			var left = _columns.GetDefinitionOffset(Math.Min(column, ColumnsCount - 1));
			var top = _rows.GetDefinitionOffset(Math.Min(row, RowsCount - 1));
			var right = _columns.GetDefinitionOffset(rightColumn) + _columns.GetDefinitionLength(rightColumn);
			var bottom = _rows.GetDefinitionOffset(bottomRow) + _rows.GetDefinitionLength(bottomRow);

			return new Rect(new Point(left, top), new Point(right, bottom));
		}

		public float GetColumnOffset(int column)
		{
			return _columns.GetDefinitionOffset(column);
		}

		public float GetColumnWidth(int column)
		{
			return _columns.GetDefinitionLength(column);
		}

		public Range<int> GetColumnsRange(float horizontalOffset, float viewportWidth)
		{
			return _columns.GetDefinitionRange(horizontalOffset, viewportWidth);
		}

		public float GetHorizontalLineOffset(int index)
		{
			return index == 0 ? 0 : _rows.GetDefinitionOffset(index - 1) + _rows.GetDefinitionLength(index - 1);
		}

		public float GetRowHeight(int row)
		{
			return _rows.GetDefinitionLength(row);
		}

		public float GetRowOffset(int row)
		{
			return _rows.GetDefinitionOffset(row);
		}

		public Range<int> GetRowsRange(float verticalOffset, float viewportHeight)
		{
			return _rows.GetDefinitionRange(verticalOffset, viewportHeight);
		}

		public float GetVerticalLineOffset(int index)
		{
			return index == 0 ? 0 : _columns.GetDefinitionOffset(index - 1) + _columns.GetDefinitionLength(index - 1);
		}

		public ICellRange GetVisibleRange(float horizontalOffset, float verticalOffset, float viewPortWidth, float viewPortHeight)
		{
			var columnRange = GetColumnsRange(horizontalOffset, viewPortWidth);
			var rowRange = GetRowsRange(verticalOffset, viewPortHeight);

			return new CellRange(columnRange.Minimum, rowRange.Minimum, columnRange.Maximum - columnRange.Minimum + 1, rowRange.Maximum - rowRange.Minimum + 1);
		}

		public ICellRange HitTest(Point point)
		{
			var column = _columns.GetDefinition((float)point.X);
			var row = _rows.GetDefinition((float)point.Y);
			return column != -1 && row != -1 ? new CellRange(column, row, 1, 1) : null;
		}

		public void Resize(int columnCount, int rowCount, float columnWidth, float rowHeight)
		{
			_columns = new TableDefinitionArray(columnCount, columnWidth, GetColumnLines(Border));
			_rows = new TableDefinitionArray(rowCount, rowHeight, GetRowLines(Border));
			OnChanged();
		}

		public void SetColumnWidth(int column, float width)
		{
			_columns.SetDefinitionLength(column, width);
			OnChanged();
		}

		public void SetColumnWidth(float width)
		{
			for (var iColumn = 0; iColumn < ColumnsCount; iColumn++)
				_columns.SetDefinitionLength(iColumn, width);

			OnChanged();
		}

		public void SetRowHeight(float height)
		{
			for (var iRow = 0; iRow < RowsCount; iRow++)
				_rows.SetDefinitionLength(iRow, height);

			OnChanged();
		}

		public void SetRowHeight(int row, float height)
		{
			_rows.SetDefinitionLength(row, height);
			OnChanged();
		}

		public void UpdateSize()
		{
			_columns.UpdateSize();
			_rows.UpdateSize();
		}

		protected virtual void OnChanged()
		{
			var handler = Changed;
			if (handler != null) handler(this, EventArgs.Empty);
		}

		private static Lines GetColumnLines(TableBorder border)
		{
			var columnsLines = Lines.None;

			if ((border & TableBorder.Left) != 0)
				columnsLines |= Lines.Start;
			if ((border & TableBorder.Right) != 0)
				columnsLines |= Lines.End;

			return columnsLines;
		}

		private static Lines GetRowLines(TableBorder border)
		{
			var rowLines = Lines.None;

			if ((border & TableBorder.Top) != 0)
				rowLines |= Lines.Start;
			if ((border & TableBorder.Bottom) != 0)
				rowLines |= Lines.End;

			return rowLines;
		}

		#endregion

		#region Nested type: Lines

		[Flags]
		private enum Lines
		{
			None = 0,
			Start = 1,
			End = 2
		}

		#endregion

		#region Nested type: TableDefinitionArray

		private class TableDefinitionArray
		{
			#region Fields

			private readonly int _count;
			private readonly float _defaultLength;
			private readonly float[] _lengthArray;
			private readonly float[] _offsetArray;
			private float _fullLength;
			private bool _isDirty;

			#endregion

			#region Ctors

			public TableDefinitionArray(int count, float defaultLength, Lines lines)
			{
				Lines = lines;
				_count = count;
				_defaultLength = defaultLength;

				_lengthArray = new float[count];
				_offsetArray = new float[count];

				UpdateSize(true);
			}

			#endregion

			#region Properties

			public int Count => _count;

			public float FullLength => _fullLength;

			public Lines Lines { get; set; }

			#endregion

			#region Methods

			public int GetDefinition(float offset)
			{
				if (offset < 0 || offset >= FullLength)
					return -1;

				var definition = Array.BinarySearch(_offsetArray, offset);
				if (definition < 0)
					definition = ~definition - 1;

				if (definition < 0 || definition > _offsetArray.Length)
					return -1;

				var definitionOffset = _offsetArray[definition];
				var definitionLength = _lengthArray[definition];

				return definitionOffset <= offset && offset <= definitionOffset + definitionLength ? definition : -1;
			}

			public float GetDefinitionLength(int index)
			{
				return _lengthArray[index];
			}

			public float GetDefinitionOffset(int index)
			{
				return _offsetArray[index];
			}

			public Range<int> GetDefinitionRange(float offset, float size)
			{
				var first = Array.BinarySearch(_offsetArray, offset);

				if (first < 0)
					first = ~first - 1;

				var last = float.IsPositiveInfinity(size) ? Count - 1 : Array.BinarySearch(_offsetArray, offset + size);

				if (last < 0)
					last = ~last - 1;

				return Zaaml.Core.Range.Create(Math.Max(first, 0), Math.Max(last, 0));
			}

			public void SetDefinitionLength(int index, float value)
			{
				_lengthArray[index] = value;
				_isDirty = true;
			}

			public void UpdateSize(bool setDefault = false)
			{
				if (setDefault)
					ResetSize();
				else
					CalcOffsetArray();
			}

			private void CalcOffsetArray()
			{
				if (_isDirty == false)
					return;

				try
				{
					if (_count == 0)
					{
						_fullLength = 0;
						return;
					}

					var offset = (Lines & Lines.Start) != 0 ? 1.0f : 0.0f;
					for (var i = 0; i < _count; i++)
					{
						_offsetArray[i] = offset;
						offset += _lengthArray[i] + 1.0f;
					}

					_fullLength = _offsetArray.Last() + _lengthArray.Last() + ((Lines & Lines.End) != 0 ? 1.0f : 0.0f);
				}
				finally
				{
					_isDirty = false;
				}
			}

			private void ResetSize()
			{
				for (var i = 0; i < _count; i++)
					_lengthArray[i] = _defaultLength;

				_isDirty = true;
				CalcOffsetArray();
			}

			#endregion
		}

		#endregion
	}
}