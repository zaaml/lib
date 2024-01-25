// <copyright file="TreeViewItemExpander.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Data;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Runtime;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	public class TreeViewItemExpander : Control
	{
		public static readonly DependencyProperty IsExpandedProperty = DPM.Register<bool, TreeViewItemExpander>
			("IsExpanded");

		private static readonly DependencyPropertyKey TreeViewItemPropertyKey = DPM.RegisterReadOnly<TreeViewItem, TreeViewItemExpander>
			("TreeViewItem", d => d.OnTreeViewItemPropertyChangedPrivate);

		private static readonly DependencyPropertyKey ActualVisibilityPropertyKey = DPM.RegisterReadOnly<Visibility, TreeViewItemExpander>
			("ActualVisibility", Visibility.Collapsed);

		public static readonly DependencyProperty ActualVisibilityProperty = ActualVisibilityPropertyKey.DependencyProperty;

		public static readonly DependencyProperty TreeViewItemProperty = TreeViewItemPropertyKey.DependencyProperty;
		private static readonly PropertyPath TreeViewItemIsExpandedPropertyPath = new PropertyPath(TreeViewItem.IsExpandedProperty);

		static TreeViewItemExpander()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TreeViewItemExpander>();
		}

		public TreeViewItemExpander()
		{
			this.OverrideStyleKey<TreeViewItemExpander>();
		}

		public Visibility ActualVisibility
		{
			get => (Visibility) GetValue(ActualVisibilityProperty);
			private set => this.SetReadOnlyValue(ActualVisibilityPropertyKey, value.Box());
		}

		public bool IsExpanded
		{
			get => (bool) GetValue(IsExpandedProperty);
			set => SetValue(IsExpandedProperty, value.Box());
		}

		public TreeViewItem TreeViewItem
		{
			get => (TreeViewItem) GetValue(TreeViewItemProperty);
			internal set => this.SetReadOnlyValue(TreeViewItemPropertyKey, value);
		}

		private void OnTreeViewItemPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.Property == TreeViewItem.HasItemsProperty)
				UpdateVisibility();
		}

		private void OnTreeViewItemPropertyChangedPrivate(TreeViewItem oldValue, TreeViewItem newValue)
		{
			if (oldValue != null)
			{
				oldValue.DependencyPropertyChangedInternal -= OnTreeViewItemPropertyChanged;

				ClearValue(IsExpandedProperty);
			}

			if (newValue != null)
			{
				newValue.DependencyPropertyChangedInternal += OnTreeViewItemPropertyChanged;

				SetBinding(IsExpandedProperty, new Binding {Path = TreeViewItemIsExpandedPropertyPath, Source = newValue, Mode = BindingMode.TwoWay});
			}

			UpdateVisibility();
		}

		private void UpdateVisibility()
		{
			var actualVisibility = TreeViewItem?.HasItems == false ? Visibility.Collapsed : Visibility.Visible;

			if (ActualVisibility != actualVisibility)
				ActualVisibility = actualVisibility;
		}
	}
}