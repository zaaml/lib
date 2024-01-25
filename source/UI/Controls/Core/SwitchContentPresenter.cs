// <copyright file="SwitchContentPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Core
{
	public class SwitchContentPresenter : FixedTemplateControl<ContentPresenter>
	{
		private static readonly DependencyPropertyKey ActualContentPropertyKey = DPM.RegisterReadOnly<object, SwitchContentPresenter>
			("ActualContent", default, d => d.OnActualContentPropertyChangedPrivate);

		public static readonly DependencyProperty EmptyVisibilityProperty = DPM.Register<Visibility, SwitchContentPresenter>
			("EmptyVisibility", Visibility.Visible, s => s.UpdateVisibility);

		public static readonly DependencyProperty ActualContentProperty = ActualContentPropertyKey.DependencyProperty;

		public object ActualContent
		{
			get => GetValue(ActualContentProperty);
			internal set => this.SetReadOnlyValue(ActualContentPropertyKey, value);
		}

		public Visibility EmptyVisibility
		{
			get => (Visibility) GetValue(EmptyVisibilityProperty);
			set => SetValue(EmptyVisibilityProperty, value);
		}

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			TemplateRoot.Content = ActualContent;
		}

		private void OnActualContentPropertyChangedPrivate(object oldValue, object newValue)
		{
			try
			{
				if (TemplateRoot == null)
					return;

				TemplateRoot.Content = newValue;
			}
			finally
			{
				UpdateVisibility();
			}
		}

		protected override void UndoTemplateOverride()
		{
			TemplateRoot.Content = null;

			UpdateVisibility();

			base.UndoTemplateOverride();
		}

		private void UpdateVisibility()
		{
			Visibility = ActualContent == null ? EmptyVisibility : Visibility.Visible;
		}
	}
}