// <copyright file="ContextPopupControlService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	internal class ContextPopupControlService
	{
		public static readonly DependencyProperty ControllerSelectorProperty = DPM.RegisterAttached<IPopupControllerSelector, ContextPopupControlService>
			("ControllerSelector", OnControllerSelectorPropertyChanged);

		private static ContextPopupControlService _instance;

		private static readonly object Lock = new();

		private ContextPopupControlService()
		{
			var mouseEvents = MouseRootsEventsProducer.Instance;

			mouseEvents.MouseRightButtonDown += InstanceOnMouseRightButtonDown;
			mouseEvents.MouseRightButtonUp += InstanceOnMouseRightButtonUp;
		}

		public static ContextPopupControlService Current => EnsureInstance();

		public static void ClearControllerSelector(FrameworkElement frameworkElement)
		{
			frameworkElement.ClearValue(ControllerSelectorProperty);
		}

		private static ContextPopupControlService EnsureInstance()
		{
			if (_instance != null)
				return _instance;

			lock (Lock)
			{
				var dispatcher = DispatcherUtils.ApplicationDispatcher;

				if (dispatcher.CheckAccess())
					return _instance = new ContextPopupControlService();

				var waitResult = new WaitResult<ContextPopupControlService>();

				dispatcher.BeginInvoke(() => waitResult.Result = new ContextPopupControlService());

				_instance = waitResult.Result;
			}

			return _instance;
		}

		private static PopupControlController GetController(FrameworkElement fre, DependencyObject eventSource)
		{
			return GetControllerSelector(fre)?.SelectController(fre, eventSource);
		}

		private static PopupControlController GetController(MouseEventArgsInt e, out FrameworkElement contextControlOwner)
		{
			contextControlOwner = null;

			if (e.OriginalSource is not DependencyObject dependencyObject)
				return null;

			foreach (var frameworkElement in dependencyObject.GetAncestorsAndSelf(MixedTreeEnumerationStrategy.DisconnectedThenVisualThenLogicalInstance).OfType<FrameworkElement>())
			{
				var controller = GetController(frameworkElement, dependencyObject);

				if (controller == null)
					continue;

				contextControlOwner = frameworkElement;

				return controller;
			}

			return null;
		}

		public static IPopupControllerSelector GetControllerSelector(FrameworkElement frameworkElement)
		{
			return frameworkElement.GetValue<IPopupControllerSelector>(ControllerSelectorProperty);
		}

		private static void InstanceOnMouseRightButtonDown(object sender, MouseButtonEventArgsInt e)
		{
		}

		private static void InstanceOnMouseRightButtonUp(object sender, MouseButtonEventArgsInt e)
		{
			if (e.Handled || MouseService.Instance.RightButtonState == MouseButtonState.Pressed)
				return;

			var controller = GetController(e, out var contextControlOwner);

			//e.Handled = controller?.OpenContextControl(contextControlOwner, e.OriginalSource as DependencyObject) ?? false;

			controller?.OpenContextControl(contextControlOwner, e.OriginalSource as DependencyObject);
		}

		private static void OnControllerSelectorPropertyChanged(DependencyObject dependencyObject, IPopupControllerSelector oldSelector, IPopupControllerSelector newSelector)
		{
			EnsureInstance();
		}

		public static void OnPopupControllerSelectorChanged(FrameworkElement frameworkElement, IPopupControllerSelector oldSelector, IPopupControllerSelector newSelector)
		{
			if (newSelector != null)
				SetControllerSelector(frameworkElement, newSelector);
			else if (oldSelector != null && ReferenceEquals(oldSelector, GetControllerSelector(frameworkElement)))
				ClearControllerSelector(frameworkElement);
		}

		public static void SetControllerSelector(FrameworkElement frameworkElement, IPopupControllerSelector controllerSelector)
		{
			EnsureInstance();

			frameworkElement.SetValue(ControllerSelectorProperty, controllerSelector);
		}
	}
}