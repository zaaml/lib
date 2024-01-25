// <copyright file="MouseHoverVisualStateFlickeringReducer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Utils
{
	internal abstract class FlickeringReducer<TItem> : IDisposable
		where TItem : FrameworkElement
	{
		public abstract void OnArrange();

		public abstract void OnMouseMove(MouseEventArgs mouseEventArgs);

		public abstract void OnMouseWheel();

		public virtual void Dispose()
		{
		}
	}

	internal sealed class MouseHoverVisualStateFlickeringReducer<TItem> : FlickeringReducer<TItem> where TItem : FrameworkElement, ILayoutInformation, MouseHoverVisualStateFlickeringReducer<TItem>.IClient
	{
		private readonly Panel _panel;
		private bool _arrangeAfterWheel;
		private long _frameCount;

		public MouseHoverVisualStateFlickeringReducer(Panel panel)
		{
			_panel = panel;
		}

		private long FrameCount
		{
			get => _frameCount;
			set
			{
				if (_frameCount == value)
					return;

				if (_frameCount < long.MaxValue)
					CompositionTarget.Rendering += CompositionTargetOnRendering;
				else
					CompositionTarget.Rendering -= CompositionTargetOnRendering;

				_frameCount = value;
			}
		}

		private HashSet<TItem> ImplicitMouseItems { get; } = new HashSet<TItem>();

		private Point LocalMousePoint { get; set; } = new Point(double.MaxValue, double.MaxValue);

		private void CleanImplicitMouseItems(bool arrange)
		{
			if (ImplicitMouseItems.Count == 0)
				return;

			foreach (var treeViewItem in _panel.Children.OfType<TItem>())
			{
				if (ImplicitMouseItems.Remove(treeViewItem) == false)
					continue;

				treeViewItem.UpdateVisualStateOnArrange(null);

				if (arrange)
					treeViewItem.Arrange(treeViewItem.ArrangeRect);
			}
		}

		private void CompositionTargetOnRendering(object sender, EventArgs e)
		{
			if (FrameCounter.Frame <= FrameCount + 2)
				return;

			CleanImplicitMouseItems(false);

			FrameCount = long.MaxValue;
		}

		public override void Dispose()
		{
			FrameCount = long.MaxValue;

			ImplicitMouseItems.Clear();
		}

		public override void OnArrange()
		{
			if (_arrangeAfterWheel)
			{
				foreach (var item in _panel.Children.OfType<TItem>())
				{
					var containsMouse = item.ArrangeRect.Contains(LocalMousePoint);

					if (item.IsMouseOver == containsMouse)
					{
						if (ImplicitMouseItems.Remove(item))
						{
							item.UpdateVisualStateOnArrange(null);
							item.Arrange(item.ArrangeRect);
						}

						continue;
					}

					ImplicitMouseItems.Add(item);

					item.UpdateVisualStateOnArrange(containsMouse);
					item.Arrange(item.ArrangeRect);

					FrameCount = FrameCounter.Frame;
				}
			}
			else
				CleanImplicitMouseItems(true);

			_arrangeAfterWheel = false;
		}

		public override void OnMouseMove(MouseEventArgs mouseEventArgs)
		{
			LocalMousePoint = mouseEventArgs.GetPosition(_panel);
		}

		public override void OnMouseWheel()
		{
			_arrangeAfterWheel = true;
		}

		public interface IClient : ILayoutInformation
		{
			void UpdateVisualStateOnArrange(bool? isMouseOver);
		}
	}
}