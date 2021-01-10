// <copyright file="SwitchContentControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Core
{
	[ContentProperty(nameof(Content))]
	public class SwitchContentControl : Control
	{
		public static readonly DependencyProperty ContentProperty = DPM.Register<object, SwitchContentControl>
			("Content", d => d.LogicalChildMentor.OnLogicalChildPropertyChanged);

		public static readonly DependencyProperty PresenterProperty = DPM.Register<SwitchContentPresenter, SwitchContentControl>
			("Presenter", d => d.OnPresenterPropertyChangedPrivate);

		public object Content
		{
			get => GetValue(ContentProperty);
			set => SetValue(ContentProperty, value);
		}

		public SwitchContentPresenter Presenter
		{
			get => (SwitchContentPresenter) GetValue(PresenterProperty);
			set => SetValue(PresenterProperty, value);
		}

		private void OnPresenterPropertyChangedPrivate(SwitchContentPresenter oldValue, SwitchContentPresenter newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
				oldValue.ActualContent = null;

			if (newValue != null)
				newValue.ActualContent = Content;
		}
	}
}