// <copyright file="TextWrapPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Panels
{
	public class TextWrapPanel : Panel
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty TextProperty = DPM.Register<string, TextWrapPanel>
			("Text", string.Empty, t => t.OnTextChanged);

		#endregion

		#region Fields

		private TextMeasureHelper _fontMeasureHelper;
		private FontOptionsStruct _fontOptions;
		private bool _measureDirty;
		private Size _lastMeasureSize;
		private Size _lastMeasureResult;
		private TextWrapping _textWrapping;

		#endregion

		#region Properties

		internal FontOptionsStruct FontOptions
		{
			get => _fontOptions;
			set
			{
				_fontOptions = value;
				_fontMeasureHelper = TextMeasureHelper.GetTextMeasure(value);
				_measureDirty = true;

				InvalidateMeasure();
			}
		}

		public string Text
		{
			get => (string) GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		#endregion

		#region  Methods

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			var verticalOffset = 0.0;

			foreach (var textBlock in Children.Cast<TextBlock>())
			{
#if SILVERLIGHT
        var rect = new Rect(new Point(0, verticalOffset), textBlock.GetActualSize());
#else
				var rect = new Rect(new Point(0, verticalOffset), new Size(finalSize.Width, textBlock.DesiredSize.Height));
#endif
				textBlock.Arrange(rect);

				verticalOffset += textBlock.ActualHeight;
			}

			return finalSize;
		}

		private ITextWrapper TextWrapper => Panels.TextWrapper.Instance;

		internal TextWrapping TextWrapping
		{
			get => _textWrapping;
			set
			{
				if (_textWrapping == value)
					return;

				_textWrapping = value;

				_measureDirty = true;
				InvalidateMeasure();
			}
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			if (_measureDirty == false && _lastMeasureSize.IsCloseTo(availableSize))
				return _lastMeasureResult;

			_measureDirty = false;
			_lastMeasureSize = availableSize;

			var textBlockList = Children.OfType<TextBlock>().ToList();

			Children.Clear();

			var lines = TextWrapper.WrapText(Text, TextWrapping == TextWrapping.NoWrap  ? 1 : int.MaxValue, false, availableSize, _fontMeasureHelper);
			var height = 0.0;
			var width = 0.0;

			foreach (var textBlock in lines.Select(l => AddTextBlock(l, textBlockList)))
			{
				textBlock.Measure(XamlConstants.InfiniteSize);

				height += textBlock.ActualHeight;
				width = Math.Max(width, textBlock.ActualWidth);

				Children.Add(textBlock);
			}

			_lastMeasureResult = new Size(width, height);

			return _lastMeasureResult;
		}

		private static TextBlock AddTextBlock(string line, List<TextBlock> textBlockList)
		{
			TextBlock textBlock;

			if (textBlockList.Count > 0)
			{
				textBlock = textBlockList[textBlockList.Count - 1];
				textBlockList.RemoveAt(textBlockList.Count - 1);
			}
			else
				textBlock = new TextBlock();

			textBlock.Text = line;

			return textBlock;
		}

		private void OnTextChanged()
		{
			_measureDirty = true;

			InvalidateMeasure();
		}

		#endregion
	}

	internal class TextWrapper : ITextWrapper
	{
		#region Static Fields and Constants

		private const string Ellipsis = "...";

		#endregion

		#region Interface Implementations

		private TextWrapper()
		{
		}

		public static ITextWrapper Instance = new TextWrapper();

		#region ITextWrapper

		public IEnumerable<string> WrapText(string text, int maxLineCount, bool trimVertical, Size availableSize, ITextMeasure measure)
		{
			if (string.IsNullOrEmpty(text))
				return Enumerable.Empty<string>();

			maxLineCount = Math.Max(1, maxLineCount);

			var verticalOffset = 0.0;

			var lineHeight = 0.0;
			var lineWidth = 0.0;
			var result = new List<string>();

			var lineStart = 0;
			var lineEnd = lineStart;
			var whiteSpaceLineStart = lineStart;

			int? wordEnd = null;
			int currentIndex;
			var trim = -1;

			for (currentIndex = lineStart; currentIndex < text.Length; currentIndex++)
			{
				var ch = text[currentIndex];
				var isSeparator = IsWrapSeparator(ch);
				var charSize = measure.MeasureChar(ch);

				lineHeight = Math.Max(lineHeight, charSize.Height);

				var isOverflow = availableSize.Height.IsLessThan(verticalOffset + lineHeight);
				var canFinish = result.Count >= maxLineCount;

				if (canFinish == false)
				{
					if (isOverflow)
					{
						canFinish = trim == result.Count || trimVertical;

						if (trimVertical == false)
							trim = result.Count + 1;
					}
				}

				if (result.Count > 0 && canFinish)
				{
					for (; currentIndex < text.Length; currentIndex++)
						if (availableSize.Width.IsLessThanOrClose(lineWidth += measure.MeasureChar(text[currentIndex]).Width))
							break;

					var str = string.Concat(result.Last(), text.Substring(whiteSpaceLineStart, currentIndex - whiteSpaceLineStart));

					result[result.Count - 1] = TrimToEllipsis(str.Trim(), availableSize.Width, measure);

					return result;
				}

				if (availableSize.Width.IsGreaterThanOrClose(lineWidth + charSize.Width))
				{
					if (isSeparator == false)
						lineEnd = currentIndex;
					else
						wordEnd = currentIndex - 1;

					lineWidth += charSize.Width;
				}
				else
				{
					if (isSeparator)
						wordEnd = currentIndex - 1;

					result.Add(text.Substring(lineStart, (wordEnd ?? lineEnd) - lineStart + 1));
					whiteSpaceLineStart = wordEnd + 1 ?? currentIndex;
					lineStart = lineEnd = SkipWhiteSpaces(text, whiteSpaceLineStart);
					wordEnd = null;
					currentIndex = lineStart - 1;
					verticalOffset += lineHeight;
					lineWidth = 0;
					lineHeight = 0;
				}
			}

			if (lineEnd < text.Length)
				result.Add(text.Substring(lineStart, text.Length - lineStart));

			return result;
		}

		#endregion

		#endregion

		#region  Methods

		private static bool IsWrapSeparator(char ch)
		{
			return ch == ' ' || ch == '-';
		}

		private static int SkipWhiteSpaces(string text, int index)
		{
			while (char.IsWhiteSpace(text[index]))
				index++;

			return index;
		}

		private static string TrimToEllipsis(string str, double width, ITextMeasure measure)
		{
			var ellipsisWidth = measure.MeasureString(Ellipsis).Width;
			var strWidth = measure.MeasureString(str).Width;
			var fullWidth = strWidth + ellipsisWidth;
			var i = str.Length - 1;

			for (; i >= 0 && fullWidth.IsGreaterThan(width); i--)
				fullWidth -= measure.MeasureChar(str[i]).Width;

			return string.Concat(str.Substring(0, i + 1).TrimEnd(), Ellipsis);
		}

		#endregion
	}

	internal class TextMeasureHelper : ITextMeasure
	{
		#region Ctors

		private TextMeasureHelper(FontOptionsStruct fontOptions)
		{
			_textBlock = new TextBlock
			{
				FontFamily = fontOptions.FontFamily,
				FontSize = fontOptions.FontSize,
				FontStyle = fontOptions.FontStyle,
				FontWeight = fontOptions.FontWeight
			};
		}

		#endregion

		#region Fields

		private readonly TextBlock _textBlock;

		#endregion

		#region Interface Implementations

		private static readonly Dictionary<FontOptionsStruct, WeakReference> TextMeasureDictionary = new Dictionary<FontOptionsStruct, WeakReference>();

		public static TextMeasureHelper GetTextMeasure(FontOptionsStruct fontOptionsStruct)
		{
			WeakReference weakReference;
			TextMeasureHelper textMeasureHelper = null;

			if (TextMeasureDictionary.TryGetValue(fontOptionsStruct, out weakReference))
				textMeasureHelper = (TextMeasureHelper) weakReference.Target;

			if (textMeasureHelper != null)
				return textMeasureHelper;

			textMeasureHelper = new TextMeasureHelper(fontOptionsStruct);

			TextMeasureDictionary[fontOptionsStruct] = new WeakReference(textMeasureHelper);

			return textMeasureHelper;
		}

		#region ITextMeasure

		private readonly Dictionary<char, Size> _measureDictionary = new Dictionary<char, Size>();

		public Size MeasureChar(char ch)
		{
			Size size;

			if (_measureDictionary.TryGetValue(ch, out size))
				return size;

			size = MeasureString(ch.ToString(CultureInfo.InvariantCulture));

			_measureDictionary[ch] = size;

			return size;
		}

		public Size MeasureString(string text)
		{
			_textBlock.Text = text;
			_textBlock.Measure(XamlConstants.InfiniteSize);

#if SILVERLIGHT
      return _textBlock.GetActualSize();
#else
			return _textBlock.DesiredSize;
#endif
		}

		#endregion

		#endregion
	}

	internal interface ITextMeasure
	{
		#region  Methods

		Size MeasureChar(char ch);

		Size MeasureString(string text);

		#endregion
	}

	internal interface ITextWrapper
	{
		#region  Methods

		IEnumerable<string> WrapText(string text, int maxLineCount, bool trimVertical, Size availableSize, ITextMeasure measure);

		#endregion
	}
}