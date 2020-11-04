// <copyright file="IButtonController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;

namespace Zaaml.UI.Controls.Primitives
{
	internal interface IButtonController : ICommandController
	{
		#region  Methods

		void OnKeyDown(KeyEventArgs e);

		void OnKeyUp(KeyEventArgs e);

		void OnLostKeyboardFocus(RoutedEventArgs e);

		void OnLostMouseCapture(MouseEventArgs e);

		void OnMouseEnter(MouseEventArgs e);

		void OnMouseLeave(MouseEventArgs e);

		void OnMouseLeftButtonDown(MouseButtonEventArgs e);

		void OnMouseLeftButtonUp(MouseButtonEventArgs e);

		void OnMouseMove(MouseEventArgs e);

		void RaiseOnClick();

		#endregion
	}
}