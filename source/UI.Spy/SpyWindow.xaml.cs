// <copyright file="SpyWindow.xaml.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Spy
{
	public partial class SpyWindow
	{
		public static readonly DependencyProperty TrackerProperty = DPM.Register<SpyElementTracker, SpyWindow>
			("Tracker", d => d.OnTrackerPropertyChangedPrivate);

		private bool _ignoreClosing;

		private SpyWindow()
		{
			InitializeComponent();

			RenderingObserver = new CompositionRenderingObserver(UnloadWindow);

			ShowInTaskbar = false;
		}

		protected override void OnStateChanged(EventArgs e)
		{
			base.OnStateChanged(e);

			if (WindowState == WindowState.Minimized)
				Hide();
		}

		private static SpyWindow Instance => LazyInstance.Value;

		private static Lazy<SpyWindow> LazyInstance { get; } = new(() => new SpyWindow());

		private CompositionRenderingObserver RenderingObserver { get; }

		public SpyElementTracker Tracker
		{
			get => (SpyElementTracker) GetValue(TrackerProperty);
			set => SetValue(TrackerProperty, value);
		}

		internal static void Enable(SpyMouseElementTracker spyMouseElementTracker)
		{
			Instance.Tracker = spyMouseElementTracker;
		}

		public static void Enable()
		{
			Instance.Tracker = new SpyMouseElementTracker {Trigger = new SpyKeyboardTrigger {Key = Key.RightCtrl}};
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			if (_ignoreClosing)
				return;

			e.Cancel = true;

			Hide();
		}

		private void OnElementChanged(object sender, EventArgs e)
		{
			ShowSpyWindow();
		}

		private void OnTrackerPropertyChangedPrivate(SpyElementTracker oldValue, SpyElementTracker newValue)
		{
			if (oldValue != null)
				oldValue.ElementChanged -= OnElementChanged;

			if (newValue != null)
				newValue.ElementChanged += OnElementChanged;

			SpyControl.Tracker = newValue;
		}

		private void ShowSpyWindow()
		{
			if (IsVisible)
				return;

			Show();

			WindowState = WindowState.Normal;
		}

		private void UnloadWindow()
		{
			var windows = Application.Current.Windows;

			if (windows.Count == 1 && ReferenceEquals(windows[0], this))
			{
				_ignoreClosing = true;

				Close();
			}
		}
	}
}