// <copyright file="TrackBarItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using Zaaml.Core.Runtime;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Primitives.TrackBar
{
	[ContentProperty(nameof(Content))]
	public abstract class TrackBarItem : Control
	{
		public static readonly DependencyProperty ContentProperty = DPM.Register<object, TrackBarItem>
			("Content");

		public static readonly DependencyProperty CanDragProperty = DPM.Register<bool, TrackBarItem>
			("CanDrag");

		static TrackBarItem()
		{
			ControlUtils.OverrideIsTabStop<TrackBarItem>(false);
		}

		public bool CanDrag
		{
			get => (bool)GetValue(CanDragProperty);
			set => SetValue(CanDragProperty, value.Box());
		}

		public object Content
		{
			get => GetValue(ContentProperty);
			set => SetValue(ContentProperty, value);
		}

		internal int Index { get; set; }

		internal TrackBarValueItem NextValueItem
		{
			get
			{
				if (TrackBarControl == null)
					return null;

				var index = Index + 1;

				if (CollectionUtils.IsWithinRanges(index, TrackBarControl.ItemCollection) == false)
					return null;

				for (var i = index; i < TrackBarControl.ItemCollection.Count; i++)
				{
					if (TrackBarControl.ItemCollection[i] is TrackBarValueItem valueItem)
						return valueItem;
				}

				return null;
			}
		}

		internal TrackBarValueItem PrevValueItem
		{
			get
			{
				if (TrackBarControl == null)
					return null;

				var index = Index - 1;

				if (CollectionUtils.IsWithinRanges(index, TrackBarControl.ItemCollection) == false)
					return null;

				for (var i = index; i >= 0; i--)
				{
					if (TrackBarControl.ItemCollection[i] is TrackBarValueItem valueItem)
						return valueItem;
				}

				return null;
			}
		}

		internal TrackBarControl TrackBarControl { get; set; }

		internal void Clamp()
		{
			ClampCore();
		}

		private protected abstract void ClampCore();

		protected virtual void OnDragEnded()
		{
		}

		internal void OnDragEndedInternal()
		{
			OnDragEnded();
		}

		protected virtual void OnDragStarted()
		{
		}

		internal void OnDragStartedInternal()
		{
			OnDragStarted();
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);

			TrackBarControl?.OnItemMouseLeftButtonDown(this, e);
		}
	}
}