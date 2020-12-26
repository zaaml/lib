// <copyright file="ListViewItemGlyphPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	public class ListViewItemGlyphPresenter : ContentPresenter
	{
		public ListViewItemGlyphPresenter()
		{
			EmptyVisibility = Visibility.Collapsed;
		}
	}
}