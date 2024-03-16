// <copyright file="IPropertyViewFilter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.PropertyView
{
	public interface IPropertyViewFilter
	{
		event EventHandler Changed;

		bool IsEnabled { get; }

		bool Pass(PropertyItem item, IServiceProvider serviceProvider);
	}
}