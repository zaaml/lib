// <copyright file="GridViewCellsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.ScrollView;

namespace Zaaml.UI.Controls.Core.GridView
{
	public abstract class GridViewCellsPresenter : FixedTemplateControl
	{
		public static readonly DependencyProperty AllowCellSplitterProperty = DPM.Register<bool, GridViewCellsPresenter>
			("AllowCellSplitter", default, d => d.OnAllowCellSplitterPropertyChangedPrivate);

		private ScrollViewControl _scrollViewControl;

		protected GridViewCellsPresenter() : base(TemplateKind.None)
		{
			ClipToBounds = true;
		}

		public bool AllowCellSplitter
		{
			get => (bool)GetValue(AllowCellSplitterProperty);
			set => SetValue(AllowCellSplitterProperty, value.Box());
		}

		internal Size ArrangeBounds { get; set; }

		protected GridViewCellsPanel CellsPanelCore => TemplateRoot;

		internal GridViewCellsPanel CellsPanelInternal => CellsPanelCore;

		internal GridViewColumnController ColumnControllerInternal => ViewController.ColumnController;

		protected abstract Type GridCellsPanelType { get; }

		internal ScrollViewControl ScrollViewControl
		{
			get => _scrollViewControl;
			set
			{
				if (ReferenceEquals(_scrollViewControl, value))
					return;

				if (_scrollViewControl != null)
					_scrollViewControl.DependencyPropertyChangedInternal -= OnScrollViewDependencyPropertyChanged;

				_scrollViewControl = value;

				if (_scrollViewControl != null)
					_scrollViewControl.DependencyPropertyChangedInternal += OnScrollViewDependencyPropertyChanged;
			}
		}

		protected GridViewCellsPanel TemplateRoot
		{
			get
			{
				if (this.GetImplementationRoot() is GridViewCellsPanel gridCellsPanel)
					return gridCellsPanel;

				ApplyTemplatePrivate();

				gridCellsPanel = this.GetImplementationRoot() as GridViewCellsPanel;

				return gridCellsPanel;
			}
		}

		protected abstract GridViewController ViewController { get; }

		internal GridViewController ViewControllerInternal => ViewController;

		private void ApplyTemplatePrivate()
		{
			TemplateInternal = ControlTemplateBuilder.GetTemplate(GridCellsPanelType);

			ApplyTemplate();
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			ArrangeBounds = arrangeBounds;

			var arrangeResult = base.ArrangeOverride(arrangeBounds);

			return arrangeResult;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			ApplyTemplatePrivate();

			var measureResult = base.MeasureOverride(availableSize);

			return measureResult;
		}

		private void OnAllowCellSplitterPropertyChangedPrivate(bool oldValue, bool newValue)
		{
			CellsPanelCore?.InvalidateStructure();
		}

		protected override void OnLoaded()
		{
			base.OnLoaded();

			ApplyTemplatePrivate();
		}

		private void OnScrollViewDependencyPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.Property == ScrollViewControlBase.HorizontalOffsetProperty ||
			    e.Property == ScrollViewControlBase.ViewportWidthProperty ||
			    e.Property == ScrollViewControlBase.ExtentWidthProperty)
				UpdateScrollInfo();
		}

		private void UpdateScrollInfo()
		{
			if (ScrollViewControl == null)
				return;

			var cellsPanel = CellsPanelCore;

			if (cellsPanel == null)
				return;

			ViewController.ScrollInfo = ScrollViewControl.ActualScrollInfoInternal.Axis(Orientation.Horizontal);
		}
	}

	[ContentProperty(nameof(Cells))]
	public abstract class GridViewCellsPresenter<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell> : GridViewCellsPresenter
		where TGridColumn : GridViewColumn
		where TGridCellsPresenter : GridViewCellsPresenter<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellsPanel : GridViewCellsPanel<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellCollection : GridViewCellCollection<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCell : GridViewCell<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
	{
		private static readonly DependencyPropertyKey CellsPropertyKey = DPM.RegisterReadOnly<TGridCellCollection, TGridCellsPresenter>
			("CellsPrivate");

		public static readonly DependencyProperty CellsProperty = CellsPropertyKey.DependencyProperty;

		private GridViewColumnCollection<TGridColumn> _columns;

		static GridViewCellsPresenter()
		{
			UIElementUtils.OverrideFocusable<GridViewCellsPresenter<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>>(false);
			ControlUtils.OverrideIsTabStop<GridViewCellsPresenter<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>>(false);
			FrameworkElementUtils.OverrideVisualStyle<GridViewCellsPresenter<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>>(null);
		}

		public TGridCellCollection Cells => this.GetValueOrCreate(CellsPropertyKey, CreateCellCollection);

		protected TGridCellsPanel CellsPanel => (TGridCellsPanel)TemplateRoot;

		protected GridViewColumnCollection<TGridColumn> Columns
		{
			get => _columns;
			set
			{
				if (ReferenceEquals(_columns, value))
					return;

				DestroyCells();

				if (_columns != null)
				{
					_columns.ColumnRemoved -= OnColumnRemoved;
					_columns.ColumnAdded -= OnColumnAdded;
				}

				_columns = value;

				if (_columns != null)
				{
					_columns.ColumnRemoved += OnColumnRemoved;
					_columns.ColumnAdded += OnColumnAdded;
				}

				CreateCells();
			}
		}

		protected override Type GridCellsPanelType => typeof(TGridCellsPanel);

		protected abstract void AddCellGeneratorChanged(TGridColumn gridColumn, EventHandler cellGeneratorChanged);

		protected virtual void AttachCell(TGridCell gridCell)
		{
		}

		private TGridCell CreateCell(TGridColumn gridColumn)
		{
			var cell = GetCellGenerator(gridColumn).CreateCellCore();

			cell.AttachColumnInternal(gridColumn);

			AttachCell(cell);

			return cell;
		}

		protected abstract TGridCellCollection CreateCellCollection();

		private void CreateCells()
		{
			try
			{
				if (Columns == null)
					return;

				foreach (var column in Columns)
					Cells.Add(CreateCell(column));
			}
			finally
			{
				OnViewColumnsChanged();
			}
		}

		private void DestroyCell(TGridCell cell)
		{
			var gridColumn = cell.GridColumn;

			DetachCell(cell);

			cell.DetachColumnInternal(gridColumn);

			GetCellGenerator(gridColumn).DisposeCellCore(cell);
		}

		private void DestroyCells()
		{
			try
			{
				foreach (var cell in Cells)
					DestroyCell(cell);

				Cells.Clear();
			}
			finally
			{
				OnViewColumnsChanged();
			}
		}

		protected virtual void DetachCell(TGridCell gridCell)
		{
		}

		protected abstract GridViewCellGenerator<TGridCell> GetCellGenerator(TGridColumn gridColumn);

		private void OnColumnAdded(object sender, GridViewColumnEventArgs<TGridColumn> e)
		{
			try
			{
				var columnIndex = Columns.IndexOf(e.Column);

				Cells.Insert(columnIndex, CreateCell(e.Column));
			}
			finally
			{
				OnViewColumnsChanged();
			}
		}

		private void OnColumnRemoved(object sender, GridViewColumnEventArgs<TGridColumn> e)
		{
			try
			{
				foreach (var cell in Cells)
				{
					if (ReferenceEquals(cell.GridColumn, e.Column) == false)
						continue;

					DestroyCell(cell);

					Cells.Remove(cell);

					return;
				}
			}
			finally
			{
				OnViewColumnsChanged();
			}
		}

		private void OnViewColumnsChanged()
		{
			InvalidateMeasure();
		}

		protected abstract void RemoveCellGeneratorChanged(TGridColumn gridColumn, EventHandler cellGeneratorChanged);
	}
}