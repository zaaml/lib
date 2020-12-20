// <copyright file="PopupControlService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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
	internal class PopupControlService
	{
		public static readonly DependencyProperty ControllerSelectorProperty = DPM.RegisterAttached<IPopupControllerSelector, PopupControlService>
			("ControllerSelector", OnControllerSelectorPropertyChanged);

		private static PopupControlService _instance;

		private static readonly object Lock = new object();

		private PopupControlService()
		{
			var mouseEvents = MouseRootsEventsProducer.Instance;

			mouseEvents.MouseRightButtonDown += InstanceOnMouseRightButtonDown;
			mouseEvents.MouseRightButtonUp += InstanceOnMouseRightButtonUp;
		}

		public static PopupControlService Current => EnsureInstance();

		public static void ClearControllerSelector(FrameworkElement frameworkElement)
		{
			frameworkElement.ClearValue(ControllerSelectorProperty);
		}

		private static PopupControlService EnsureInstance()
		{
			if (_instance != null)
				return _instance;

			lock (Lock)
			{
				var dispatcher = DispatcherUtils.ApplicationDispatcher;

				if (dispatcher.CheckAccess())
					return _instance = new PopupControlService();

				var waitResult = new WaitResult<PopupControlService>();

				dispatcher.BeginInvoke(() => waitResult.Result = new PopupControlService());

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

			if (!(e.OriginalSource is DependencyObject dependencyObject))
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
#if SILVERLIGHT
      if (e.Handled || MouseService.Instance.RightButtonState == MouseButtonState.Released)
        return;

      FrameworkElement contextControlOwner;
      e.Handled = GetController(e, out contextControlOwner) != null;
#endif
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