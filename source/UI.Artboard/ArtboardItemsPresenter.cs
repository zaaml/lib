// <copyright file="ArtboardItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Artboard
{
	[TemplateContractType(typeof(ArtboardItemsPresenterTemplateContract))]
	public class ArtboardItemsPresenter : ItemsPresenterBase<ArtboardControl, ArtboardItem, ArtboardItemCollection, ArtboardItemsPanel>, IArtboardComponentControl
	{
		ArtboardControl IArtboardComponentControl.Artboard { get; set; }

		double IArtboardComponentControl.ScrollOffsetX { get; set; }

		double IArtboardComponentControl.ScrollOffsetY { get; set; }

		double IArtboardComponentControl.Zoom { get; set; }
	}

	public class ArtboardItemsPresenterTemplateContract : ItemsPresenterBaseTemplateContract<ArtboardItemsPanel, ArtboardItem>
	{
	}
}