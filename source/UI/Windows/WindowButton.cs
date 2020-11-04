// <copyright file="WindowButton.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Primitives;

namespace Zaaml.UI.Windows
{
	public abstract class WindowButton : Button, IWindowElement
	{
		#region Static Fields and Constants

		private static readonly DependencyPropertyKey WindowPropertyKey = DPM.RegisterReadOnly<WindowBase, WindowButton>
			("Window", b => b.OnWindowChanged);

		public static readonly DependencyProperty WindowProperty = WindowPropertyKey.DependencyProperty;

		#endregion

		#region Properties

		internal IWindowElement AsWindowElement => this;

		public WindowBase Window
		{
			get => (WindowBase) GetValue(WindowProperty);
			internal set => this.SetReadOnlyValue(WindowPropertyKey, value);
		}

		#endregion

		#region  Methods

		protected virtual void OnWindowChanged(WindowBase oldWindow, WindowBase newWindow)
		{
		}

		#endregion

		#region Interface Implementations

		#region IWindowElement

		IEnumerable<IWindowElement> IWindowElement.EnumerateWindowElements()
		{
			yield break;
		}

		IWindow IWindowElement.Window
		{
			get => Window;
			set => Window = (WindowBase) value;
		}

		#endregion

		#endregion
	}
}