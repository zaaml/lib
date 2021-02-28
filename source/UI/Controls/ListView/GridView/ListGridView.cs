// <copyright file="ListGridView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Panels.Flexible;

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
			("ColumnWidth", GridColumn.DefaultWidth, d => d.OnColumnWidthPropertyChangedPrivate);

		public static readonly DependencyProperty ColumnMinWidthProperty = DPM.Register<FlexLength, ListGridView>
			("ColumnMinWidth", GridColumn.DefaultMinWidth, d => d.OnColumnMinWidthPropertyChangedPrivate);

		public static readonly DependencyProperty ColumnMaxWidthProperty = DPM.Register<FlexLength, ListGridView>
			("ColumnMaxWidth", GridColumn.DefaultMaxWidth, d => d.OnColumnMaxWidthPropertyChangedPrivate);

		public static readonly DependencyProperty ColumnsProperty = ColumnsPropertyKey.DependencyProperty;

		[TypeConverter(typeof(FlexLengthTypeConverter))]
		public FlexLength ColumnMaxWidth
		{
			get => (FlexLength) GetValue(ColumnMaxWidthProperty);
			set => SetValue(ColumnMaxWidthProperty, value);
		}

		[TypeConverter(typeof(FlexLengthTypeConverter))]
		public FlexLength ColumnMinWidth
		{
			get => (FlexLength) GetValue(ColumnMinWidthProperty);
			set => SetValue(ColumnMinWidthProperty, value);
		}

		public ListGridViewColumnCollection Columns => this.GetValueOrCreate(ColumnsPropertyKey, () => new ListGridViewColumnCollection(this));

		[TypeConverter(typeof(FlexLengthTypeConverter))]
		public FlexLength ColumnWidth
		{
			get => (FlexLength) GetValue(ColumnWidthProperty);
			set => SetValue(ColumnWidthProperty, value);
		}

		internal GridColumnWidthConstraints DefaultColumnWidthConstraints => new(ColumnWidth, ColumnMinWidth, ColumnMaxWidth);

		internal ListViewItemGridController GridController { get; private set; }

		public static ControlTemplate GetTemplate(DependencyObject element)
		{
			return (ControlTemplate) element.GetValue(TemplateProperty);
		}

		protected override ControlTemplate GetTemplateCore(FrameworkElement frameworkElement)
		{
			return GetTemplate(frameworkElement);
		}

		private void OnColumnMaxWidthPropertyChangedPrivate(FlexLength oldValue, FlexLength newValue)
		{
			GridController?.OnColumnWidthConstraintsChanged();
		}

		private void OnColumnMinWidthPropertyChangedPrivate(FlexLength oldValue, FlexLength newValue)
		{
			GridController?.OnColumnWidthConstraintsChanged();
		}

		private void OnColumnWidthPropertyChangedPrivate(FlexLength oldValue, FlexLength newValue)
		{
			GridController?.OnColumnWidthConstraintsChanged();
		}

		protected override void OnListViewControlChanged(ListViewControl oldValue, ListViewControl newValue)
		{
			base.OnListViewControlChanged(oldValue, newValue);

			GridController = newValue != null ? new ListViewItemGridController(newValue) : null;
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