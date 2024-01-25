// <copyright file="IconPresenterBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Zaaml.PresentationCore.Runtime;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
	public abstract class IconPresenterBase : Panel
	{
		private static readonly DependencyPropertyKey ActualHasIconPropertyKey = DPM.RegisterReadOnly<bool, IconPresenterBase>
			("ActualHasIcon", false, d => d.OnActualHasIconPropertyChangedPrivate);

		public static readonly DependencyProperty ActualHasIconProperty = ActualHasIconPropertyKey.DependencyProperty;

		public static readonly DependencyProperty IconProperty = DPM.Register<IconBase, IconPresenterBase>
			("Icon", i => i.OnIconChanged);

		public static readonly DependencyProperty VerticalIconAlignmentProperty = DPM.Register<VerticalAlignment, IconPresenterBase>
			("VerticalIconAlignment", VerticalAlignment.Center, d => d.OnVerticalIconAlignmentPropertyChangedPrivate);

		public static readonly DependencyProperty HorizontalIconAlignmentProperty = DPM.Register<HorizontalAlignment, IconPresenterBase>
			("HorizontalIconAlignment", HorizontalAlignment.Center, d => d.OnHorizontalIconAlignmentPropertyChangedPrivate);

		private IconBase _actualIcon;

		internal IconPresenterBase()
		{
			Focusable = false;
		}

		public bool ActualHasIcon
		{
			get => (bool)GetValue(ActualHasIconProperty);
			private set => this.SetReadOnlyValue(ActualHasIconPropertyKey, value.Box());
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

				ActualHasIcon = ActualIcon != null;
				OnActualIconChanged();
			}
		}

		internal IconBase ActualIconInternal => ActualIcon;

		public HorizontalAlignment HorizontalIconAlignment
		{
			get => (HorizontalAlignment)GetValue(HorizontalIconAlignmentProperty);
			set => SetValue(HorizontalIconAlignmentProperty, value.Box());
		}

		public IconBase Icon
		{
			get => (IconBase)GetValue(IconProperty);
			set => SetValue(IconProperty, value);
		}

		public VerticalAlignment VerticalIconAlignment
		{
			get => (VerticalAlignment)GetValue(VerticalIconAlignmentProperty);
			set => SetValue(VerticalIconAlignmentProperty, value.Box());
		}

		private protected virtual void OnActualHasIconChanged()
		{
		}

		private void OnActualHasIconPropertyChangedPrivate(bool oldValue, bool newValue)
		{
			OnActualHasIconChanged();
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
			ActualIcon = newIcon;
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
				ActualIcon.SetCurrentValueInternal(VerticalAlignmentProperty, VerticalIconAlignment.Box());

			if (ActualIcon.GetValueSource(HorizontalAlignmentProperty) == PropertyValueSource.Default)
				ActualIcon.SetCurrentValueInternal(HorizontalAlignmentProperty, HorizontalIconAlignment.Box());
		}
	}
}