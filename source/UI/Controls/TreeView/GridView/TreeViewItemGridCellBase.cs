// <copyright file="TreeViewItemGridCellBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;

namespace Zaaml.UI.Controls.TreeView
{
	public abstract class TreeViewItemGridCellBase<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		: GridCell<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellsPresenter : GridCellsPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellsPanel : GridCellsPanel<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellCollection : GridCellCollection<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCell : GridCell<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
	{
		protected static readonly DependencyProperty ContentProperty = DPM.Register<object, TreeViewItemGridCellBase<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>>
			("Content", c => c.OnContentPropertyChangedPrivate);

		private DataTemplate _cellContentTemplate;

		internal TreeViewItemGridCellBase()
		{
			BorderThickness = new Thickness(1);
			Margin = new Thickness(-1, 0, 0, 0);
		}

		private DataTemplate CellContentTemplate
		{
			get => _cellContentTemplate;
			set
			{
				if (ReferenceEquals(_cellContentTemplate, value))
					return;

				_cellContentTemplate = value;

				ChildCore = null;
			}
		}

		protected abstract DataTemplate CellContentTemplateCore { get; }

		protected TreeGridViewColumn Column => (TreeGridViewColumn) ColumnInternal;

		protected object Content
		{
			get => GetValue(ContentProperty);
			set => SetValue(ContentProperty, value);
		}

		protected abstract Binding ContentBindingCore { get; }

		private IconContentPresenter ContentPresenter { get; set; }

		protected virtual Thickness PaddingCore => Padding;

		private TextBlock TextBlock { get; set; }

		private void EnsureContentBinding()
		{
			var binding = ContentBindingCore;
			var bindingExpression = this.ReadLocalBindingExpression(ContentProperty);

			if (bindingExpression != null && ReferenceEquals(bindingExpression.ParentBinding, binding))
				return;

			if (binding != null)
				SetBinding(ContentProperty, binding);
		}

		private void InvalidateCellMeasure()
		{
			var treeViewItem = (CellsPresenterInternal as TreeViewItemGridCellsPresenter)?.TreeViewItem;

			if (treeViewItem != null)
				this.InvalidateAncestorsMeasure(treeViewItem, true);
			else
				InvalidateMeasure();
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			EnsureContentBinding();

			CellContentTemplate = CellContentTemplateCore;

			if (ChildCore == null && (Content != null || CellContentTemplate != null))
				UpdateChild();

			UpdatePadding();

			return base.MeasureOverride(availableSize);
		}

		protected override void OnChildCoreChanged()
		{
			base.OnChildCoreChanged();

			InvalidateCellMeasure();
		}

		private void OnContentPropertyChangedPrivate(object oldContent, object newContent)
		{
			ChildCore = null;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == PaddingProperty)
				UpdatePadding();
		}

		private void UpdateChild()
		{
			var content = Content;
			var cellContentTemplate = CellContentTemplate;

			if (cellContentTemplate != null)
			{
				ContentPresenter ??= new IconContentPresenter
				{
					HorizontalAlignment = HorizontalAlignment.Stretch,
					VerticalAlignment = VerticalAlignment.Stretch,
					HorizontalContentAlignment = HorizontalAlignment.Stretch,
					VerticalContentAlignment = VerticalAlignment.Stretch
				};

				ContentPresenter.Content = Content;
				ContentPresenter.ContentTemplate = cellContentTemplate;

				ChildCore = ContentPresenter;
			}
			else
			{
				if (content is FrameworkElement fre)
					ChildCore = fre;
				else if (content != null)
				{
					TextBlock ??= new TextBlock();

					TextBlock.Text = content.ToString();

					ChildCore = TextBlock;
				}
				else
					ChildCore = null;
			}
		}

		private void UpdatePadding()
		{
			var border = Border;

			if (border == null)
				return;

			var padding = PaddingCore;

			if (border.Padding.Equals(padding) == false)
				border.Padding = padding;
		}

		protected override void UpdateStructure()
		{
			InvalidateCellMeasure();
		}
	}
}