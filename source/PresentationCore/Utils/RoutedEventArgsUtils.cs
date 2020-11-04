// <copyright file="RoutedEventArgsUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
#if SILVERLIGHT
using System.Linq;
using System.Reflection;
using Zaaml.Annotations;
using Zaaml.Core.Reflection;
using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using System.Collections.Generic;

#endif

namespace Zaaml.PresentationCore.Utils
{
	internal static class RoutedEventArgsUtils
	{
		#region  Methods

		#endregion

#if SILVERLIGHT

		private static readonly Dictionary<Type, Action<object, bool>> HandlersMap = new Dictionary<Type, Action<object, bool>>();

		[UsedImplicitly]
		private static void GenerateCodeTemplate()
		{
			var registerCodeList = new List<string>();

			foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetLoadableTypes()))
			{
				if (!type.IsSubclassOf(typeof(RoutedEventArgs)) || type.IsSubclassOf(typeof(RoutedEventArgsSL)) || type.GetProperty("Handled", BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty) == null) continue;
				var registerCode = $"RegisterHandler<{type.Name}>((a, h) => a.Handled = h);";
				registerCodeList.Add(registerCode);
			}

			var code = string.Join(Environment.NewLine, registerCodeList);
		}

		static RoutedEventArgsUtils()
		{
			RegisterHandler<DragEventArgs>((a, h) => a.Handled = h);
			RegisterHandler<MediaCommandEventArgs>((a, h) => a.Handled = h);
			RegisterHandler<MouseButtonEventArgs>((a, h) => a.Handled = h);
			RegisterHandler<MouseWheelEventArgs>((a, h) => a.Handled = h);
			RegisterHandler<KeyEventArgs>((a, h) => a.Handled = h);
			RegisterHandler<TextCompositionEventArgs>((a, h) => a.Handled = h);
			RegisterHandler<ManipulationStartedEventArgs>((a, h) => a.Handled = h);
			RegisterHandler<ManipulationDeltaEventArgs>((a, h) => a.Handled = h);
			RegisterHandler<ManipulationCompletedEventArgs>((a, h) => a.Handled = h);
			RegisterHandler<GestureEventArgs>((a, h) => a.Handled = h);
			RegisterHandler<ValidationErrorEventArgs>((a, h) => a.Handled = h);
			RegisterHandler<RoutedEventArgsSL>((a, h) => a.Handled = h);
		}

		private static void RegisterHandler<T>(Action<T, bool> handler) where T : RoutedEventArgs
		{
			HandlersMap[typeof(T)] = (args, handled) => handler((T) args, handled);
		}

		public static void SetHandled(RoutedEventArgs args, bool handled)
		{
			HandlersMap.GetValueOrDefault(args.GetType())?.Invoke(args, handled);
		}
#else
		public static void SetHandled(RoutedEventArgs args, bool handled)
		{
			args.Handled = true;
		}
#endif
	}
}
