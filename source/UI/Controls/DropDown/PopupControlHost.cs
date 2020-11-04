// <copyright file="PopupControlHost.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.DropDown
{
	public sealed class PopupControlHost : FixedTemplateControl<Panel>, IClickBarrier
	{
		private PopupControlBase _dropDownControl;

		public PopupControlHost()
		{
			Focusable = false;
			IsTabStop = false;
		}

		internal PopupControlBase DropDownControl
		{
			get => _dropDownControl;
			set
			{
				if (ReferenceEquals(_dropDownControl, value))
					return;

				if (_dropDownControl != null)
					TemplateRoot?.Children.Clear();

				_dropDownControl = value;

				if (_dropDownControl != null && _dropDownControl.GetVisualParent() == null && _dropDownControl.GetLogicalParent() == null)
					TemplateRoot?.Children.Add(_dropDownControl);
			}
		}

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			if (_dropDownControl != null)
				TemplateRoot?.Children.Add(_dropDownControl);
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			_dropDownControl?.Arrange(XamlConstants.ZeroRect);

			return XamlConstants.ZeroSize;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			_dropDownControl?.Measure(XamlConstants.ZeroSize);

			return XamlConstants.ZeroSize;
		}

		protected override void UndoTemplateOverride()
		{
			TemplateRoot?.Children.Clear();

			base.UndoTemplateOverride();
		}

		bool IClickBarrier.PropagateEvents => false;
	}
}