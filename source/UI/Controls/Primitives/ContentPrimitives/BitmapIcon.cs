// <copyright file="BitmapIcon.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
	public sealed partial class BitmapIcon : IconBase
	{
		public static readonly DependencyProperty SourceProperty = DPM.RegisterAttached<ImageSource, BitmapIcon>
			("Source", OnIconPropertyChanged);

		public static readonly DependencyProperty StretchProperty = DPM.RegisterAttached<Stretch, BitmapIcon>
			("Stretch", Stretch.None, OnIconPropertyChanged);

		public static readonly DependencyProperty StretchDirectionProperty = DPM.RegisterAttached<StretchDirection, BitmapIcon>
			("StretchDirection", StretchDirection.Both, OnIconPropertyChanged);

		private static readonly List<DependencyProperty> Properties =
		[
			SourceProperty,
			StretchProperty,
			StretchDirectionProperty
		];

		private static readonly Dictionary<DependencyProperty, DependencyProperty> PropertyDictionary = new()
		{
			{ SourceProperty, Image.SourceProperty },
			{ StretchProperty, Image.StretchProperty },
			{ StretchDirectionProperty, Image.StretchDirectionProperty }
		};

		private Image _image;

		static BitmapIcon()
		{
			Factories[SourceProperty] = () => new BitmapIcon();
		}

		private ImageSource ActualSource => GetActualValue<ImageSource>(SourceProperty);

		private Stretch ActualStretch => GetActualValue<Stretch>(StretchProperty);

		private StretchDirection ActualStretchDirection => GetActualValue<StretchDirection>(StretchDirectionProperty);

		internal Uri BaseUri
		{
			get => (Uri)GetValue(BaseUriHelper.BaseUriProperty);
			set => SetValue(BaseUriHelper.BaseUriProperty, value);
		}

		protected internal override FrameworkElement IconElement => _image ??= CreateImage();

		protected override IEnumerable<DependencyProperty> PropertiesCore => Properties;

		public ImageSource Source
		{
			get => (ImageSource)GetValue(SourceProperty);
			set => SetValue(SourceProperty, value);
		}

		public Stretch Stretch
		{
			get => (Stretch)GetValue(StretchProperty);
			set => SetValue(StretchProperty, value);
		}

		public StretchDirection StretchDirection
		{
			get => (StretchDirection)GetValue(StretchDirectionProperty);
			set => SetValue(StretchDirectionProperty, value);
		}

		private Image CreateImage()
		{
			var image = new Image
			{
				Source = ActualSource,
				Stretch = ActualStretch,
				StretchDirection = ActualStretchDirection
			};

			var uriContext = (IUriContext)image;

			uriContext.BaseUri = BaseUri;

			return image;
		}

		protected override IconBase CreateInstanceCore() => new BitmapIcon();

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			UpdateUri();

			return base.MeasureOverrideCore(availableSize);
		}

		protected override void OnIconPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			if (_image == null)
				return;

			var imageProperty = PropertyDictionary.GetValueOrDefault(e.Property);

			if (imageProperty != null)
				_image.SetValue(imageProperty, GetActualValue<object>(e.Property));
		}

		private void UpdateUri()
		{
			if (_image?.Source is not IUriContext sourceUriContext)
				return;

			var baseUri = BaseUriHelper.GetBaseUri(this);

			if (_image.Source.IsFrozen || !(sourceUriContext.BaseUri == null) || !(baseUri != null))
				return;

			sourceUriContext.BaseUri = baseUri;
		}
	}
}