// <copyright file="TextBlock.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using NativeTextBlock = System.Windows.Controls.TextBlock;
using NativeTextTrimming = System.Windows.TextTrimming;

namespace Zaaml.UI.Controls.Core.Compatibility
{
	public sealed partial class TextBlock : NativeTextBlock
	{
		#region Static Fields and Constants

		public new static readonly DependencyProperty TextTrimmingProperty = DPM.Register<TextTrimming, TextBlock>
			("TextTrimming", TextTrimming.None, t => t.OnTextTrimmingChanged);

		#endregion

		#region Properties

		public new TextTrimming TextTrimming
		{
			get => (TextTrimming) GetValue(TextTrimmingProperty);
			set => SetValue(TextTrimmingProperty, value);
		}

		#endregion

		#region  Methods

		private void OnTextTrimmingChanged()
		{
			switch (TextTrimming)
			{
				case TextTrimming.None:
					SetValue(NativeTextBlock.TextTrimmingProperty, NativeTextTrimming.None);
					break;
				case TextTrimming.CharacterEllipsis:
					SetValue(NativeTextBlock.TextTrimmingProperty, NativeTextTrimming.CharacterEllipsis);
					break;
				case TextTrimming.WordEllipsis:
					SetValue(NativeTextBlock.TextTrimmingProperty, NativeTextTrimming.WordEllipsis);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		#endregion
	}
}