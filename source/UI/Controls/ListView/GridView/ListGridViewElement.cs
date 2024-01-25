// <copyright file="ListGridViewElement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.ListView
{
	public abstract class ListGridViewElement : GridViewElement
	{
		protected abstract GridViewLines GridViewLines { get; }

		protected virtual Thickness GetBorderThickness(GridViewLines gridViewLines)
		{
			var vt = gridViewLines.ShowVertical() ? 1 : 0;
			var ht = gridViewLines.ShowHorizontal() ? 1 : 0;

			return BorderThickness = new Thickness(vt, ht, vt, ht);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			UpdateBorders();

			return base.MeasureOverride(availableSize);
		}

		private void UpdateBorders()
		{
			var lines = GridViewLines;
			var currentBorderThickness = BorderThickness;
			var expectedBorderThickness = GetBorderThickness(lines);

			if (currentBorderThickness != expectedBorderThickness)
				BorderThickness = expectedBorderThickness;
		}
	}
}