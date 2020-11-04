// <copyright file="ISelectable.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Core
{
	public interface ISelectable
	{
		event EventHandler IsSelectedChanged;

		bool IsSelected { get; set; }
	}

	internal interface ISelectableEx : ISelectable
	{
		bool CanSelect { get; }
	}
}