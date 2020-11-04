// <copyright file="PopupHierarchyController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Services;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	internal class PopupHierarchyController
	{
		public static readonly DependencyProperty PopupHierarchyParentProperty = DPM.RegisterAttached<FrameworkElement, PopupHierarchyController>
			("PopupHierarchyParent");

		private static readonly DependencyProperty IsHierarchyItemProperty = DPM.RegisterAttached<bool, PopupHierarchyController>
			("IsHierarchyItem");

		private static void ElementOnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var element = (FrameworkElement) sender;

			FrameworkElement current = null;

			foreach (var parent in element.GetVisualAncestors().OfType<FrameworkElement>())
			{
				current = parent;

				if (GetIsHierarchyItem(current))
					break;
			}

			if (current != null)
			{
				current.GetServiceOrCreate(() => new PopupHierarchyService()).AddPopup((IPopup) element);
				SetPopupHierarchyParent(element, current);
			}
		}

		private static void ElementOnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var element = (FrameworkElement) sender;
			var hierarchyParent = GetPopupHierarchyParent(element);

			if (hierarchyParent != null && hierarchyParent.IsVisualAncestorOf(element) == false)
			{
				hierarchyParent.GetService<PopupHierarchyService>().RemovePopup((IPopup) element);
				SetPopupHierarchyParent(element, null);
			}
		}

		public static void EnableHierarchy<T>(T element) where T : FrameworkElement, IPopup
		{
			element.Loaded += ElementOnLoaded;
			element.Unloaded += ElementOnUnloaded;

			SetIsHierarchyItem(element, true);
		}

		private static bool GetIsHierarchyItem(DependencyObject element)
		{
			return (bool) element.GetValue(IsHierarchyItemProperty);
		}

		public static FrameworkElement GetPopupHierarchyParent(DependencyObject element)
		{
			return (FrameworkElement) element.GetValue(PopupHierarchyParentProperty);
		}

		private static void SetIsHierarchyItem(DependencyObject element, bool value)
		{
			element.SetValue(IsHierarchyItemProperty, value);
		}

		public static void SetPopupHierarchyParent(DependencyObject element, FrameworkElement value)
		{
			element.SetValue(PopupHierarchyParentProperty, value);
		}
	}

	internal class PopupHierarchyService : ServiceBase<FrameworkElement>
	{
		private readonly List<IPopup> _popupItems = new List<IPopup>();
		private IPopup _currentOpenedPopup;
		private bool _skipEvent;

		public IPopup CurrentOpenedPopup
		{
			get => _currentOpenedPopup;
			set
			{
				if (ReferenceEquals(_currentOpenedPopup, value))
					return;

				if (_currentOpenedPopup != null)
					_currentOpenedPopup.IsOpen = false;

				_currentOpenedPopup = value;
			}
		}

		public void AddPopup(IPopup popup)
		{
			popup.IsOpenChanged += OnPopupIsOpenChanged;
			_popupItems.Add(popup);

			if (popup.IsOpen)
				CurrentOpenedPopup = popup;
		}

		private void OnPopupIsOpenChanged(object sender, EventArgs eventArgs)
		{
			if (_skipEvent) return;
			_skipEvent = true;

			var popup = (IPopup) sender;

			if (popup.IsOpen)
				CurrentOpenedPopup = popup;
			else if (ReferenceEquals(CurrentOpenedPopup, popup))
				CurrentOpenedPopup = null;

			_skipEvent = false;
		}

		public void RemovePopup(IPopup popup)
		{
			if (CurrentOpenedPopup == popup)
				CurrentOpenedPopup = null;

			_popupItems.Remove(popup);

			popup.IsOpenChanged -= OnPopupIsOpenChanged;
		}
	}
}