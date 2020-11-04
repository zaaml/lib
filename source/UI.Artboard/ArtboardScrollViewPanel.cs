// <copyright file="ArtboardScrollViewPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Controls.ScrollView;

namespace Zaaml.UI.Controls.Artboard
{
	public class ArtboardScrollViewPanel : ScrollViewPanelBase
	{
		private ArtboardScrollViewControl ArtboardScrollView => (ArtboardScrollViewControl) ScrollView;

		protected override HorizontalAlignment HorizontalContentAlignment => HorizontalAlignment.Center;

		protected override ScrollViewControlBase ScrollView => ViewPresenter?.ScrollView;

		protected override VerticalAlignment VerticalContentAlignment => VerticalAlignment.Center;

		private ArtboardScrollViewPresenter ViewPresenter => this.GetVisualParent() as ArtboardScrollViewPresenter;

		internal void UpdateZoom()
		{
			var zoom = ArtboardScrollView?.Zoom ?? 1.0;

			UpdateScale(zoom, zoom);
		}
	}
}