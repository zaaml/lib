// <copyright file="DropDownEditableSelectorBase.DropDown.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.DropDown
{
	public abstract partial class DropDownEditableSelectorBase<TItemsControl, TItem>
	{
		protected virtual bool CanCloseDropDownOnRequest => true;

		private protected override void OnIsDropDownOpenChangedInternal()
		{
			base.OnIsDropDownOpenChangedInternal();

			if (ActualSuspendDropDownHandler)
				return;

			if (IsDropDownOpen == false)
				CancelEdit();
			else
				BeginEdit();
		}

		private void OpenDropDownOnTextInput(bool force)
		{
			if (IsDropDownOpen == false) 
				_delayOpenDropDown.Invoke(force ? TimeSpan.Zero : GetFilterDelay());
		}

		private protected override void OnQueryCloseDropDownOnClick(object sender, QueryCloseOnClickEventArgs e)
		{
			if (e.MouseEventSource == null)
				return;

			if (IsDescendant(e.MouseEventSource))
				e.CanClose = false;
		}

		private void PopupCloseControllerOnClosing(object sender, CancelEventArgs e)
		{
			if (IsSelectionHandling)
				return;

			if (CanCloseDropDownOnRequest == false)
				e.Cancel = true;
		}
	}
}