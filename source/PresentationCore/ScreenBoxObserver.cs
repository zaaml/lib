// <copyright file="ScreenBoxObserver.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore
{
	internal partial class ScreenBoxObserver : IDisposable
	{
		private readonly Action _notifyCallback;
		private Rect _screenBox;

		public ScreenBoxObserver(FrameworkElement frameworkElement, Action notifyCallback)
		{
			_screenBox = frameworkElement.GetScreenLogicalBox();
			_notifyCallback = notifyCallback;

			FrameworkElement = frameworkElement;
			FrameworkElement.LayoutUpdated += FrameworkElementOnLayoutUpdated;

			PlatformCtor();
			UpdateScreenBox();
		}

		public FrameworkElement FrameworkElement { get; private set; }

		public bool IsDisposed => FrameworkElement == null;

		public Rect ScreenBox
		{
			get => _screenBox;
			private set
			{
				if (_screenBox.IsCloseTo(value))
					return;

				_screenBox = value;
				_notifyCallback();
			}
		}

		private void FrameworkElementOnLayoutUpdated(object sender, EventArgs eventArgs)
		{
			UpdateScreenBox();
		}

		partial void PlatformCtor();

		private void UpdateScreenBox()
		{
			if (FrameworkElement == null)
				return;

			ScreenBox = FrameworkElement.GetScreenLogicalBox();
		}

		public void Dispose()
		{
			if (IsDisposed)
				return;

			FrameworkElement.LayoutUpdated -= FrameworkElementOnLayoutUpdated;
			FrameworkElement = null;
		}
	}
}