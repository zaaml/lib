// <copyright file="BackstageViewHost.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.BackstageView
{
	internal sealed class BackstageViewHost
	{
		#region Type: Fields

		private readonly OverlayItemPresenter _overlayItemPresenter;
		private bool _isOpen;
		private OverlayContentControl _overlayLayer;

		#endregion

		#region Ctors

		public BackstageViewHost(BackstageViewControl backstageViewControl)
		{
			BackstageViewControl = backstageViewControl;

			_overlayItemPresenter = new OverlayItemPresenter
			{
				Content = backstageViewControl
			};
		}

		#endregion

		#region Properties

		public BackstageViewControl BackstageViewControl { get; }

		public bool IsOpen
		{
			get => _isOpen;
			set
			{
				if (_isOpen == value)
					return;

				_isOpen = value;

				if (_isOpen)
					Open();
				else
					Close();
			}
		}

		private OverlayContentControl OverlayLayer
		{
			set
			{
				if (ReferenceEquals(_overlayLayer, value))
					return;

				_overlayLayer?.RemoveItem(_overlayItemPresenter);

				_overlayLayer = value;

				_overlayLayer?.AddItem(_overlayItemPresenter);
			}
		}

		#endregion

		#region  Methods

		private void Close()
		{
			OverlayLayer = null;
		}

		private void Open()
		{
			OverlayLayer = BackstageViewControl.GetLogicalAncestorsAndSelf().OfType<FrameworkElement>().Select(OverlayContentControl.GetOverlay).LastOrDefault(a => a != null);
		}

		#endregion
	}
}