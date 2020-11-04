// <copyright file="ArtboardScrollViewPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.ScrollView;

namespace Zaaml.UI.Controls.Artboard
{
	public class ArtboardScrollViewPresenter : ScrollViewPresenterBase<ArtboardScrollViewPanel>
	{
		private ArtboardScrollViewControl ArtboardScrollView => ScrollView as ArtboardScrollViewControl;

		internal ArtboardScrollViewPanel ScrollViewPanel => TemplateRoot;

		protected override void OnScrollViewAttached()
		{
			base.OnScrollViewAttached();

			UpdateZoom();
		}

		protected override void OnTemplateAttached()
		{
			base.OnTemplateAttached();

			UpdateZoom();
		}

		private void UpdateZoom()
		{
			if (ScrollViewPanel == null || ArtboardScrollView == null)
				return;

			ScrollViewPanel.UpdateZoom();
		}
	}
}