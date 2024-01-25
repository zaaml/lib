// <copyright file="ListGridViewHeaderElement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.ListView
{
	public class ListGridViewHeaderElement : ListGridViewElement
	{
		static ListGridViewHeaderElement()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ListGridViewHeaderElement>();
		}

		public ListGridViewHeaderElement(ListGridViewHeadersPanel headersPanel)
		{
			this.OverrideStyleKey<ListGridViewHeaderElement>();

			HeadersPanel = headersPanel;
		}

		protected override GridViewLines GridViewLines => HeadersPanel.View?.HeaderAppearance?.ActualGridLines ?? GridViewLines.Both;

		public ListGridViewHeadersPanel HeadersPanel { get; }

		protected override Thickness GetBorderThickness(GridViewLines gridViewLines)
		{
			var vt = gridViewLines.ShowVertical() ? 1 : 0;
			var ht = gridViewLines.ShowHorizontal() ? 1 : 0;

			return BorderThickness = new Thickness(0, 0, 0, ht);
		}
	}
}