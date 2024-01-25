// <copyright file="GridViewElement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.Core.GridView
{
	[TemplateContractType(typeof(GridViewElementTemplateContract))]
	public abstract class GridViewElement : TemplateContractControl
	{
	}

	public class GridViewElementTemplateContract : TemplateContract
	{
	}
}