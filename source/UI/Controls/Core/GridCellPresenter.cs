// <copyright file="GridCellPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Controls.Core
{
	[ContentProperty(nameof(Cells))]
	public abstract class GridCellsPresenter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn> : FixedTemplateControl<TGridCellPanel>
		where TGridCellPresenter : GridCellsPresenter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellPanel : GridCellsPanel<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellCollection : GridCellCollection<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCell : GridCell<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellSplitter : GridCellSplitter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellColumnController : GridCellColumnController<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellColumn : GridCellColumn<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
	{
		private static readonly DependencyPropertyKey CellsPropertyKey = DPM.RegisterReadOnly<TGridCellCollection, TGridCellPresenter>
			("CellsInternal");

		public static readonly DependencyProperty AllowCellSplitterProperty = DPM.Register<bool, TGridCellPresenter>
			("AllowCellSplitter", default, d => d.OnAllowCellSplitterPropertyChangedPrivate);

		public static readonly DependencyProperty CellsProperty = CellsPropertyKey.DependencyProperty;

		static GridCellsPresenter()
		{
			UIElementUtils.OverrideFocusable<GridCellsPresenter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>>(false);
			ControlUtils.OverrideIsTabStop<GridCellsPresenter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>>(false);
			FrameworkElementUtils.OverrideVisualStyle<GridCellsPresenter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>>(null);
		}

		public bool AllowCellSplitter
		{
			get => (bool) GetValue(AllowCellSplitterProperty);
			set => SetValue(AllowCellSplitterProperty, value);
		}

		public TGridCellCollection Cells => this.GetValueOrCreate(CellsPropertyKey, CreateCellCollection);

		protected abstract TGridCellColumnController ColumnController { get; }

		internal TGridCellColumnController ColumnControllerInternal => ColumnController;

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			TemplateRoot.CellPresenter = (TGridCellPresenter) this;
		}

		protected abstract TGridCellCollection CreateCellCollection();

		private void OnAllowCellSplitterPropertyChangedPrivate(bool oldValue, bool newValue)
		{
			TemplateRoot?.InvalidateStructure();
		}

		protected override void UndoTemplateOverride()
		{
			TemplateRoot.CellPresenter = null;

			base.UndoTemplateOverride();
		}
	}
}