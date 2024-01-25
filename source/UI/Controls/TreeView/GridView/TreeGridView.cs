// <copyright file="TreeGridView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core.GridView;
using Zaaml.UI.Panels.Flexible;
using GridViewColumn = Zaaml.UI.Controls.Core.GridView.GridViewColumn;

namespace Zaaml.UI.Controls.TreeView
{
	[ContentProperty(nameof(Columns))]
	public class TreeGridView : TreeViewBase
	{
		private static readonly DependencyPropertyKey ColumnsPropertyKey = DPM.RegisterReadOnly<TreeGridViewColumnCollection, TreeGridView>
			("ColumnsPrivate");

		public static readonly DependencyProperty TemplateProperty = DPM.RegisterAttached<ControlTemplate, TreeGridView>
			("Template", OnTemplatePropertyChanged);

		public static readonly DependencyProperty ExpanderColumnProperty = DPM.Register<TreeGridViewColumn, TreeGridView>
			("ExpanderColumn", d => d.OnExpanderColumnPropertyChangedPrivate);

		private static readonly DependencyPropertyKey ActualExpanderColumnPropertyKey = DPM.RegisterReadOnly<TreeGridViewColumn, TreeGridView>
			("ActualExpanderColumn", d => d.OnActualExpanderColumnPropertyChangedPrivate);

		public static readonly DependencyProperty ColumnWidthProperty = DPM.Register<FlexLength, TreeGridView>
			("ColumnWidth", GridViewColumn.DefaultWidth, d => d.OnColumnWidthPropertyChangedPrivate);

		public static readonly DependencyProperty ColumnMinWidthProperty = DPM.Register<FlexLength, TreeGridView>
			("ColumnMinWidth", GridViewColumn.DefaultMinWidth, d => d.OnColumnMinWidthPropertyChangedPrivate);

		public static readonly DependencyProperty ColumnMaxWidthProperty = DPM.Register<FlexLength, TreeGridView>
			("ColumnMaxWidth", GridViewColumn.DefaultMaxWidth, d => d.OnColumnMaxWidthPropertyChangedPrivate);

		public static readonly DependencyProperty CellStyleProperty = DPM.Register<Style, TreeGridView>
			("CellStyle", d => d.OnCellStylePropertyChangedPrivate);

		public static readonly DependencyProperty HeaderStyleProperty = DPM.Register<Style, TreeGridView>
			("HeaderStyle", d => d.OnHeaderStylePropertyChangedPrivate);

		public static readonly DependencyProperty CellAppearanceProperty = DPM.Register<TreeGridViewCellAppearance, TreeGridView>
			("CellAppearance", default, d => d.OnCellAppearancePropertyChangedPrivate);

		public static readonly DependencyProperty HeaderAppearanceProperty = DPM.Register<TreeGridViewHeaderAppearance, TreeGridView>
			("HeaderAppearance", default, d => d.OnHeaderAppearancePropertyChangedPrivate);

		public static readonly DependencyProperty ColumnsProperty = ColumnsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ActualExpanderColumnProperty = ActualExpanderColumnPropertyKey.DependencyProperty;

		public TreeGridViewColumn ActualExpanderColumn
		{
			get => (TreeGridViewColumn)GetValue(ActualExpanderColumnProperty);
			private set => this.SetReadOnlyValue(ActualExpanderColumnPropertyKey, value);
		}

		public TreeGridViewCellAppearance CellAppearance
		{
			get => (TreeGridViewCellAppearance)GetValue(CellAppearanceProperty);
			set => SetValue(CellAppearanceProperty, value);
		}

		public Style CellStyle
		{
			get => (Style)GetValue(CellStyleProperty);
			set => SetValue(CellStyleProperty, value);
		}

		private GridViewColumnController ColumnController => ViewController?.ColumnController;

		[TypeConverter(typeof(FlexLengthTypeConverter))]
		public FlexLength ColumnMaxWidth
		{
			get => (FlexLength)GetValue(ColumnMaxWidthProperty);
			set => SetValue(ColumnMaxWidthProperty, value);
		}

		[TypeConverter(typeof(FlexLengthTypeConverter))]
		public FlexLength ColumnMinWidth
		{
			get => (FlexLength)GetValue(ColumnMinWidthProperty);
			set => SetValue(ColumnMinWidthProperty, value);
		}

		public TreeGridViewColumnCollection Columns => this.GetValueOrCreate(ColumnsPropertyKey, () => new TreeGridViewColumnCollection(this));

		[TypeConverter(typeof(FlexLengthTypeConverter))]
		public FlexLength ColumnWidth
		{
			get => (FlexLength)GetValue(ColumnWidthProperty);
			set => SetValue(ColumnWidthProperty, value);
		}

		internal GridViewColumnWidthConstraints DefaultColumnWidthConstraints => new(ColumnWidth, ColumnMinWidth, ColumnMaxWidth);

		public TreeGridViewColumn ExpanderColumn
		{
			get => (TreeGridViewColumn)GetValue(ExpanderColumnProperty);
			set => SetValue(ExpanderColumnProperty, value);
		}

		public TreeGridViewHeaderAppearance HeaderAppearance
		{
			get => (TreeGridViewHeaderAppearance)GetValue(HeaderAppearanceProperty);
			set => SetValue(HeaderAppearanceProperty, value);
		}

		public Style HeaderStyle
		{
			get => (Style)GetValue(HeaderStyleProperty);
			set => SetValue(HeaderStyleProperty, value);
		}

		internal TreeGridViewController ViewController { get; private set; }

		public static ControlTemplate GetTemplate(DependencyObject element)
		{
			return (ControlTemplate)element.GetValue(TemplateProperty);
		}

		protected override ControlTemplate GetTemplateCore(FrameworkElement frameworkElement)
		{
			return GetTemplate(frameworkElement);
		}

		private void OnActualExpanderColumnPropertyChangedPrivate(TreeGridViewColumn oldValue, TreeGridViewColumn newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			var controller = ColumnController;

			if (oldValue != null)
			{
				oldValue.IsExpanderColumn = false;
				controller?.OnCellStructurePropertyChanged(oldValue);
			}

			if (newValue != null)
			{
				newValue.IsExpanderColumn = true;
				controller?.OnCellStructurePropertyChanged(newValue);
			}
		}


		private void OnCellAppearancePropertyChangedPrivate(TreeGridViewCellAppearance oldValue, TreeGridViewCellAppearance newValue)
		{
		}

		private void OnCellStylePropertyChangedPrivate(Style oldValue, Style newValue)
		{
			foreach (var column in Columns)
				column.UpdateActualCellStyle();
		}

		private void OnColumnMaxWidthPropertyChangedPrivate(FlexLength oldValue, FlexLength newValue)
		{
			ColumnController?.OnColumnWidthConstraintsChanged();
		}

		private void OnColumnMinWidthPropertyChangedPrivate(FlexLength oldValue, FlexLength newValue)
		{
			ColumnController?.OnColumnWidthConstraintsChanged();
		}

		private void OnColumnWidthPropertyChangedPrivate(FlexLength oldValue, FlexLength newValue)
		{
			ColumnController?.OnColumnWidthConstraintsChanged();
		}

		private void OnExpanderColumnPropertyChangedPrivate(TreeGridViewColumn oldValue, TreeGridViewColumn newValue)
		{
			UpdateActualExpanderColumn();
		}

		private void OnHeaderAppearancePropertyChangedPrivate(TreeGridViewHeaderAppearance oldValue, TreeGridViewHeaderAppearance newValue)
		{
		}

		private void OnHeaderStylePropertyChangedPrivate(Style oldValue, Style newValue)
		{
			foreach (var column in Columns)
				column.UpdateActualHeaderStyle();
		}

		private static void OnTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TreeViewItem treeViewItem)
				treeViewItem.UpdateViewTemplate();
			else if (d is TreeViewControl treeViewControl)
				treeViewControl.UpdateViewTemplate();
		}

		protected override void OnTreeViewControlChanged(TreeViewControl oldValue, TreeViewControl newValue)
		{
			base.OnTreeViewControlChanged(oldValue, newValue);

			ViewController = newValue != null ? new TreeGridViewController(newValue) : null;

			UpdateActualExpanderColumn();
		}

		public static void SetTemplate(DependencyObject element, ControlTemplate value)
		{
			element.SetValue(TemplateProperty, value);
		}

		internal void UpdateActualExpanderColumn()
		{
			ActualExpanderColumn = ExpanderColumn ?? (TreeGridViewColumn)ColumnController?.GetColumn(0);
		}
	}
}