// <copyright file="SpyZoomElementSizeRenderer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Spy
{
	internal sealed class SpyZoomElementSizeRenderer : FixedTemplateControl<Border>
	{
		private Size _elementSize;

		public Size ElementSize
		{
			get => _elementSize;
			set
			{
				if (_elementSize == value)
				{
					return;
				}

				_elementSize = value;

				UpdateSize();
			}
		}

		private TextBlock TextBlock { get; } = new()
		{
			Foreground = Brushes.White
		};

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			TemplateRoot.Background = Brushes.Red;
			TemplateRoot.Padding = new Thickness(10, 4, 10, 4);
			TemplateRoot.Child = TextBlock;
			TemplateRoot.IsHitTestVisible = false;
		}

		protected override void UndoTemplateOverride()
		{
			TemplateRoot.Child = null;

			base.UndoTemplateOverride();
		}

		private void UpdateSize()
		{
			TextBlock.Text = $"{ElementSize.Width}x{ElementSize.Height}";
		}
	}
}