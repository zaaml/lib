// <copyright file="PropertyViewControlTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.PropertyView
{
	public class PropertyViewControlTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = false)]
		public PropertyTreeViewControl TreeView { get; [UsedImplicitly] private set; }
	}
}