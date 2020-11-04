// <copyright file="DropDownEditorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Editors.Core;

namespace Zaaml.UI.Controls.Editors.DropDown
{
	[TemplateContractType(typeof(DropDownEditorBaseTemplateContract))]
	public abstract class DropDownEditorBase : EditorBase
	{
		private static readonly DependencyPropertyKey ActualShowDropDownButtonPropertyKey = DPM.RegisterReadOnly<bool, DropDownEditorBase>
			("ActualShowDropDownButton", false);

		public static readonly DependencyProperty ActualShowDropDownButtonProperty = ActualShowDropDownButtonPropertyKey.DependencyProperty;

		public bool ActualShowDropDownButton
		{
			get => (bool) GetValue(ActualShowDropDownButtonProperty);
			private set => this.SetReadOnlyValue(ActualShowDropDownButtonPropertyKey, value);
		}

		protected override void OnIsEditingChanged()
		{
			base.OnIsEditingChanged();

			UpdateActualShowDropDownButton();
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);

			UpdateActualShowDropDownButton();
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);

			UpdateActualShowDropDownButton();
		}

		private void UpdateActualShowDropDownButton()
		{
			ActualShowDropDownButton = IsFocused || IsMouseOver || IsEditing;
		}
	}

	public abstract class DropDownEditorBaseTemplateContract : EditorBaseTemplateContract
	{
	}
}