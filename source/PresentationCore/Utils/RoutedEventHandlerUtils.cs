// <copyright file="RoutedEventHandlerUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

#if SILVERLIGHT
using System.Collections.Generic;
using System.Windows.Input;
using Zaaml.Core.Extensions;
#endif

namespace Zaaml.PresentationCore.Utils
{
	internal static class RoutedEventHandlerUtils
	{
#if SILVERLIGHT

		#region Static Fields and Constants

		private static readonly Dictionary<RoutedEvent, Func<RoutedEventHandler, Delegate>> HandlerFactoryDict = new Dictionary<RoutedEvent, Func<RoutedEventHandler, Delegate>>();

		#endregion

		#region Ctors

		static RoutedEventHandlerUtils()
		{
			RegisterHandlerFactory(UIElement.KeyDownEvent, u => new KeyEventHandler(u));
			RegisterHandlerFactory(UIElement.KeyUpEvent, u => new KeyEventHandler(u));
			RegisterHandlerFactory(UIElement.MouseLeftButtonDownEvent, u => new MouseButtonEventHandler(u));
			RegisterHandlerFactory(UIElement.MouseLeftButtonDownEvent, u => new MouseButtonEventHandler(u));
			RegisterHandlerFactory(UIElement.MouseRightButtonDownEvent, u => new MouseButtonEventHandler(u));
			RegisterHandlerFactory(UIElement.MouseRightButtonUpEvent, u => new MouseButtonEventHandler(u));
			RegisterHandlerFactory(UIElement.MouseWheelEvent, u => new MouseWheelEventHandler(u));
			RegisterHandlerFactory(UIElement.TextInputEvent, u => new TextCompositionEventHandler(u));
			RegisterHandlerFactory(UIElement.TextInputStartEvent, u => new TextCompositionEventHandler(u));
			RegisterHandlerFactory(UIElement.TextInputUpdateEvent, u => new TextCompositionEventHandler(u));
		}

		#endregion

		#region  Methods

		public static Delegate CreateRoutedEventHandler(RoutedEvent routedEvent, RoutedEventHandler underlyingDelegate)
		{
			return HandlerFactoryDict.GetValueOrDefault(routedEvent)?.Invoke(underlyingDelegate) ?? underlyingDelegate;
		}

		private static void RegisterHandlerFactory(RoutedEvent routedEvent, Func<RoutedEventHandler, Delegate> factory)
		{
			HandlerFactoryDict[routedEvent] = factory;
		}

		#endregion

#else
		public static Delegate CreateRoutedEventHandler(RoutedEvent routedEvent, RoutedEventHandler underlyingDelegate)
		{
			return underlyingDelegate;
		}
#endif
	}
}
