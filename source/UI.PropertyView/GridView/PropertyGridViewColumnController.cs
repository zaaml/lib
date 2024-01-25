// <copyright file="PropertyGridViewColumnController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.PropertyView
{
	public sealed class PropertyGridViewColumnController
		: GridViewColumnController
	{
		public PropertyGridViewColumnController(PropertyGridViewController viewController) : base(viewController)
		{
		}
	}
}