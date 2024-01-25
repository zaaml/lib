// <copyright file="ListViewGroupItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.ListView
{
	public class ListViewGroupItem : ListViewItem
	{
		public static readonly DependencyProperty IsExpandedProperty = DPM.Register<bool, ListViewGroupItem>
			("IsExpanded", default, d => d.OnIsExpandedPropertyChangedPrivate);

		public bool IsExpanded
		{
			get => (bool) GetValue(IsExpandedProperty);
			set => SetValue(IsExpandedProperty, value.Box());
		}

		private void OnIsExpandedPropertyChangedPrivate(bool oldValue, bool newValue)
		{
		}
	}
}