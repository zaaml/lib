// <copyright file="ListGridView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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

namespace Zaaml.UI.Controls.ListView
{
	[ContentProperty(nameof(Columns))]
	public class ListGridView : ListViewBase
	{
		private static readonly DependencyPropertyKey ColumnsPropertyKey = DPM.RegisterReadOnly<ListGridViewColumnCollection, ListGridView>
			("ColumnsPrivate");

		public static readonly DependencyProperty TemplateProperty = DPM.RegisterAttached<ControlTemplate, ListGridView>
			("Template", OnTemplatePropertyChanged);

		public static readonly DependencyProperty ColumnWidthProperty = DPM.Register<FlexLength, ListGridView>
			("ColumnWidth", GridViewColumn.DefaultWidth, d => d.OnColumnWidthPropertyChangedPrivate);

		public static readonly DependencyProperty ColumnMinWidthProperty = DPM.Register<FlexLength, ListGridView>
			("ColumnMinWidth", GridViewColumn.DefaultMinWidth, d => d.OnColumnMinWidthPropertyChangedPrivate);

		public static readonly DependencyProperty ColumnMaxWidthProperty = DPM.Register<FlexLength, ListGridView>
			("ColumnMaxWidth", GridViewColumn.DefaultMaxWidth, d => d.OnColumnMaxWidthPropertyChangedPrivate);

		public static readonly DependencyProperty CellStyleProperty = DPM.Register<Style, ListGridView>
			("CellStyle", d => d.OnCellStylePropertyChangedPrivate);

		public static readonly DependencyProperty HeaderStyleProperty = DPM.Register<Style, ListGridView>
			("HeaderStyle", d => d.OnHeaderStylePropertyChangedPrivate);

		public static readonly DependencyProperty CellAppearanceProperty = DPM.Register<ListGridViewCellAppearance, ListGridView>
			("CellAppearance", default, d => d.OnCellAppearancePropertyChangedPrivate);

		public static readonly DependencyProperty HeaderAppearanceProperty = DPM.Register<ListGridViewHeaderAppearance, ListGridView>
			("HeaderAppearance", default, d => d.OnHeaderAppearancePropertyChangedPrivate);

		public static readonly DependencyProperty ColumnsProperty = ColumnsPropertyKey.DependencyProperty;

		public ListGridViewCellAppearance CellAppearance
		{
			get => (ListGridViewCellAppearance)GetValue(CellAppearanceProperty);
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

		public ListGridViewColumnCollection Columns => this.GetValueOrCreate(ColumnsPropertyKey, () => new ListGridViewColumnCollection(this));

		[TypeConverter(typeof(FlexLengthTypeConverter))]
		public FlexLength ColumnWidth
		{
			get => (FlexLength)GetValue(ColumnWidthProperty);
			set => SetValue(ColumnWidthProperty, value);
		}

		internal GridViewColumnWidthConstraints DefaultColumnWidthConstraints => new(ColumnWidth, ColumnMinWidth, ColumnMaxWidth);

		public ListGridViewHeaderAppearance HeaderAppearance
		{
			get => (ListGridViewHeaderAppearance)GetValue(HeaderAppearanceProperty);
			set => SetValue(HeaderAppearanceProperty, value);
		}

		public Style HeaderStyle
		{
			get => (Style)GetValue(HeaderStyleProperty);
			set => SetValue(HeaderStyleProperty, value);
		}

		internal ListGridViewController ViewController { get; private set; }

		public static ControlTemplate GetTemplate(DependencyObject element)
		{
			return (ControlTemplate)element.GetValue(TemplateProperty);
		}

		protected override ControlTemplate GetTemplateCore(FrameworkElement frameworkElement)
		{
			return GetTemplate(frameworkElement);
		}

		private void OnCellAppearancePropertyChangedPrivate(ListGridViewCellAppearance oldValue, ListGridViewCellAppearance newValue)
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

		private void OnHeaderAppearancePropertyChangedPrivate(ListGridViewHeaderAppearance oldValue, ListGridViewHeaderAppearance newValue)
		{
		}

		private void OnHeaderStylePropertyChangedPrivate(Style oldValue, Style newValue)
		{
			foreach (var column in Columns)
				column.UpdateActualHeaderStyle();
		}

		protected override void OnListViewControlChanged(ListViewControl oldValue, ListViewControl newValue)
		{
			base.OnListViewControlChanged(oldValue, newValue);

			ViewController = newValue != null ? new ListGridViewController(newValue) : null;
		}

		private static void OnTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is ListViewItem listViewItem)
				listViewItem.UpdateViewTemplate();
			else if (d is ListViewControl listViewControl)
				listViewControl.UpdateViewTemplate();
		}

		public static void SetTemplate(DependencyObject element, ControlTemplate value)
		{
			element.SetValue(TemplateProperty, value);
		}
	}
}