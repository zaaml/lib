// <copyright file="ArtboardDesignContentControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Artboard
{
	[ContentProperty(nameof(Child))]
	public sealed class ArtboardDesignContentControl : ArtboardControlBase<ArtboardDesignContentPanel>, IFixedTemplateContentControl<ArtboardDesignContentPanel>
	{
		public static readonly DependencyProperty ChildProperty = DPM.Register<UIElement, ArtboardDesignContentControl>
			("Child", default, d => d.OnChildPropertyChangedPrivate);

		public ArtboardDesignContentControl()
		{
			Controller = new FixedTemplateContentControlController<ArtboardDesignContentPanel, UIElement>(this);
		}

		public UIElement Child
		{
			get => (UIElement) GetValue(ChildProperty);
			set => SetValue(ChildProperty, value);
		}

		private FixedTemplateContentControlController<ArtboardDesignContentPanel, UIElement> Controller { get; }

		protected override IEnumerator LogicalChildren => Controller.LogicalChildren;

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			Controller.AttachChild();
		}

		private void OnChildPropertyChangedPrivate()
		{
			Controller.Child = Child;
		}

		protected override void UndoTemplateOverride()
		{
			Controller.DetachChild();

			base.UndoTemplateOverride();
		}

		ArtboardDesignContentPanel IFixedTemplateContentControl<ArtboardDesignContentPanel>.Panel => TemplateRoot;

		bool IFixedTemplateContentControl<ArtboardDesignContentPanel>.IsLogicalParent => true;
	}
}