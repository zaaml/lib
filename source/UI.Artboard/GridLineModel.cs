// <copyright file="GridLineModel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Artboard
{
	public abstract class GridLineModel : InheritanceContextObject
	{
		internal event EventHandler ModelChanged;

		internal int Version { get; private set; }

		internal void OnModelChangedInternal()
		{
			Version++;

			ModelChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	[ContentProperty("GridLines")]
	public abstract class GridLineModel<TGridLineCollection, TGridLine> : GridLineModel where TGridLineCollection : GridLineCollection<TGridLine> where TGridLine : GridLine
	{
		private static readonly DependencyPropertyKey GridLinesPropertyKey = DPM.RegisterReadOnly<TGridLineCollection, GridLineModel<TGridLineCollection, TGridLine>>
			("GridLinesPrivate");

		public static readonly DependencyProperty GridLinesProperty = GridLinesPropertyKey.DependencyProperty;

		public static readonly DependencyProperty GridSizeProperty = DPM.Register<double, GridLineModel<TGridLineCollection, TGridLine>>
			("GridSize", default, d => d.OnGridSizePropertyChangedPrivate);

		public TGridLineCollection GridLines => this.GetValueOrCreate(GridLinesPropertyKey, CreateGridLineCollection);

		public double GridSize
		{
			get => (double) GetValue(GridSizeProperty);
			set => SetValue(GridSizeProperty, value);
		}

		protected abstract TGridLineCollection CreateGridLineCollection();

		protected virtual void OnGridSizeChanged()
		{
			OnModelChangedInternal();
		}

		private void OnGridSizePropertyChangedPrivate(double oldValue, double newValue)
		{
			OnGridSizeChanged();
		}
	}

	public abstract class GridLineCollection<TGridLine> : DependencyObjectCollectionBase<TGridLine> where TGridLine : GridLine
	{
		private readonly GridLineModel _model;
		private readonly List<TGridLine> _sortedLines = new List<TGridLine>();

		internal event EventHandler<EventArgs<TGridLine>> LineAdded;
		internal event EventHandler<EventArgs<TGridLine>> LineRemoved;

		protected GridLineCollection(GridLineModel model)
		{
			_model = model;
			SortedLines = new ReadOnlyCollection<TGridLine>(_sortedLines);
		}

		internal IReadOnlyList<TGridLine> SortedLines { get; }

		internal TGridLine GetDefinition(int lineIndex)
		{
			return SortedLines[GetSortedDefinitionIndex(lineIndex)];
		}

		internal int GetSortedDefinitionIndex(int lineIndex)
		{
			for (var index = SortedLines.Count - 1; index >= 0; index--)
			{
				var stepDefinition = SortedLines[index];

				if (lineIndex % stepDefinition.Step != 0)
					continue;

				return index;
			}

			return 0;
		}

		protected override void OnItemAdded(TGridLine gridLine)
		{
			base.OnItemAdded(gridLine);

			_sortedLines.Add(gridLine);

			SortGridLines();

			LineAdded?.Invoke(this, new EventArgs<TGridLine>(gridLine));

			_model.OnModelChangedInternal();
		}

		protected override void OnItemRemoved(TGridLine gridLine)
		{
			base.OnItemRemoved(gridLine);

			_sortedLines.Remove(gridLine);

			SortGridLines();

			LineRemoved?.Invoke(this, new EventArgs<TGridLine>(gridLine));

			_model.OnModelChangedInternal();
		}

		private void SortGridLines()
		{
			_sortedLines.Sort(DelegateComparer.Create<TGridLine>((l, r) => Comparer<int>.Default.Compare(l.Step, r.Step)));
		}
	}
}