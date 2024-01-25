// <copyright file="Interaction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Interactivity
{
	public class Interaction
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty EnabledProperty = DependencyPropertyManager.RegisterAttached
			("Enabled", typeof(bool), typeof(Interaction), new PropertyMetadata(BooleanBoxes.False, OnEnabledPropertyChanged));

		public static readonly DependencyProperty IsMouseOverProperty = DependencyPropertyManager.RegisterAttached
			("IsMouseOver", typeof(bool), typeof(Interaction), new PropertyMetadata(BooleanBoxes.False));

		public static readonly DependencyProperty PropertyProperty = DependencyPropertyManager.RegisterAttached
			("Property", typeof(DependencyProperty), typeof(Interaction), new PropertyMetadata(null));

		private static readonly Dictionary<FrameworkElement, LayoutUpdatedHandler> element2LayoutHandler = new Dictionary<FrameworkElement, LayoutUpdatedHandler>();

		#endregion

		#region  Methods

		private static void AttachEvents(DependencyObject dependencyObject)
		{
			var fre = dependencyObject as FrameworkElement;
			if (fre == null)
				return;

			fre.MouseEnter += OnMouseEnter;
			fre.MouseLeave += OnMouseLeave;
			fre.Unloaded += OnUnloaded;

			fre.AddValueChanged(UIElement.VisibilityProperty, OnVisibilityPropertyChanged);
		}

		private static void DetachEvents(DependencyObject dependencyObject)
		{
			var fre = dependencyObject as FrameworkElement;
			if (fre == null)
				return;

			fre.MouseEnter -= OnMouseEnter;
			fre.MouseLeave -= OnMouseLeave;
			fre.Unloaded -= OnUnloaded;

			fre.RemoveValueChanged(UIElement.VisibilityProperty, OnVisibilityPropertyChanged);
		}

		public static bool GetEnabled(UIElement element)
		{
			return (bool) element.GetValue(EnabledProperty);
		}

		public static bool GetIsMouseOver(UIElement element)
		{
			return (bool) element.GetValue(IsMouseOverProperty);
		}

		public static DependencyProperty GetProperty(DependencyObject element)
		{
			return (DependencyProperty) element.GetValue(PropertyProperty);
		}

		private static void OnEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((bool) e.NewValue)
				AttachEvents(d);
			else
				DetachEvents(d);
		}

		private static void OnLayoutUpdate(object sender, EventArgs eventArgs)
		{
			var fre = (FrameworkElement) sender;
			var bx = new Rect(0, 0, fre.ActualWidth, fre.ActualHeight);

			SetIsMouseOver(fre, bx.Contains(MouseInternal.GetPosition(fre)));
		}

		private static void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var fre = (FrameworkElement) sender;
			var layoutUpdateHandler = element2LayoutHandler[fre];

			layoutUpdateHandler.LayoutUpdated += OnLayoutUpdate;
		}

		private static void OnMouseEnter(object sender, MouseEventArgs mouseEventArgs)
		{
			var uie = (UIElement) sender;
			SetIsMouseOver(uie, true);
		}

		private static void OnMouseLeave(object sender, MouseEventArgs e)
		{
			var uie = (UIElement) sender;
			SetIsMouseOver(uie, false);
		}

		private static void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var fre = (FrameworkElement) sender;
			SetIsMouseOver(fre, false);
			//var layoutUpdateHandler = element2LayoutHandler[fre];

			//layoutUpdateHandler.LayoutUpdated -= OnLayoutUpdate;
		}

		private static void OnVisibilityPropertyChanged(object sender, PropertyValueChangedEventArgs eventArgs)
		{
			var fre = (FrameworkElement) sender;
			if (fre.Visibility == Visibility.Collapsed)
				SetIsMouseOver(fre, false);
			else
				fre.InvokeOnLayoutUpdate(f => SetIsMouseOver(f, f.GetIsMouseOver()));
		}

		public static void SetEnabled(UIElement element, bool value)
		{
			element.SetValue(EnabledProperty, value.Box());
		}

		public static void SetIsMouseOver(UIElement element, bool value)
		{
			element.SetValue(IsMouseOverProperty, value.Box());
		}

		public static void SetProperty(DependencyObject element, DependencyProperty value)
		{
			element.SetValue(PropertyProperty, value);
		}

		#endregion

		#region  Nested Types

		internal class LayoutUpdatedHandler : IDisposable
		{
			#region Fields

			public event EventHandler LayoutUpdated;

			#endregion

			#region Ctors

			public LayoutUpdatedHandler(FrameworkElement fre)
			{
				FrameworkElement = fre;
				FrameworkElement.LayoutUpdated += OnFrameworkElementLayoutUpdated;
			}

			#endregion

			#region Properties

			public FrameworkElement FrameworkElement { get; }

			#endregion

			#region  Methods

			private void OnFrameworkElementLayoutUpdated(object sender, EventArgs eventArgs)
			{
				LayoutUpdated?.Invoke(FrameworkElement, EventArgs.Empty);
			}

			#endregion

			#region Interface Implementations

			#region IDisposable

			public void Dispose()
			{
				FrameworkElement.LayoutUpdated -= OnFrameworkElementLayoutUpdated;
			}

			#endregion

			#endregion
		}

		#endregion
	}
}