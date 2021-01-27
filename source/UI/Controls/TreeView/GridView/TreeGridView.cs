// <copyright file="TreeGridView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

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

		public static readonly DependencyProperty ActualExpanderColumnProperty = ActualExpanderColumnPropertyKey.DependencyProperty;

		public static readonly DependencyProperty ColumnsProperty = ColumnsPropertyKey.DependencyProperty;

		public TreeGridViewColumn ActualExpanderColumn
		{
			get => (TreeGridViewColumn) GetValue(ActualExpanderColumnProperty);
			private set => this.SetReadOnlyValue(ActualExpanderColumnPropertyKey, value);
		}

		public TreeGridViewColumnCollection Columns => this.GetValueOrCreate(ColumnsPropertyKey, () => new TreeGridViewColumnCollection(this));

		public TreeGridViewColumn ExpanderColumn
		{
			get => (TreeGridViewColumn) GetValue(ExpanderColumnProperty);
			set => SetValue(ExpanderColumnProperty, value);
		}

		internal TreeViewItemGridController ItemController { get; private set; }

		public static ControlTemplate GetTemplate(DependencyObject element)
		{
			return (ControlTemplate) element.GetValue(TemplateProperty);
		}

		protected override ControlTemplate GetTemplateCore(FrameworkElement frameworkElement)
		{
			return GetTemplate(frameworkElement);
		}

		private void OnActualExpanderColumnPropertyChangedPrivate(TreeGridViewColumn oldValue, TreeGridViewColumn newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			var controller = ItemController;

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

		private void OnExpanderColumnPropertyChangedPrivate(TreeGridViewColumn oldValue, TreeGridViewColumn newValue)
		{
			UpdateActualExpanderColumn();
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

			ItemController = newValue != null ? new TreeViewItemGridController(newValue) : null;

			UpdateActualExpanderColumn();
		}

		public static void SetTemplate(DependencyObject element, ControlTemplate value)
		{
			element.SetValue(TemplateProperty, value);
		}

		internal void UpdateActualExpanderColumn()
		{
			ActualExpanderColumn = ExpanderColumn ?? (TreeGridViewColumn) ItemController?.GetColumn(0);
		}
	}
}