// <copyright file="ZoomSelector.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Artboard
{
	public abstract class ZoomSelector : InheritanceContextObject
	{
		public static readonly DependencyProperty SelectZoomProperty = DPM.RegisterAttached<double, ZoomSelector>
			("SelectZoom", default, OnSelectZoomPropertyChangedPrivate);

		public static double GetSelectZoom(DependencyObject dependencyObject)
		{
			return (double) dependencyObject.GetValue(SelectZoomProperty);
		}

		private static void OnSelectZoomPropertyChangedPrivate(DependencyObject dependencyObject, double oldValue, double newValue)
		{
		}

		public static void SetSelectZoom(DependencyObject dependencyObject, double zoom)
		{
			dependencyObject.SetValue(SelectZoomProperty, zoom);
		}
	}

	[ContentProperty(nameof(ValueCollection))]
	public abstract class ZoomSelector<TValue> : ZoomSelector where TValue : InheritanceContextObject
	{
		public static readonly DependencyProperty ZoomProperty = DPM.Register<double, ZoomSelector<TValue>>
			("Zoom", default, d => d.OnZoomPropertyChangedPrivate);

		private static readonly DependencyPropertyKey SelectedValuePropertyKey = DPM.RegisterReadOnly<TValue, ZoomSelector<TValue>>
			("SelectedValue", default, d => d.OnSelectedValuePropertyChangedPrivate);

		private static readonly DependencyPropertyKey ValueCollectionPropertyKey = DPM.RegisterReadOnly<ZoomValueCollection<TValue>, ZoomSelector<TValue>>
			("ValueCollectionInt", default);

		public static readonly DependencyProperty ValueCollectionProperty = ValueCollectionPropertyKey.DependencyProperty;

		public static readonly DependencyProperty SelectedValueProperty = SelectedValuePropertyKey.DependencyProperty;

		public TValue SelectedValue
		{
			get => (TValue) GetValue(SelectedValueProperty);
			private set => this.SetReadOnlyValue(SelectedValuePropertyKey, value);
		}

		public ZoomValueCollection<TValue> ValueCollection => this.GetValueOrCreate(ValueCollectionPropertyKey, () => new ZoomValueCollection<TValue>(this));

		public double Zoom
		{
			get => (double) GetValue(ZoomProperty);
			set => SetValue(ZoomProperty, value);
		}

		protected virtual void OnSelectedValueChanged(TValue oldValue, TValue newValue)
		{
		}

		private void OnSelectedValuePropertyChangedPrivate(TValue oldValue, TValue newValue)
		{
			OnSelectedValueChanged(oldValue, newValue);
		}

		protected virtual void OnZoomChanged()
		{
		}

		private void OnZoomPropertyChangedPrivate(double oldValue, double newValue)
		{
			UpdateSelectedValue();

			OnZoomChanged();
		}

		internal void Update()
		{
			UpdateSelectedValue();
		}

		private void UpdateSelectedValue()
		{
			if (ValueCollection.Count == 0)
			{
				SelectedValue = null;

				return;
			}

			var selectedValue = ValueCollection.First();

			foreach (var zoomModel in ValueCollection.Where(zoomModel => GetSelectZoom(zoomModel) <= Zoom))
				selectedValue = zoomModel;

			SelectedValue = selectedValue;
		}
	}
}