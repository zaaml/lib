// <copyright file="PatternBrushControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Zaaml.Core.Monads;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Behaviors;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Primitives
{
	public interface IPattern
	{
		event EventHandler TemplateChanged;

		UIElement CreateElement();
	}

	internal class TileBlockPanel : PanelBase
	{
		private readonly Size _actualSize;
		private readonly bool _measureToActualSize;
		private readonly Size _size;

		public TileBlockPanel(Size size, IPattern pattern, bool measureToActualSize)
		{
			_size = size;
			_measureToActualSize = measureToActualSize;

			var patternElement = pattern.CreateElement();

			if (patternElement == null)
				return;

			patternElement.Measure(XamlConstants.InfiniteSize);

			var imagePattern = patternElement as Image;
			var bmp = imagePattern.Return(i => i.Source).As<WriteableBitmap>();

			var patternSize = bmp.Return(b => new Size(b.PixelWidth, b.PixelHeight), patternElement.DesiredSize);

			var w = patternSize.Width;
			var h = patternSize.Height;

			if (DoubleUtils.IsZero(w) || DoubleUtils.IsZero(h))
				return;

			var xCount = 1 + ((int) size.Width) / (int) w;
			var yCount = 1 + ((int) size.Height) / (int) h;

			for (var x = 0; x < xCount; x++)
			{
				for (var y = 0; y < yCount; y++)
				{
					var element = pattern.CreateElement();

					element.Measure(patternSize);
					Children.Add(element);
					SetArrangeRect(element, new Rect(new Point(x * w, y * h), patternSize));
				}
			}

			_actualSize = new Size(xCount * w, yCount * h);
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			foreach (var child in Children.Cast<UIElement>())
				child.Measure(GetArrangeSize(child));

			return _measureToActualSize ? _actualSize : _size;
		}
	}

	public class PatternPanel : PanelBase
	{
		private IPattern _pattern;
		private ImageSource _patternImage;

		public PatternPanel()
		{
			this.AddBehavior(new ClipToBoundsBehavior());
		}

		public IPattern Pattern
		{
			get => _pattern;
			set
			{
				if (ReferenceEquals(_pattern, value))
					return;

				if (_pattern != null)
					_pattern.TemplateChanged -= PatternOnChanged;

				_pattern = value;

				if (_pattern != null)
					_pattern.TemplateChanged += PatternOnChanged;

				InvalidatePattern();
			}
		}

		internal void InvalidatePattern()
		{
			_patternImage = null;
			InvalidateMeasure();

			if (_pattern == null)
				return;

			var size = new Size(256, 256);
			var tilePanel = new TileBlockPanel(size, _pattern, true);

#if SILVERLIGHT
      tilePanel.RenderOnLoad
        (bmp =>
        {
          _patternImage = bmp;
          InvalidateMeasure();
        });
#else
			_patternImage = Extension.CreateBitmap(tilePanel, size);
			InvalidateMeasure();
#endif
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			Children.Clear();

			if (double.IsInfinity(availableSize.Width) || double.IsInfinity(availableSize.Height) || _patternImage == null)
				return XamlConstants.ZeroSize;

			var tilePanel = new TileBlockPanel(availableSize, new ImageTemplatePattern {Image = _patternImage}, false);

			SetArrangeRect(tilePanel, new Rect(new Point(), availableSize));
			Children.Add(tilePanel);

			return availableSize;
		}

		private void PatternOnChanged(object sender, EventArgs eventArgs)
		{
			InvalidatePattern();
		}
	}

	[ContentProperty("Pattern")]
	public class PatternBrushControl : FixedTemplateControl
	{
		public static readonly DependencyProperty PatternProperty = DPM.Register<IPattern, PatternBrushControl>
			("Pattern", p => p.OnPatternChanged);

		private readonly PatternPanel _patternPanel;

		static PatternBrushControl()
		{
			ControlUtils.OverrideIsTabStop<PatternBrushControl>(false);
		}

		public PatternBrushControl()
		{
			_patternPanel = new PatternPanel();
			ChildInternal = _patternPanel;
		}

		public IPattern Pattern
		{
			get => (IPattern) GetValue(PatternProperty);
			set => SetValue(PatternProperty, value);
		}

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			UpdatePattern();
		}

		private void OnPatternChanged()
		{
			UpdatePattern();
		}

		private void UpdatePattern()
		{
			_patternPanel.Pattern = Pattern;
		}
	}

	[ContentProperty("Template")]
	public sealed class DataTemplatePattern : InheritanceContextObject, IPattern
	{
		public static readonly DependencyProperty TemplateProperty = DPM.Register<DataTemplate, DataTemplatePattern>
			("Template", d => d.OnTemplateChanged);

		public DataTemplate Template
		{
			get => (DataTemplate) GetValue(TemplateProperty);
			set => SetValue(TemplateProperty, value);
		}

		private void OnTemplateChanged()
		{
			TemplateChanged?.Invoke(this, EventArgs.Empty);
		}

		public event EventHandler TemplateChanged;

		public UIElement CreateElement()
		{
			return (UIElement) Template?.LoadContent();
		}
	}
}