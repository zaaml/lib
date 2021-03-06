// <copyright file="PatternBrushControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Behaviors;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Primitives
{
	public sealed class PatternPanel : PanelBase
	{
		private BackgroundPatternBase _pattern;
		private UIElement _patternElement;

		public PatternPanel()
		{
			this.AddBehavior(new ClipToBoundsBehavior());
		}

		public BackgroundPatternBase Pattern
		{
			get => _pattern;
			set
			{
				if (ReferenceEquals(_pattern, value))
					return;

				if (_pattern != null)
					_pattern.PatternChanged -= PatternOnChanged;

				_pattern = value;

				if (_pattern != null)
					_pattern.PatternChanged += PatternOnChanged;

				InvalidatePattern();
			}
		}

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			if (_patternElement != null)
				return finalSize;

			Children.Clear();
			Background = null;

			_patternElement ??= _pattern?.CreatePatternElementInternal();

			if (_patternElement == null)
				return finalSize;

			Children.Add(_patternElement);

			_patternElement.Measure(XamlConstants.InfiniteSize);
			_patternElement.Arrange(new Rect(new Point(finalSize.Width, finalSize.Height), _patternElement.DesiredSize));

			Background = new VisualBrush(_patternElement)
			{
				Viewport = new Rect(_patternElement.DesiredSize),
				ViewportUnits = BrushMappingMode.Absolute,
				ViewboxUnits = BrushMappingMode.RelativeToBoundingBox,
				Viewbox = new Rect(new Size(1, 1)),
				Stretch = Stretch.None,
				TileMode = TileMode.Tile
			};

			return finalSize;
		}

		internal void InvalidatePattern()
		{
			_patternElement = null;

			InvalidateArrange();
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			return XamlConstants.ZeroSize;
		}

		private void PatternOnChanged(object sender, EventArgs eventArgs)
		{
			InvalidatePattern();
		}
	}

	[ContentProperty("Pattern")]
	public sealed class PatternBrushControl : FixedTemplateControl<PatternPanel>
	{
		public static readonly DependencyProperty PatternProperty = DPM.Register<BackgroundPatternBase, PatternBrushControl>
			("Pattern", p => p.OnPatternChanged);

		static PatternBrushControl()
		{
			ControlUtils.OverrideIsTabStop<PatternBrushControl>(false);
		}

		public BackgroundPatternBase Pattern
		{
			get => (BackgroundPatternBase) GetValue(PatternProperty);
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
			if (TemplateRoot == null)
				return;

			TemplateRoot.Pattern = Pattern;
		}
	}

	[ContentProperty("Template")]
	public sealed class DataTemplateBackgroundPattern : BackgroundPatternBase
	{
		public static readonly DependencyProperty TemplateProperty = DPM.Register<DataTemplate, DataTemplateBackgroundPattern>
			("Template", d => d.OnTemplateChanged);

		public DataTemplate Template
		{
			get => (DataTemplate) GetValue(TemplateProperty);
			set => SetValue(TemplateProperty, value);
		}

		protected override UIElement CreatePatternElementCore()
		{
			return (UIElement) Template?.LoadContent();
		}

		private void OnTemplateChanged()
		{
			OnPatternChanged();
		}
	}
}