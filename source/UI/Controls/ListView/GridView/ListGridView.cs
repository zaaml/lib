// <copyright file="ListGridView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.ListView
{
	[ContentProperty(nameof(Columns))]
	public class ListGridView : ListViewBase
	{
		private static readonly DependencyPropertyKey ColumnsPropertyKey = DPM.RegisterReadOnly<ListGridViewColumnCollection, ListGridView>
			("ColumnsPrivate");

		public static readonly DependencyProperty TemplateProperty = DPM.RegisterAttached<ControlTemplate, ListGridView>
			("Template", OnTemplatePropertyChanged);

		public static readonly DependencyProperty ColumnsProperty = ColumnsPropertyKey.DependencyProperty;

		public ListGridViewColumnCollection Columns => this.GetValueOrCreate(ColumnsPropertyKey, () => new ListGridViewColumnCollection(this));

		internal ListViewItemGridController ItemController { get; private set; }

		public static ControlTemplate GetTemplate(DependencyObject element)
		{
			return (ControlTemplate) element.GetValue(TemplateProperty);
		}

		protected override ControlTemplate GetTemplateCore(FrameworkElement frameworkElement)
		{
			return GetTemplate(frameworkElement);
		}

		protected override void OnListViewControlChanged(ListViewControl oldValue, ListViewControl newValue)
		{
			base.OnListViewControlChanged(oldValue, newValue);

			ItemController = newValue != null ? new ListViewItemGridController(newValue) : null;
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