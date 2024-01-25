// <copyright file="TreeViewBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	public abstract class TreeViewBase : ControlViewBase
	{
		private static readonly DependencyPropertyKey TreeViewControlPropertyKey = DPM.RegisterReadOnly<TreeViewControl, TreeViewBase>
			("TreeViewControl", d => d.OnTreeViewControlPropertyChangedPrivate);

		public static readonly DependencyProperty TreeViewControlProperty = TreeViewControlPropertyKey.DependencyProperty;

		public TreeViewControl TreeViewControl
		{
			get => (TreeViewControl) GetValue(TreeViewControlProperty);
			internal set => this.SetReadOnlyValue(TreeViewControlPropertyKey, value);
		}

		protected abstract ControlTemplate GetTemplateCore(FrameworkElement frameworkElement);

		internal ControlTemplate GetTemplateInternal(FrameworkElement frameworkElement)
		{
			return GetTemplateCore(frameworkElement);
		}

		protected virtual void OnTreeViewControlChanged(TreeViewControl oldValue, TreeViewControl newValue)
		{
		}

		private void OnTreeViewControlPropertyChangedPrivate(TreeViewControl oldValue, TreeViewControl newValue)
		{
			OnTreeViewControlChanged(oldValue, newValue);
		}
	}
}