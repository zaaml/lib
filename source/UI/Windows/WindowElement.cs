// <copyright file="WindowElement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Windows
{
	public interface IWindow
	{
		event EventHandler StateChanged;
		event EventHandler IsActiveChanged;

		bool IsActive { get; }

		string Title { get; }

		WindowState WindowState { get; }
	}

	public abstract class WindowElement : TemplateContractControl, IWindowElement
	{
		private static readonly DependencyPropertyKey WindowPropertyKey = DPM.RegisterReadOnly<IWindow, WindowElement>
			("Window");

		public static readonly DependencyProperty WindowProperty = WindowPropertyKey.DependencyProperty;

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

		IEnumerable<IWindowElement> IWindowElement.EnumerateWindowElements()
		{
			return EnumerateWindowElements();
		}

		IWindow IWindowElement.Window
		{
			get => Window;
			set => Window = value;
		}
	}
}