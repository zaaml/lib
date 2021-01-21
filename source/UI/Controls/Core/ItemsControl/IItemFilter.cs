// <copyright file="IItemFilter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Core
{
	public interface IItemFilter
	{
		event EventHandler Changed;

		bool IsEnabled { get; }

		bool Pass(object item, IServiceProvider serviceProvider);
	}
}