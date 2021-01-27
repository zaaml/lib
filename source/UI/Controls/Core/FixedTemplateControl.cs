// <copyright file="FixedTemplateControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.TemplateCore;
using NativeContentPresenter = System.Windows.Controls.ContentPresenter;

namespace Zaaml.UI.Controls.Core
{
	public class FixedTemplateControl : FixedTemplateControlBase
	{
		private UIElement _childInternal;

		protected NativeContentPresenter ContentPresenter;

		public FixedTemplateControl() : this(TemplateKind.ContentPresenter)
		{
		}

		private protected FixedTemplateControl(TemplateKind templateKind)
		{
			TemplateInternal = GetTemplate(templateKind);
		}

		protected UIElement ChildInternal
		{
			get => _childInternal;
			set
			{
				if (ReferenceEquals(_childInternal, value))
					return;

				_childInternal = value;

				if (ContentPresenter != null)
					ContentPresenter.Content = value;
			}
		}

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			var templateRoot = this.GetImplementationRoot();

			if (templateRoot is Border border)
			{
				border.BindStyleProperties(this, BorderStyleProperties.All);
				ContentPresenter = new NativeContentPresenter();
				border.Child = ContentPresenter;
			}
			else if (templateRoot is NativeContentPresenter contentPresenter)
			{
				ContentPresenter = contentPresenter;
				ContentPresenter.BindProperties(MarginProperty, this, PaddingProperty);
			}

			if (ContentPresenter != null)
				ContentPresenter.Content = _childInternal;

			BindPropertiesCore();
		}

		protected virtual void BindPropertiesCore()
		{
		}

		private ControlTemplate GetTemplate(TemplateKind templateKind)
		{
			switch (templateKind)
			{
				case TemplateKind.None:
					return null;

				case TemplateKind.Border:
					return GenericControlTemplate.BorderTemplateInstance;

				case TemplateKind.ContentPresenter:
					return GenericControlTemplate.ContentPresenterTemplateInstance;

				default:
					throw new ArgumentOutOfRangeException(nameof(templateKind), templateKind, null);
			}
		}

		protected virtual void UnbindPropertiesCore()
		{
		}

		protected override void UndoTemplateOverride()
		{
			base.UndoTemplateOverride();

			UnbindPropertiesCore();
		}

		protected enum TemplateKind
		{
			None,
			Border,
			ContentPresenter
		}
	}

	public class FixedTemplateControl<T> : FixedTemplateControlBase where T : FrameworkElement
	{
		public FixedTemplateControl()
		{
			TemplateInternal = ControlTemplateBuilder<T>.Template;
		}

		protected T TemplateRoot { get; private set; }

		protected override void ApplyTemplateOverride()
		{
			TemplateRoot = ControlTemplateBuilder<T>.GetImplementationRoot(this);
		}

		protected override void UndoTemplateOverride()
		{
			TemplateRoot = null;
		}
	}
}