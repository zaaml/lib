// <copyright file="GridCellPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Utils;

namespace Zaaml.UI.Controls.Core
{
	[ContentProperty(nameof(Cells))]
	public abstract class GridCellPresenter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn> : FixedTemplateControl<TGridCellPanel>
		where TGridCellPresenter : GridCellPresenter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellPanel : GridCellPanel<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellCollection : GridCellCollection<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCell : GridCell<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellSplitter : GridCellSplitter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellColumnController : GridCellColumnController<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellColumn : GridCellColumn<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
	{
		static GridCellPresenter()
		{
			UIElementUtils.OverrideFocusable<GridCellPresenter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>>(false);
			ControlUtils.OverrideIsTabStop<GridCellPresenter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>>(false);
			FrameworkElementUtils.OverrideVisualStyle<GridCellPresenter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>>(null);
		}

		private static readonly DependencyPropertyKey CellsPropertyKey = DPM.RegisterReadOnly<TGridCellCollection, TGridCellPresenter>
			("CellsInternal");

		public static readonly DependencyProperty AllowCellSplitterProperty = DPM.Register<bool, TGridCellPresenter>
			("AllowCellSplitter", default, d => d.OnAllowCellSplitterPropertyChangedPrivate);

		public bool AllowCellSplitter
		{
			get => (bool) GetValue(AllowCellSplitterProperty);
			set => SetValue(AllowCellSplitterProperty, value);
		}

		private void OnAllowCellSplitterPropertyChangedPrivate(bool oldValue, bool newValue)
		{
			TemplateRoot?.InvalidateStructure();
		}

		public static readonly DependencyProperty CellsProperty = CellsPropertyKey.DependencyProperty;

		public TGridCellCollection Cells => this.GetValueOrCreate(CellsPropertyKey, CreateCellCollection);

		protected abstract TGridCellColumnController ColumnController { get; }

		internal TGridCellColumnController ColumnControllerInternal => ColumnController;

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			TemplateRoot.CellPresenter = (TGridCellPresenter) this;
		}

		protected abstract TGridCellCollection CreateCellCollection();

		protected override void UndoTemplateOverride()
		{
			TemplateRoot.CellPresenter = null;

			base.UndoTemplateOverride();
		}
	}
}