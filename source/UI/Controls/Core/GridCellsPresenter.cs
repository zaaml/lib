// <copyright file="GridCellsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.ScrollView;

namespace Zaaml.UI.Controls.Core
{
	public abstract class GridCellsPresenter : FixedTemplateControl
	{
		public static readonly DependencyProperty AllowCellSplitterProperty = DPM.Register<bool, GridCellsPresenter>
			("AllowCellSplitter", default, d => d.OnAllowCellSplitterPropertyChangedPrivate);

		private ScrollViewControl _scrollViewControl;

		protected GridCellsPresenter() : base(TemplateKind.None)
		{
			ClipToBounds = true;
		}

		public bool AllowCellSplitter
		{
			get => (bool) GetValue(AllowCellSplitterProperty);
			set => SetValue(AllowCellSplitterProperty, value);
		}

		internal Size ArrangeBounds { get; set; }

		protected GridCellsPanel CellsPanelCore => TemplateRoot;

		internal GridCellsPanel CellsPanelInternal => CellsPanelCore;

		protected abstract GridController Controller { get; }

		internal GridController ControllerInternal => Controller;

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

		protected GridCellsPanel TemplateRoot
		{
			get
			{
				if (this.GetImplementationRoot() is GridCellsPanel gridCellsPanel)
					return gridCellsPanel;

				ApplyTemplatePrivate();

				gridCellsPanel = this.GetImplementationRoot() as GridCellsPanel;

				return gridCellsPanel;
			}
		}

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

		protected override void OnChildDesiredSizeChanged(UIElement child)
		{
			base.OnChildDesiredSizeChanged(child);
		}

		private void OnScrollViewDependencyPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.Property == ScrollViewControlBase.HorizontalOffsetProperty)
				UpdateHorizontalOffset();
		}

		private void UpdateHorizontalOffset()
		{
			var horizontalOffset = ScrollViewControl?.HorizontalOffset ?? 0.0;
			var cellsPanel = CellsPanelCore;

			if (cellsPanel != null)
				cellsPanel.HorizontalOffset = horizontalOffset;
		}
	}

	[ContentProperty(nameof(Cells))]
	public abstract class GridCellsPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell> : GridCellsPresenter
		where TGridCellsPresenter : GridCellsPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellsPanel : GridCellsPanel<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellCollection : GridCellCollection<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCell : GridCell<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
	{
		private static readonly DependencyPropertyKey CellsPropertyKey = DPM.RegisterReadOnly<TGridCellCollection, TGridCellsPresenter>
			("CellsPrivate");

		public static readonly DependencyProperty CellsProperty = CellsPropertyKey.DependencyProperty;

		static GridCellsPresenter()
		{
			UIElementUtils.OverrideFocusable<GridCellsPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>>(false);
			ControlUtils.OverrideIsTabStop<GridCellsPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>>(false);
			FrameworkElementUtils.OverrideVisualStyle<GridCellsPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>>(null);
		}

		public TGridCellCollection Cells => this.GetValueOrCreate(CellsPropertyKey, CreateCellCollection);

		protected TGridCellsPanel CellsPanel => (TGridCellsPanel) TemplateRoot;

		protected override Type GridCellsPanelType => typeof(TGridCellsPanel);

		protected abstract TGridCellCollection CreateCellCollection();
	}
}