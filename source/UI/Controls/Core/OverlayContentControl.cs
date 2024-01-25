// <copyright file="OverlayContentControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Core
{
	public sealed class OverlayContentControl : FixedTemplateContentControl<OverlayPanel, FrameworkElement>
	{
		public OverlayContentControl()
		{
			Focusable = false;
			IsTabStop = false;
		}

		private List<OverlayItemPresenter> Items { get; } = new();

		public void AddItem(OverlayItemPresenter element)
		{
			Items.Add(element);

			TemplateRoot?.Children.Add(element);
		}

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			TemplateRoot.Control = this;

			foreach (var frameworkElement in Items)
				TemplateRoot.Children.Add(frameworkElement);
		}

		public static OverlayContentControl GetOverlay(FrameworkElement frameworkElement)
		{
			return frameworkElement.GetVisualAncestorsAndSelf().OfType<OverlayContentControl>().FirstOrDefault();
		}

		public void RemoveItem(OverlayItemPresenter element)
		{
			TemplateRoot?.Children.Remove(element);

			Items.Remove(element);
		}

		protected override void UndoTemplateOverride()
		{
			foreach (var frameworkElement in Items)
				TemplateRoot.Children.Remove(frameworkElement);

			TemplateRoot.Control = null;

			base.UndoTemplateOverride();
		}
	}
}