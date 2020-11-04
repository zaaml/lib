// <copyright file="TwoLineTextBlockControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;
using Zaaml.UI.Panels;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Controls.Ribbon
{
	public enum TwoLineTextBlockControlMode
	{
		SingleLine,
		TwoLine
	}

	public class TwoLineTextBlockControl : FixedTemplateControl, IIconPresenter
	{
		public static readonly DependencyProperty TextProperty = DPM.Register<string, TwoLineTextBlockControl>
			("Text", p => p.OnTextChanged);

		public static readonly DependencyProperty GlyphProperty = DPM.Register<IconBase, TwoLineTextBlockControl>
			("Glyph", p => p.OnGlyphChanged);

		public static readonly DependencyProperty GlyphPresenterStyleProperty = DPM.Register<Style, TwoLineTextBlockControl>
			("GlyphPresenterStyle", p => p.OnGlyphPresenterStyleChanged);

		public static readonly DependencyProperty ModeProperty = DPM.Register<TwoLineTextBlockControlMode, TwoLineTextBlockControl>
			("Mode", TwoLineTextBlockControlMode.TwoLine, p => p.OnModeChanged);

		[UsedImplicitly] private readonly IDisposable _fontObserverDisposer;
		private readonly FontOptions _fontOptions;
		private readonly TwoLineTextBlockPanel _panel;
		private ITextMeasure _textMeasure;
		private bool _wrapDirty = true;

		public TwoLineTextBlockControl()
		{
			_fontOptions = new FontOptions();
			_fontObserverDisposer = _fontOptions.Observe(this);
			_fontOptions.PropertyChanged += FontOptionsOnPropertyChanged;
			_textMeasure = TextMeasureHelper.GetTextMeasure(new FontOptionsStruct(_fontOptions));

			_panel = new TwoLineTextBlockPanel(CreateTextBlock(), CreateTextBlock());

			ChildInternal = _panel;
		}

		public IconBase Glyph
		{
			get => (IconBase) GetValue(GlyphProperty);
			set => SetValue(GlyphProperty, value);
		}

		public Style GlyphPresenterStyle
		{
			get => (Style) GetValue(GlyphPresenterStyleProperty);
			set => SetValue(GlyphPresenterStyleProperty, value);
		}

		public TwoLineTextBlockControlMode Mode
		{
			get => (TwoLineTextBlockControlMode) GetValue(ModeProperty);
			set => SetValue(ModeProperty, value);
		}

		public string Text
		{
			get => (string) GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		private TextBlock CreateTextBlock()
		{
			return new TextBlock { HorizontalAlignment = HorizontalAlignment.Center };
		}

		private void FontOptionsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			_textMeasure = TextMeasureHelper.GetTextMeasure(new FontOptionsStruct(_fontOptions));

			InvalidateMeasure();
		}

		private static bool IsWrapSeparator(char ch)
		{
			return ch == ' ' || ch == '-';
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			WrapText();
			return base.MeasureOverride(availableSize);
		}

		private void OnGlyphChanged()
		{
			_panel.Glyph = Glyph;
			InvalidateMeasure();
		}

		private void OnGlyphPresenterStyleChanged()
		{
			if (GlyphPresenterStyle != null)
				_panel.GlyphPresenter.Style = GlyphPresenterStyle;
			else
				_panel.GlyphPresenter.ClearValue(StyleProperty);
		}

		private void OnModeChanged()
		{
			_panel.Mode = Mode;
			InvalidateMeasure();
		}

		private void OnTextChanged()
		{
			_wrapDirty = true;
			InvalidateMeasure();
		}

		private void WrapText()
		{
			if (_wrapDirty == false)
				return;

			if (Mode == TwoLineTextBlockControlMode.TwoLine)
			{
				var size = _textMeasure.MeasureString(Text).Width;
				var i = Text.Length;

				for (var j = i - 1; j > 0; j--)
				{
					if (IsWrapSeparator(Text[j]) == false)
						continue;

					var tsize = _textMeasure.MeasureString(Text.Left(j)).Width + _textMeasure.MeasureString(Text.Right(j + 1)).Width;

					if (tsize < size)
					{
						i = j + 1;
						size = tsize;
					}
					else
						break;
				}

				if (i == Text.Length)
				{
					_panel.FirstBlock.Text = Text;
					_panel.SecondBlock.Text = " ";
				}
				else
				{
					_panel.FirstBlock.Text = Text.Left(i).Trim();
					_panel.SecondBlock.Text = Text.Right(i).Trim();
				}
			}
			else
				_panel.FirstBlock.Text = Text;

			_wrapDirty = false;
		}

		IconBase IIconPresenter.Icon => _panel.GlyphPresenter.ActualIconInternal;

		private class TwoLineTextBlockPanel : Panel
		{
			private bool _arrangeDirty;
			private Size _lastArrangeSize = Size.Empty;
			private TwoLineTextBlockControlMode _mode = TwoLineTextBlockControlMode.TwoLine;

			public TwoLineTextBlockPanel(TextBlock firstBlock, TextBlock secondBlock)
			{
				FirstBlock = firstBlock;
				SecondBlock = secondBlock;

				Children.Add(FirstBlock);
				Children.Add(SecondBlock);
			}

			public TextBlock FirstBlock { get; }

			private Size FirstLineSize => FirstBlock.DesiredSize;

			public IconBase Glyph
			{
				get => GlyphPresenter.Icon;
				set
				{
					if (ReferenceEquals(Glyph, value))
						return;

					if (Glyph != null)
						Children.Remove(GlyphPresenter);

					GlyphPresenter.Icon = value;

					if (Glyph != null)
						Children.Add(GlyphPresenter);
				}
			}

			public IconPresenter GlyphPresenter { get; } = new IconPresenter();

			public TwoLineTextBlockControlMode Mode
			{
				get => _mode;
				set
				{
					if (_mode == value)
						return;

					_mode = value;

					if (_mode == TwoLineTextBlockControlMode.SingleLine)
						Children.Remove(SecondBlock);
					else
						Children.Add(SecondBlock);

					InvalidateMeasure();
				}
			}

			public TextBlock SecondBlock { get; }

			private Size SecondLineSize => Glyph != null ? SecondBlock.DesiredSize.AsOriented(Orientation.Horizontal).StackSize(GlyphPresenter.DesiredSize).Size : SecondBlock.DesiredSize;

			private Size SingleLineSize => Glyph != null ? FirstBlock.DesiredSize.AsOriented(Orientation.Horizontal).StackSize(GlyphPresenter.DesiredSize).Size : FirstBlock.DesiredSize;

			protected override Size ArrangeOverrideCore(Size finalSize)
			{
				if (_arrangeDirty == false && _lastArrangeSize.Equals(finalSize))
					return finalSize;

				_arrangeDirty = false;
				_lastArrangeSize = finalSize;

				if (Mode == TwoLineTextBlockControlMode.SingleLine)
				{
					var singleLineHostRect = new Rect(0, 0, finalSize.Width, SingleLineSize.Height);
					var singleLineRect = RectUtils.CalcAlignBox(singleLineHostRect, new Rect(singleLineHostRect.GetTopLeft(), SingleLineSize), HorizontalAlignment.Center, VerticalAlignment.Stretch);
					var textBlockRect = singleLineRect;

					if (Glyph != null)
					{
						var glyphRect = singleLineRect;

						textBlockRect.Width = FirstBlock.DesiredSize.Width;
						glyphRect.Width = GlyphPresenter.DesiredSize.Width;
						glyphRect.X += textBlockRect.Width;
						GlyphPresenter.Arrange(glyphRect);
					}

					FirstBlock.Arrange(textBlockRect);
				}
				else
				{
					var firstLineHostRect = new Rect(0, 0, finalSize.Width, FirstLineSize.Height);
					var secondRectHostRect = new Rect(0, firstLineHostRect.Height, finalSize.Width, SecondLineSize.Height);
					var firstLineRect = RectUtils.CalcAlignBox(firstLineHostRect, new Rect(firstLineHostRect.GetTopLeft(), FirstLineSize), HorizontalAlignment.Center, VerticalAlignment.Stretch);
					var secondLineRect = RectUtils.CalcAlignBox(secondRectHostRect, new Rect(secondRectHostRect.GetTopLeft(), SecondLineSize), HorizontalAlignment.Center, VerticalAlignment.Stretch);

					FirstBlock.Arrange(firstLineRect);

					var secondBlockRect = secondLineRect;

					if (Glyph != null)
					{
						var glyphRect = secondBlockRect;

						secondBlockRect.Width = string.IsNullOrWhiteSpace(SecondBlock.Text) ? 0 : SecondBlock.DesiredSize.Width;
						glyphRect.Width = GlyphPresenter.DesiredSize.Width;
						glyphRect.X += secondBlockRect.Width;
						GlyphPresenter.Arrange(glyphRect);
					}

					SecondBlock.Arrange(secondBlockRect);
				}

				return finalSize;
			}

			protected override Size MeasureOverrideCore(Size availableSize)
			{
				_arrangeDirty = true;

				FirstBlock.Measure(XamlConstants.InfiniteSize);

				if (Mode == TwoLineTextBlockControlMode.TwoLine)
					SecondBlock.Measure(XamlConstants.InfiniteSize);

				if (Glyph != null)
					GlyphPresenter.Measure(XamlConstants.InfiniteSize);

				var result = Mode == TwoLineTextBlockControlMode.SingleLine ? SingleLineSize : FirstLineSize.AsOriented(Orientation.Vertical).StackSize(SecondLineSize).Size;

				return result;
			}
		}
	}
}