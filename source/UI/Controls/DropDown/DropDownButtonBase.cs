// <copyright file="DropDownButtonBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore;
using Zaaml.UI.Controls.Primitives;

namespace Zaaml.UI.Controls.DropDown
{
	public abstract partial class DropDownButtonBase
	{
		static DropDownButtonBase()
		{
#if !SILVERLIGHT
			KeyboardNavigation.AcceptsReturnProperty.OverrideMetadata(typeof(DropDownButtonBase), new FrameworkPropertyMetadata(KnownBoxes.BoolTrue));
#endif
		}

		protected DropDownButtonBase()
		{
			_buttonController = new ButtonController<DropDownButtonBase>(this);
			_dropDownPopupWrapper = new DropDownPopupWrapper(this, IsDropDownOpenProperty, PlacementProperty);

			LayoutUpdated += OnLayoutUpdated;
		}
	}
}