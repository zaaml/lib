// <copyright file="FocusNavigator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.UI.Controls.ScrollView;
using NativeControl = System.Windows.Controls.Control;

namespace Zaaml.UI.Controls.Core
{
	internal abstract class FocusNavigator<TItem> where TItem : NativeControl
	{
		#region Properties

		protected abstract NativeControl ControlCore { get; }

		public virtual TItem FocusedItem { get; set; }

		public virtual void ClearFocus()
		{
			var focusedItem = FocusedItem ?? FocusedItemCache;

			if (focusedItem != null)
				FocusHelper.ClearFocus(focusedItem);
		}

		private TItem FocusedItemCache { get; set; }

		#endregion

		#region  Methods

		public abstract void EnsureFocus();

		public abstract bool HandleNavigationKey(Key key);

		public virtual bool IsNavigationKey(Key key)
		{
			switch (key)
			{
				case Key.PageUp:
				case Key.PageDown:
				case Key.End:
				case Key.Home:
				case Key.Left:
				case Key.Up:
				case Key.Right:
				case Key.Down:

					return true;

				default:

					return false;
			}
		}

		internal bool IsLogical { get; set; }

		protected bool ApplyFocus(TItem item)
		{
			if (IsLogical)
			{
				var focusScope = FocusManager.GetFocusScope(item);

				if (focusScope == null)
					return FocusHelper.Focus(item);

				FocusManager.SetFocusedElement(focusScope, item);

				return ReferenceEquals(FocusManager.GetFocusedElement(focusScope), item);
			}

			return FocusHelper.Focus(item);
		}

		protected virtual void OnItemAttached(TItem item)
		{
		}

		internal void OnItemAttachedInternal(TItem item)
		{
			OnItemAttached(item);
		}

		protected virtual void OnItemDetached(TItem item)
		{
		}

		internal void OnItemDetachedInternal(TItem item)
		{
			OnItemDetached(item);
		}

		protected virtual void OnItemGotFocus(TItem item)
		{
		}

		protected virtual void OnItemLostFocus(TItem item)
		{
		}

		internal void OnItemGotFocusInternal(TItem item)
		{
			FocusedItemCache = item;

			OnItemGotFocus(item);
		}

		internal void OnItemLostFocusInternal(TItem item)
		{
			OnItemLostFocus(item);

			if (ReferenceEquals(FocusedItemCache, item))
				FocusedItemCache = null;
		}

		#endregion
	}

	internal abstract class FocusNavigator<TControl, TItem> : FocusNavigator<TItem>
		where TControl : NativeControl, IFocusNavigatorAdvisor<TItem>
		where TItem : NativeControl
	{
		#region Ctors

		protected FocusNavigator(TControl control)
		{
			Control = control;
		}

		#endregion

		#region Properties

		public TControl Control { get; }

		protected override NativeControl ControlCore => Control;

		#endregion

		#region  Methods

		protected bool IsItemsHostVisible => Control.IsItemsHostVisible;

		protected int ItemsCount => Control.ItemsCount;

		protected Orientation LogicalOrientation => Control.LogicalOrientation;

		protected ScrollViewControl ScrollView => Control.ScrollView;

		protected void FocusItem(TItem item)
		{
			var controlItem = item;

			if (controlItem == null)
				return;

			var reenter = false;

			for(var i = 0; i < 3; i++)
			{
				if (controlItem.IsVisualDescendantOf(Control))
				{
					if (ApplyFocus(item))
						return;
				}

				if (reenter || Control.IsVirtualizing == false || Control.ScrollView == null)
					return;

				Control.ScrollView.UpdateLayout();

				reenter = true;
			}
		}

		#endregion
	}
}