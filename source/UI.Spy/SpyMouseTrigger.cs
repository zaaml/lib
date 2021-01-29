// <copyright file="SpyMouseTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Spy
{
	public class SpyMouseTrigger : SpyTrigger
	{
		public static readonly DependencyProperty ButtonProperty = DPM.Register<MouseButton?, SpyMouseTrigger>
			("Button", null, d => d.OnButtonPropertyChangedPrivate);

		public SpyMouseTrigger()
		{
			RenderingObserver = new CompositionRenderingObserver(UpdateState);
		}

		public MouseButton? Button
		{
			get => (MouseButton) GetValue(ButtonProperty);
			set => SetValue(ButtonProperty, value);
		}

		private CompositionRenderingObserver RenderingObserver { [UsedImplicitly] get; }

		private void OnButtonPropertyChangedPrivate()
		{
			UpdateState();
		}

		private void UpdateState()
		{
			IsOpen = Button switch
			{
				MouseButton.Left => Mouse.LeftButton == MouseButtonState.Pressed,
				MouseButton.Middle => Mouse.MiddleButton == MouseButtonState.Pressed,
				MouseButton.Right => Mouse.RightButton == MouseButtonState.Pressed,
				MouseButton.XButton1 => Mouse.XButton1 == MouseButtonState.Pressed,
				MouseButton.XButton2 => Mouse.XButton2 == MouseButtonState.Pressed,
				null => true,
				_ => throw new ArgumentOutOfRangeException()
			};
		}
	}
}