// <copyright file="ListViewBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	public abstract class ListViewBase : ControlViewBase
	{
		private static readonly DependencyPropertyKey ListViewControlPropertyKey = DPM.RegisterReadOnly<ListViewControl, ListViewBase>
			("ListViewControl", d => d.OnListViewControlPropertyChangedPrivate);

		public static readonly DependencyProperty ListViewControlProperty = ListViewControlPropertyKey.DependencyProperty;

		public ListViewControl ListViewControl
		{
			get => (ListViewControl) GetValue(ListViewControlProperty);
			internal set => this.SetReadOnlyValue(ListViewControlPropertyKey, value);
		}

		protected abstract ControlTemplate GetTemplateCore(FrameworkElement frameworkElement);

		internal ControlTemplate GetTemplateInternal(FrameworkElement frameworkElement)
		{
			return GetTemplateCore(frameworkElement);
		}

		protected virtual void OnListViewControlChanged(ListViewControl oldValue, ListViewControl newValue)
		{
		}

		private void OnListViewControlPropertyChangedPrivate(ListViewControl oldValue, ListViewControl newValue)
		{
			OnListViewControlChanged(oldValue, newValue);
		}
	}
}