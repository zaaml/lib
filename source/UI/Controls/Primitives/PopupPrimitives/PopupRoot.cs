// <copyright file="PopupRoot.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public sealed class PopupRoot : ControlRootBase
	{
		#region Fields

		private Popup _popup;

		#endregion

		#region Properties

		internal Popup Popup
		{
			get => _popup;
			set
			{
				if (ReferenceEquals(_popup, value))
					return;

				if (_popup != null)
					DetachPopup();

				_popup = value;

				if (_popup != null)
					AttachPopup();
			}
		}

		#endregion

		#region  Methods

		private void AttachPopup()
		{
			FrameworkElement nativePopup = _popup.PopupSource;

			switch (_popup.TreeMode)
			{
				case PopupTreeMode.Logical:

					AddLogicalChild(nativePopup);

					break;

				case PopupTreeMode.Visual:

					if (nativePopup is Window)
						PresentationTreeUtils.SetDisconnectedParent(nativePopup, _popup);
					else
						Children.Add(_popup.PopupSource);

					break;

				default:

					PresentationTreeUtils.SetDisconnectedParent(nativePopup, _popup);

					break;
			}
		}

		private void DetachPopup()
		{
			FrameworkElement nativePopup = _popup.PopupSource;

			switch (_popup.TreeMode)
			{
				case PopupTreeMode.Logical:

					RemoveLogicalChild(_popup.PopupSource);

					break;

				case PopupTreeMode.Visual:

					if (nativePopup is Window)
						PresentationTreeUtils.SetDisconnectedParent(_popup.PopupSource, null);
					else
						Children.Clear();

					break;

				default:

					PresentationTreeUtils.SetDisconnectedParent(_popup.PopupSource, null);

					break;
			}
		}

		#endregion
	}
}