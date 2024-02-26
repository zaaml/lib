// <copyright file="FrameworkElementExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Behaviors;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Extensions
{
	public static class FrameworkElementExtensions
	{
		public static void AddBehavior(this FrameworkElement fre, BehaviorBase behavior)
		{
			Extension.GetBehaviors(fre).Add(behavior);
		}

		public static void ClearValidationError(this FrameworkElement frameworkElement)
		{
			ValidationUtils.ClearValidationError(frameworkElement);
		}

		public static void ConstraintSize(this FrameworkElement fre, Size size)
		{
			fre.MaxWidth = size.Width;
			fre.MaxHeight = size.Height;
		}

		internal static void DetachFromLogicalParent(this FrameworkElement frameworkElement)
		{
			var logicalParent = frameworkElement?.GetLogicalParent<FrameworkElement>();

			if (logicalParent == null)
				return;

			if (logicalParent is Panel panel)
			{
				panel.Children.Remove(frameworkElement);

				return;
			}

			if (logicalParent is ContentControl contentControl)
			{
				contentControl.Content = null;

				return;
			}

			if (logicalParent is ContentPresenter contentPresenter)
			{
				contentPresenter.Content = null;

				return;
			}

			if (logicalParent is Border border)
				border.Child = null;
		}

		public static Size GetActualSize(this FrameworkElement fre)
		{
			return new Size(fre.ActualWidth, fre.ActualHeight);
		}

		public static TBehavior GetBehavior<TBehavior>(this FrameworkElement element) where TBehavior : BehaviorBase
		{
			return element.GetBehaviors().OfType<TBehavior>().FirstOrDefault();
		}

		public static BehaviorCollection GetBehaviors(this FrameworkElement element)
		{
			return Extension.GetBehaviors(element);
		}

		internal static Rect GetBoundingBox(this FrameworkElement element, FrameworkElement relativeTo)
		{
			if (ReferenceEquals(element, relativeTo))
				return element.GetClientBox();

			if (element.IsVisualDescendantOf(relativeTo) == false)
				throw new InvalidOperationException("Specified elements are not relatives");

			return element.TransformToVisual(relativeTo).TransformBounds(element.GetClientBox());
		}

		public static Rect GetClientBox(this FrameworkElement fre)
		{
			return new Rect(0, 0, fre.ActualWidth, fre.ActualHeight);
		}

		internal static Size GetCurrentSize(this FrameworkElement fre)
		{
			return new Size(fre.Width.IsNaN() ? fre.ActualWidth : fre.Width, fre.Height.IsNaN() ? fre.ActualHeight : fre.Height);
		}

		public static Size GetMaxSize(this FrameworkElement fre)
		{
			return new Size(fre.MaxWidth, fre.MaxHeight);
		}

		public static Size GetMinSize(this FrameworkElement fre)
		{
			return new Size(fre.MinWidth, fre.MinHeight);
		}

		public static Rect GetScreenDeviceBox(this FrameworkElement element)
		{
			return element.GetScreenLogicalBox().FromLogicalToDevice();
		}

		public static Point GetScreenDeviceLocation(this FrameworkElement element)
		{
			return element.GetScreenLogicalLocation().FromLogicalToDevice();
		}

		public static Rect GetScreenLogicalBox(this FrameworkElement element)
		{
			return UIElementTransformUtils.TransformToScreen(element).TransformBounds(element.GetClientBox());
		}

		public static Point GetScreenLogicalLocation(this FrameworkElement element)
		{
			return UIElementTransformUtils.TransformToScreen(element).Transform(new Point());
		}

		public static Rect GetVisualRootBox(this FrameworkElement element)
		{
			var visualRoot = element.GetVisualRoot<Visual>();
			var clientBox = element.GetClientBox();

			return ReferenceEquals(visualRoot, element) ? clientBox : element.TransformToAncestor(visualRoot).TransformBounds(clientBox);
		}

		public static Rect GetTemplateRootBox(this FrameworkElement element)
		{
			var templateRoot = element.GetTemplateRoot();
			var clientBox = element.GetClientBox();

			return ReferenceEquals(templateRoot, element) ? clientBox : element.TransformToAncestor(templateRoot).TransformBounds(clientBox);
		}

		public static bool HasValidationError(this FrameworkElement frameworkElement)
		{
			return ValidationUtils.HasValidationError(frameworkElement);
		}

		public static void InvokeOnDeltaFrame(this FrameworkElement fre, int deltaFrame, Action action)
		{
			fre.InvokeOnFrame(FrameCounter.Frame + deltaFrame, action);
		}

		public static void InvokeOnFrame(this FrameworkElement fre, long frame, Action action)
		{
			void Handler(object sender, EventArgs e)
			{
				if (FrameCounter.Frame < frame)
					return;

				CompositionTarget.Rendering -= Handler;

				action();
			}

			if (frame <= FrameCounter.Frame)
				return;

			CompositionTarget.Rendering += Handler;
		}

		public static void InvokeOnLayoutUpdate<T>(this T fre, Action<T> action) where T : FrameworkElement
		{
			void Handler(object sender, EventArgs e)
			{
				fre.LayoutUpdated -= Handler;

				action(fre);
			}

			fre.LayoutUpdated += Handler;
		}

		public static void InvokeOnLayoutUpdate(this FrameworkElement fre, Action action)
		{
			void Handler(object sender, EventArgs e)
			{
				fre.LayoutUpdated -= Handler;

				action();
			}

			fre.LayoutUpdated += Handler;
		}

		internal static void InvokeOnLayoutUpdateUntil(this FrameworkElement fre, Func<bool> action)
		{
			void Handler(object sender, EventArgs e)
			{
				fre.LayoutUpdated -= Handler;

				if (action() == false)
					fre.LayoutUpdated += Handler;
			}

			fre.LayoutUpdated += Handler;
		}

		public static void InvokeOnLoaded(this FrameworkElement fre, Action action)
		{
			void Handler(object sender, RoutedEventArgs e)
			{
				fre.Loaded -= Handler;

				action();
			}

			fre.Loaded += Handler;
		}

		public static void InvokeOnUnloaded(this FrameworkElement fre, Action action)
		{
			void Handler(object sender, RoutedEventArgs e)
			{
				fre.Unloaded -= Handler;

				action();
			}

			fre.Unloaded += Handler;
		}

		internal static bool IsInLiveTree(this FrameworkElement fre)
		{
			return PresentationTreeUtils.EnumerateVisualRoots().Where(e => e.Dispatcher.CheckAccess()).Any(vr => ReferenceEquals(vr, fre) || vr?.IsVisualAncestorOf(fre) == true);
		}

		public static Size MaxSize(this FrameworkElement fre)
		{
			return new Size(fre.MaxWidth, fre.MaxHeight);
		}

		public static Size MinSize(this FrameworkElement fre)
		{
			return new Size(fre.MinWidth, fre.MinHeight);
		}

		internal static Size OnMeasureOverride(this FrameworkElement fre, Func<Size, Size> measureImpl, Size availableSize)
		{
			try
			{
				return measureImpl(availableSize);
			}
			catch (Exception e)
			{
				LogService.LogError(e);
			}

			return XamlConstants.ZeroSize;
		}

		public static void RemoveBehavior(this FrameworkElement fre, BehaviorBase behavior)
		{
			Extension.GetBehaviors(fre).Remove(behavior);
		}

		public static void SetNaNSize(this FrameworkElement fre)
		{
			SetSize(fre, XamlConstants.NanSize);
		}

		public static void SetSize(this FrameworkElement fre, Size size)
		{
			fre.Width = size.Width;
			fre.Height = size.Height;
		}

		public static void SetValidationError(this FrameworkElement frameworkElement, string message)
		{
			ValidationUtils.SetValidationError(frameworkElement, message);
		}
	}
}