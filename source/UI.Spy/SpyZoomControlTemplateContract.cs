// <copyright file="SpyZoomControlTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Spy
{
	public class SpyZoomControlTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = false)]
		public SpyArtboardControl ArtboardControl { get; [UsedImplicitly] private set; }
	}
}