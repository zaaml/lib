// <copyright file="ICommandControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;

namespace Zaaml.UI.Controls.Interfaces
{
	internal interface ICommandControl
	{
		ICommand Command { get; }

		object CommandParameter { get; }

		DependencyObject CommandTarget { get; }
	}
}