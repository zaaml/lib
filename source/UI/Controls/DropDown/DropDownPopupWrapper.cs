// <copyright file="DropDownPopupWrapper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.DropDown
{
	internal class DropDownPopupWrapper : PopupWrapper
	{
		private readonly Control _owner;
		private readonly SnapPlacement _placement;

		public DropDownPopupWrapper(Control owner, DependencyProperty isPopupOpenProperty, DependencyProperty placement)
		{
			_owner = owner;

			_placement = new SnapPlacement
			{
				Target = owner
			};

			SyncDirection = PopupWrapperSyncDirection.SyncPopup;

			this.SetBinding(IsOpenProperty, new Binding { Path = new PropertyPath(isPopupOpenProperty), Source = _owner, Mode = BindingMode.TwoWay });

			var placementBinding = new Binding
			{
				Path = new PropertyPath(placement),
				Source = _owner,
				Mode = BindingMode.OneWay,
				Converter = SnapSideConverter.Instance
			};

			_placement.SetBinding(SnapPlacement.SnapSideProperty, placementBinding);

			SetValue(PlacementProperty, _placement);
		}

		public FrameworkElement PlacementTarget
		{
			get => _placement.Target;
			set => _placement.Target = value;
		}

		protected override void Sync()
		{
			base.Sync();

			if (Popup is PopupControlBase dropDownPopup)
				dropDownPopup.Owner = _owner;
		}
	}
}