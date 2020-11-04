// <copyright file="WindowElement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Windows
{
	public interface IWindow
	{
		#region Fields

		event EventHandler StateChanged;
		event EventHandler IsActiveChanged;

		#endregion

		#region Properties

		string Title { get; }

		WindowState WindowState { get; }

    bool IsActive { get; }

		#endregion
	}

  public abstract class WindowElement : TemplateContractControl, IWindowElement
	{
		#region Static Fields and Constants

		private static readonly DependencyPropertyKey WindowPropertyKey = DPM.RegisterReadOnly<IWindow, WindowElement>
			("Window");

		public static readonly DependencyProperty WindowProperty = WindowPropertyKey.DependencyProperty;

		#endregion

		#region Properties

		public IWindow Window
		{
			get => (IWindow) GetValue(WindowProperty);
			internal set
			{
				var window = Window;

				if (ReferenceEquals(window, value))
					return;

				if (window != null)
					OnWindowDetaching();

				this.SetReadOnlyValue(WindowPropertyKey, value);

				if (value != null)
					OnWindowAttached();
			}
		}

		#endregion

		#region  Methods

		internal virtual IEnumerable<IWindowElement> EnumerateWindowElements()
		{
			yield break;
		}

		protected virtual void OnWindowAttached()
		{
		}

		protected virtual void OnWindowDetaching()
		{
		}

		#endregion

		#region Interface Implementations

		#region IWindowElement

		IEnumerable<IWindowElement> IWindowElement.EnumerateWindowElements()
		{
			return EnumerateWindowElements();
		}

		IWindow IWindowElement.Window
		{
			get => Window;
			set => Window = value;
		}

		#endregion

		#endregion
	}
}