// <copyright file="GridLineRendererPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Zaaml.Core.Extensions;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Behaviors;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Artboard
{
	[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
	public abstract partial class GridLineRendererPanel<TGridLineModel, TGridLineCollection, TGridLine> : PanelBase
		where TGridLineModel : GridLineModel<TGridLineCollection, TGridLine>
		where TGridLineCollection : GridLineCollection<TGridLine>
		where TGridLine : GridLine
	{
		private static readonly PropertyPath StrokePropertyPath = new PropertyPath(GridLine.StrokeProperty);

		private static readonly PropertyPath StrokeThicknessPropertyPath =
			new PropertyPath(GridLine.StrokeThicknessProperty);

		private readonly Stack<Path> _pathPool = new Stack<Path>();
		protected readonly TranslateTransform OffsetTransform = new TranslateTransform();
		private FigureBuilder _currentFigureBuilder;

		private ConditionalWeakTable<TGridLineModel, Dictionary<double, FigureBuilder>> _figureBuildersDictionary =
			new ConditionalWeakTable<TGridLineModel, Dictionary<double, FigureBuilder>>();

		private bool _horizontalLines;
		private TGridLineModel _model;
		private GridLineStepLength _offsetX;
		private GridLineStepLength _offsetY;
		private double _scale = 1.0;
		private bool _structureDirty;
		private double _syncGridStep;
		private bool _verticalLines;

		protected GridLineRendererPanel()
		{
			this.AddBehavior(new ClipToBoundsBehavior());
		}

		private protected FigureBuilder CurrentFigureBuilder
		{
			get => _currentFigureBuilder;
			set
			{
				if (ReferenceEquals(_currentFigureBuilder, value))
					return;

				_currentFigureBuilder?.Detach();

				_currentFigureBuilder = value;

				_currentFigureBuilder?.Attach();

				UpdateOffsetTransform();
			}
		}

		public bool HorizontalLines
		{
			get => _horizontalLines;
			set
			{
				if (_horizontalLines == value)
					return;

				_horizontalLines = value;

				InvalidateStructure();
			}
		}

		public TGridLineModel Model
		{
			get => _model;
			set
			{
				if (ReferenceEquals(_model, value))
					return;

				if (_model != null)
					Detach(_model);

				_model = value;

				if (_model != null)
					Attach(_model);

				InvalidateStructure();
			}
		}

		public GridLineStepLength OffsetX
		{
			get => _offsetX;
			set
			{
				_offsetX = value;

				OnOffsetChanged();
			}
		}

		public GridLineStepLength OffsetY
		{
			get => _offsetY;
			set
			{
				_offsetY = value;

				OnOffsetChanged();
			}
		}

		public double Scale
		{
			get => _scale;
			set
			{
				if (_scale.IsCloseTo(value))
					return;

				_scale = value;

				OnScaleChanged();
			}
		}

		internal double SyncGridStep
		{
			get => _syncGridStep;
			set
			{
				if (_syncGridStep.IsCloseTo(value))
					return;

				_syncGridStep = value;

				InvalidateStructure();
			}
		}

		public bool VerticalLines
		{
			get => _verticalLines;
			set
			{
				if (_verticalLines == value)
					return;

				_verticalLines = value;

				InvalidateStructure();
			}
		}

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			CurrentFigureBuilder?.EnsureSize(finalSize);

			return base.ArrangeOverrideCore(finalSize);
		}

		private void Attach(TGridLineModel model)
		{
			model.ModelChanged += OnModelChanged;
		}

		private static double CalcRenderOffset(GridLineStepLength offset, FigureBuilder figureBuilder,
			Orientation orientation)
		{
			var maxGridStep = figureBuilder.MaxGridStep;
			var actualGridStep = figureBuilder.GridStep;
			var pixelOffset = offset.Unit == GridLineStepUnit.Step ? offset.Value * actualGridStep : offset.Value;

			pixelOffset = -pixelOffset % maxGridStep;

			if (pixelOffset < 0)
				pixelOffset += maxGridStep;

			return pixelOffset.LayoutRound(orientation, RoundingMode.ToZero);
		}

		private protected virtual FigureBuilder CreateFigureBuilder()
		{
			return new FigureBuilder(this);
		}

		private void Detach(TGridLineModel model)
		{
			model.ModelChanged -= OnModelChanged;
		}

		private void EnsureStructure()
		{
			if (_structureDirty == false)
				return;

			CurrentFigureBuilder = null;

			var model = Model;

			if (model == null)
				return;

			var scale = Scale;

			if (_figureBuildersDictionary.TryGetValue(model, out var scaleDictionary) == false || scaleDictionary == null)
				_figureBuildersDictionary.Add(model, scaleDictionary = new Dictionary<double, FigureBuilder>());

			var figureBuilder = scaleDictionary.GetValueOrCreate(scale, s => CreateFigureBuilder());

			if (figureBuilder.ModelVersion != model.Version || ReferenceEquals(figureBuilder.Model, model) == false)
				scaleDictionary[scale] = figureBuilder = CreateFigureBuilder();

			CurrentFigureBuilder = figureBuilder;

			_structureDirty = false;
		}

		protected void InvalidateStructure()
		{
			_structureDirty = true;

			InvalidateMeasure();
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			EnsureStructure();

			return XamlConstants.ZeroSize;
		}

		private Path MountPath(TGridLine gridLine)
		{
			var path = _pathPool.Count > 0 ? _pathPool.Pop() : new Path();

			path.RenderTransform = OffsetTransform;
			path.Stretch = Stretch.None;

			path.SetBinding(Shape.StrokeProperty, new Binding {Path = StrokePropertyPath, Source = gridLine});
			path.SetBinding(Shape.StrokeThicknessProperty,
				new Binding {Path = StrokeThicknessPropertyPath, Source = gridLine});

			return path;
		}

		protected override void OnChildDesiredSizeChanged(UIElement child)
		{
		}

		private void OnModelChanged(object sender, EventArgs e)
		{
			InvalidateStructure();
		}

		protected virtual void OnOffsetChanged()
		{
			UpdateOffsetTransform();
		}

		private void OnScaleChanged()
		{
			InvalidateStructure();
		}

		protected virtual void OnStructureUpdated()
		{
		}

		private void ReleasePath(Path path)
		{
			path.RenderTransform = null;
			path.ClearValue(Shape.StrokeProperty);
			path.ClearValue(Shape.StrokeThicknessProperty);

			Children.Remove(path);

			_pathPool.Push(path);
		}

		private protected void ResetFigureBuilders()
		{
			_figureBuildersDictionary = new ConditionalWeakTable<TGridLineModel, Dictionary<double, FigureBuilder>>();

			InvalidateStructure();
		}

		private protected void UpdateOffsetTransform(TranslateTransform transform, FigureBuilder figureBuilder)
		{
			if (VerticalLines)
				transform.X = CalcRenderOffset(OffsetX, figureBuilder, Orientation.Horizontal);

			if (HorizontalLines)
				transform.Y = CalcRenderOffset(OffsetY, figureBuilder, Orientation.Vertical);
		}

		private void UpdateOffsetTransform()
		{
			if (CurrentFigureBuilder != null)
				UpdateOffsetTransform(OffsetTransform, CurrentFigureBuilder);
		}
	}
}