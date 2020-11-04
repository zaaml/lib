// <copyright file="ItemsPresenterBaseTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Core
{
	public abstract class ItemsPresenterBaseTemplateContract : TemplateContract
	{
		#region Properties

		protected abstract Panel ItemsHostBase { get; }

		internal Panel ItemsHostInternal => ItemsHostBase;

		#endregion
	}
}