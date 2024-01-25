// <copyright file="ArtboardItemTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.Artboard
{
	public class ArtboardItemTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public ArtboardCanvasPresenter CanvasPresenter { get; [UsedImplicitly] private set; }
	}
}