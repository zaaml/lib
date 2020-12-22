// <copyright file="IconPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
	public abstract class IconPresenterBase : Panel
	{
		public static readonly DependencyProperty IconProperty = DPM.Register<IconBase, IconPresenterBase>
			("Icon", i => i.OnIconChanged);

		public static readonly DependencyProperty VerticalIconAlignmentProperty = DPM.Register<VerticalAlignment, IconPresenterBase>
			("VerticalIconAlignment", VerticalAlignment.Center, d => d.OnVerticalIconAlignmentPropertyChangedPrivate);

		public static readonly DependencyProperty HorizontalIconAlignmentProperty = DPM.Register<HorizontalAlignment, IconPresenterBase>
			("HorizontalIconAlignment", HorizontalAlignment.Center, d => d.OnHorizontalIconAlignmentPropertyChangedPrivate);

		private IconBase _actualIcon;

		internal IconPresenterBase()
		{
#if !SILVERLIGHT
			Focusable = false;
#endif
		}

		protected IconBase ActualIcon
		{
			get => _actualIcon;
			private set
			{
				if (ReferenceEquals(_actualIcon, value))
					return;

				if (_actualIcon != null)
					_actualIcon.Presenter = null;

				IconBase.UseIcon(ref _actualIcon, value, this);

				if (_actualIcon != null)
					_actualIcon.Presenter = this;

				OnActualIconChanged();
			}
		}

		internal IconBase ActualIconInternal => ActualIcon;

		public HorizontalAlignment HorizontalIconAlignment
		{
			get => (HorizontalAlignment) GetValue(HorizontalIconAlignmentProperty);
			set => SetValue(HorizontalIconAlignmentProperty, value);
		}

		public IconBase Icon
		{
			get => (IconBase) GetValue(IconProperty);
			set => SetValue(IconProperty, value);
		}

		public VerticalAlignment VerticalIconAlignment
		{
			get => (VerticalAlignment) GetValue(VerticalIconAlignmentProperty);
			set => SetValue(VerticalIconAlignmentProperty, value);
		}

		protected virtual void OnActualIconChanged()
		{
			UpdateIconAlignment();
			InvalidateMeasure();
		}

		private void OnHorizontalIconAlignmentPropertyChangedPrivate(HorizontalAlignment oldValue, HorizontalAlignment newValue)
		{
			UpdateIconAlignment();
		}

		private void OnIconChanged(IconBase oldIcon, IconBase newIcon)
		{
			ActualIcon = newIcon?.SharedResource == true ? newIcon.Clone(false) : newIcon;
		}

		private void OnVerticalIconAlignmentPropertyChangedPrivate(VerticalAlignment oldValue, VerticalAlignment newValue)
		{
			UpdateIconAlignment();
		}

		private void UpdateIconAlignment()
		{
			if (ActualIcon == null)
				return;

			if (ActualIcon.GetValueSource(VerticalIconAlignmentProperty) == PropertyValueSource.Default)
				ActualIcon.SetCurrentValueInternal(VerticalAlignmentProperty, VerticalIconAlignment);

			if (ActualIcon.GetValueSource(HorizontalAlignmentProperty) == PropertyValueSource.Default)
				ActualIcon.SetCurrentValueInternal(HorizontalAlignmentProperty, HorizontalIconAlignment);
		}
	}

	[ContentProperty(nameof(Icon))]
	public sealed class IconPresenter : IconPresenterBase
	{
		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			var icon = ActualIcon;

			if (icon == null)
				return finalSize;

			icon.Arrange(finalSize.Rect());

			return finalSize;
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			var icon = ActualIcon;

			if (icon == null)
				return XamlConstants.ZeroSize;

			icon.Measure(availableSize);

			return icon.DesiredSize;
		}
	}

	internal interface IIconPresenter
	{
		IconBase Icon { get; }
	}
}