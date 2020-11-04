// <copyright file="FocusHelper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;
using System.Windows.Input;
using Zaaml.Core.Weak.Collections;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Input
{
	public static partial class FocusHelper
	{
		#region Static Fields and Constants

		private static readonly WeakLinkedList<QueueFocusHelper> QueryFocusHelpers = new WeakLinkedList<QueueFocusHelper>();

		#endregion

		#region  Methods

		public static void CancelKeyboardFocusQuery(FrameworkElement element)
		{
			QueryFocusHelpers.FirstOrDefault(f => ReferenceEquals(element, f.Element))?.Cancel();
		}

		public static bool EnsureKeyboardFocusWithin(DependencyObject dependencyObject)
		{
			if (IsKeyboardFocusWithin(dependencyObject))
				return true;

			var keyboardFocusedElement = SetKeyboardFocusedElement(dependencyObject);

			return ReferenceEquals(GetKeyboardFocusedElement(), keyboardFocusedElement) == false && IsKeyboardFocusWithin(dependencyObject);
		}

		private static bool IsFocusScopeValid(DependencyObject dependencyObject, DependencyObject focusScope)
		{
			if (ReferenceEquals(focusScope, dependencyObject))
			{
				if (dependencyObject is FrameworkElement frameworkElement && (frameworkElement.IsLoaded == false || frameworkElement.IsMeasureValid == false || frameworkElement.IsArrangeValid == false))
					return false;
			}

			return true;
		}

		public static bool EnsureFocusWithin(DependencyObject dependencyObject)
		{
			var focusScope = FocusManager.GetFocusScope(dependencyObject);

			if (IsFocusScopeValid(focusScope, dependencyObject) == false)
				return false;

			if (ReferenceEquals(dependencyObject, FocusManager.GetFocusedElement(focusScope)))
				return true;

			FocusManager.SetFocusedElement(focusScope, dependencyObject as IInputElement);

			return ReferenceEquals(dependencyObject, FocusManager.GetFocusedElement(focusScope));
		}

		public static bool IsKeyboardFocusWithin(DependencyObject dependencyObject)
		{
			return GetKeyboardFocusedElement() is DependencyObject keyboardFocusedElement && PresentationTreeUtils.IsWithinTree(dependencyObject, keyboardFocusedElement);
		}

		public static void QueryKeyboardFocus(FrameworkElement element)
		{
			QueueFocusHelper.Query(element, QueueFocusHelperKind.Keyboard);
		}

		public static void QueryFocus(FrameworkElement element)
		{
			QueueFocusHelper.Query(element, QueueFocusHelperKind.Logical);
		}

		public static void ClearFocus(FrameworkElement element)
		{
			var focusScope = FocusManager.GetFocusScope(element);

			if (IsFocusScopeValid(focusScope, element) == false)
				return;

			if (ReferenceEquals(FocusManager.GetFocusedElement(focusScope), element))
				FocusManager.SetFocusedElement(focusScope, null);
		}

		#endregion

		#region  Nested Types

		private enum QueueFocusHelperKind
		{
			Keyboard,
			Logical
		}

		private class QueueFocusHelper
		{
			#region Ctors

			private QueueFocusHelper(FrameworkElement element, QueueFocusHelperKind kind)
			{
				Element = element;
				Kind = kind;
			}

			#endregion

			#region Properties

			public FrameworkElement Element { get; }

			public QueueFocusHelperKind Kind { get; }

			private QueryFocusState QueryState { get; set; }

			#endregion

			#region  Methods

			public void Cancel()
			{
				QueryState = QueryFocusState.None;
				QueryFocusHelpers.Remove(this);
			}

			public static void Query(FrameworkElement element, QueueFocusHelperKind kind)
			{
				var query = new QueueFocusHelper(element, kind);

				query.TryFocus(true);

				if (query.QueryState == QueryFocusState.Wait)
					QueryFocusHelpers.Add(query);
			}

			private bool EnsureFocusWithinImpl()
			{
				return Kind == QueueFocusHelperKind.Keyboard ? EnsureKeyboardFocusWithin(Element) : EnsureFocusWithin(Element);
			}

			private void TryFocus(bool wait)
			{
				if (wait)
					QueryState = QueryFocusState.Wait;

				if (QueryState == QueryFocusState.Wait)
					QueryState = EnsureFocusWithinImpl() ? QueryFocusState.Got : QueryFocusState.Wait;

				if (QueryState == QueryFocusState.Wait)
					Element.InvokeOnLayoutUpdate(() => TryFocus(false));

				if (QueryState == QueryFocusState.Got && wait == false)
					QueryFocusHelpers.Remove(this);
			}

			#endregion

			#region  Nested Types

			private enum QueryFocusState
			{
				None,
				Wait,
				Got
			}

			#endregion
		}

		#endregion
	}
}