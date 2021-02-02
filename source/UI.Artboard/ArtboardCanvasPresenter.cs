// <copyright file="ArtboardCanvasPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Artboard
{
	public sealed class ArtboardCanvasPresenter : FixedTemplateContentControlBase<Panel, ArtboardCanvas>
	{
		public ArtboardCanvas Canvas
		{
			get => ChildCore;
			internal set => ChildCore = value;
		}
	}
}