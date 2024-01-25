// <copyright file="PathIcon.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Zaaml.PresentationCore.PropertyCore;

#if NETCOREAPP
#else
using Zaaml.Core.Extensions;
#endif

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
	[Flags]
	public enum PathIconBrushMode
	{
		None = 0,
		Fill = 1,
		Stroke = 2
	}

	public sealed partial class PathIcon : IconBase
	{
		public static readonly DependencyProperty DataProperty = DPM.RegisterAttached<Geometry, PathIcon>
			("Data", OnIconPropertyChanged);

		public static readonly DependencyProperty StretchProperty = DPM.RegisterAttached<Stretch, PathIcon>
			("Stretch", Stretch.None, OnIconPropertyChanged);

		public static readonly DependencyProperty BrushProperty = DPM.RegisterAttached<Brush, PathIcon>
			("Brush", OnIconPropertyChanged);

		public static readonly DependencyProperty FillProperty = DPM.RegisterAttached<Brush, PathIcon>
			("Fill", OnIconPropertyChanged);

		public static readonly DependencyProperty StrokeProperty = DPM.RegisterAttached<Brush, PathIcon>
			("Stroke", OnIconPropertyChanged);

		public static readonly DependencyProperty StrokeThicknessProperty = DPM.RegisterAttached<double, PathIcon>
			("StrokeThickness", OnIconPropertyChanged);

		public static readonly DependencyProperty StrokeLineJoinProperty = DPM.RegisterAttached<PenLineJoin, PathIcon>
			("StrokeLineJoin", OnIconPropertyChanged);

		public static readonly DependencyProperty BrushModeProperty = DPM.RegisterAttached<PathIconBrushMode, PathIcon>
			("BrushMode", OnIconPropertyChanged);

		private static readonly List<DependencyProperty> Properties = new()
		{
			DataProperty,
			StretchProperty,
			BrushProperty,
			FillProperty,
			StrokeProperty,
			StrokeThicknessProperty,
			StrokeLineJoinProperty,
			BrushModeProperty,
		};

		private static readonly Dictionary<DependencyProperty, DependencyProperty> PropertyDictionary = new()
		{
			{ DataProperty, Path.DataProperty },
			{ StretchProperty, Shape.StretchProperty },
			{ FillProperty, Shape.FillProperty },
			{ StrokeProperty, Shape.StrokeProperty },
			{ StrokeThicknessProperty, Shape.StrokeThicknessProperty },
		};

		private Path _path;

		static PathIcon()
		{
			Factories[DataProperty] = () => new PathIcon();
		}

		private Brush ActualBrushAsFill => IsBrushFill ? GetActualValue<Brush>(BrushProperty) : null;

		private Brush ActualBrushAsStroke => IsBrushStroke ? GetActualValue<Brush>(BrushProperty) : null;

		private PathIconBrushMode ActualBrushMode => GetActualValue<PathIconBrushMode>(BrushModeProperty);

		private Geometry ActualData => GetActualValue<Geometry>(DataProperty);

		private Brush ActualFill => GetActualValue<Brush>(FillProperty);

		private Stretch ActualStretch => GetActualValue<Stretch>(StretchProperty);

		private Brush ActualStroke => GetActualValue<Brush>(StrokeProperty);

		private PenLineJoin ActualStrokeLineJoin => GetActualValue<PenLineJoin>(StrokeLineJoinProperty);

		private double ActualStrokeThickness => GetActualValue<double>(StrokeThicknessProperty);

		public Brush Brush
		{
			get => (Brush)GetValue(BrushProperty);
			set => SetValue(BrushProperty, value);
		}

		public PathIconBrushMode BrushMode
		{
			get => (PathIconBrushMode)GetValue(BrushModeProperty);
			set => SetValue(BrushModeProperty, value);
		}

		public Geometry Data
		{
			get => (Geometry)GetValue(DataProperty);
			set => SetValue(DataProperty, value);
		}

		public Brush Fill
		{
			get => (Brush)GetValue(FillProperty);
			set => SetValue(FillProperty, value);
		}

		protected internal override FrameworkElement IconElement => _path ??= CreatePath();

		private bool IsBrushFill => (ActualBrushMode & PathIconBrushMode.Fill) != 0;

		private bool IsBrushStroke => (ActualBrushMode & PathIconBrushMode.Stroke) != 0;

		protected override IEnumerable<DependencyProperty> PropertiesCore => Properties;

		public Stretch Stretch
		{
			get => (Stretch)GetValue(StretchProperty);
			set => SetValue(StretchProperty, value);
		}

		public Brush Stroke
		{
			get => (Brush)GetValue(StrokeProperty);
			set => SetValue(StrokeProperty, value);
		}

		public PenLineJoin StrokeLineJoin
		{
			get => (PenLineJoin)GetValue(StrokeLineJoinProperty);
			set => SetValue(StrokeLineJoinProperty, value);
		}

		public double StrokeThickness
		{
			get => (double)GetValue(StrokeThicknessProperty);
			set => SetValue(StrokeThicknessProperty, value);
		}

		protected override object CopyValue(object value)
		{
#if SILVERLIGHT
	    var geometry = value as Geometry;

	    if (geometry != null)
	    {
	      try
	      {
	        var str = geometry.ToString();
	        geometry = StringGeometryConverter.ConvertString(str);
	      }
	      catch (Exception e)
	      {
	        LogService.LogError(e);
	      }

	      return geometry;
	    }
#endif

			return base.CopyValue(value);
		}

		protected override IconBase CreateInstanceCore() => new PathIcon();


		private Path CreatePath()
		{
			var path = new Path
			{
				Data = ActualData,
				Stretch = ActualStretch,
				Fill = ActualFill ?? ActualBrushAsFill,
				Stroke = ActualStroke ?? ActualBrushAsStroke,
				StrokeThickness = ActualStrokeThickness,
				StrokeLineJoin = ActualStrokeLineJoin
			};

			return path;
		}

		protected override void OnIconPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			if (_path == null)
				return;

			var pathProperty = PropertyDictionary.GetValueOrDefault(e.Property);

			if (pathProperty != null)
				_path.SetValue(pathProperty, GetActualValue<object>(e.Property));
			else
			{
				var property = e.Property;

				if (property == BrushModeProperty || property == BrushProperty)
				{
					if (ActualFill == null)
						_path.Fill = ActualBrushAsFill;

					if (ActualStroke == null)
						_path.Stroke = ActualBrushAsStroke;
				}
			}
		}
	}
}