// <copyright file="ArtboardRulerPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Artboard
{
	public sealed partial class ArtboardRulerPanel : GridLineRendererPanel<ArtboardRulerModel, TickMarkDefinitionCollection, TickMarkDefinition>
	{
		private readonly RotateTransform _labelRotateTransform = new RotateTransform();
		private readonly Stack<TextBlock> _textBlockPool = new Stack<TextBlock>();
		private readonly TransformGroup _transform;
		private ArtboardRuler _artboardRuler;
		private FontOptions _fontOptions;

		public ArtboardRulerPanel()
		{
			_transform = new TransformGroup
			{
				Children = new TransformCollection
				{
					_labelRotateTransform,
					OffsetTransform
				}
			};
		}

		internal ArtboardRuler ArtboardRuler
		{
			get => _artboardRuler;
			set
			{
				if (ReferenceEquals(_artboardRuler, value))
					return;

				_artboardRuler = value;
				_fontOptions = _fontOptions.DisposeExchange(value != null ? FontOptions.FromControl(value) : null);

				UpdateOrientation();
				Reset();
			}
		}

		private RulerFigureBuilder CurrentRulerFigureBuilder => (RulerFigureBuilder) CurrentFigureBuilder;

		private protected override FigureBuilder CreateFigureBuilder()
		{
			return new RulerFigureBuilder(this);
		}

		private TextBlock CreateTextBlock()
		{
			if (_artboardRuler == null)
				return null;

			var textBlock = new TextBlock
			{
				RenderTransform = _transform
			};

			_fontOptions.Attach(textBlock);

			return textBlock;
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			var measureOverrideCore = base.MeasureOverrideCore(availableSize);

			if (CurrentRulerFigureBuilder == null)
				return measureOverrideCore;

			var maxTickMarkSize = CurrentRulerFigureBuilder.MaxTickMarkSize;

			if (_artboardRuler.Orientation == Orientation.Horizontal)
				measureOverrideCore.Height = measureOverrideCore.Height.Clamp(maxTickMarkSize, double.MaxValue);
			else
				measureOverrideCore.Width = measureOverrideCore.Width.Clamp(maxTickMarkSize, double.MaxValue);

			return measureOverrideCore;
		}

		private TextBlock MountTextBlock()
		{
			return _textBlockPool.Count > 0 ? _textBlockPool.Pop() : CreateTextBlock();
		}

		protected override void OnOffsetChanged()
		{
			base.OnOffsetChanged();

			CurrentRulerFigureBuilder?.UpdateLabels();
		}

		internal void OnOrientationChanged()
		{
			ResetFigureBuilders();

			UpdateOrientation();
		}

		private void ReleaseTextBlock(TextBlock textBlock)
		{
			_textBlockPool.Push(textBlock);
		}

		private void Reset()
		{
			ResetFigureBuilders();
		}

		private void UpdateOrientation()
		{
			_labelRotateTransform.Angle = _artboardRuler?.Orientation == Orientation.Horizontal ? 0 : -90;
		}
	}
}