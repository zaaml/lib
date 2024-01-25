// <copyright file="XYControllerTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.Primitives
{
	public class XYControllerTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = false)]
		public XYControllerPanel XYControllerPanel { get; [UsedImplicitly] private set; }
	}
}