// <copyright file="SourcePopupTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public abstract class SourcePopupTrigger : PopupTrigger
	{
		public static readonly DependencyProperty SourceProperty = DPM.Register<FrameworkElement, SourcePopupTrigger>
			("Source", d => d.OnTargetPropertyChangedPrivate);

		public FrameworkElement Source
		{
			get => (FrameworkElement)GetValue(SourceProperty);
			set => SetValue(SourceProperty, value);
		}

		protected abstract void AttachTarget(FrameworkElement source);

		protected abstract void DetachTarget(FrameworkElement source);

		private void OnTargetPropertyChangedPrivate(FrameworkElement oldValue, FrameworkElement newValue)
		{
			if (oldValue != null)
				DetachTarget(oldValue);

			if (newValue != null)
				AttachTarget(newValue);
		}
	}
}