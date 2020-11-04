// <copyright file="DropDownSelectorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;
using Control = System.Windows.Controls.Control;

namespace Zaaml.UI.Controls.DropDown
{
	public abstract partial class DropDownSelectorBase<TItemsControl, TItem> : DropDownItemsControl<TItemsControl, TItem>
		where TItemsControl : Control
		where TItem : Control, ISelectable
	{
		private static readonly DependencyProperty SelectedIconProperty = DPM.Register<IconBase, DropDownSelectorBase<TItemsControl, TItem>>
			("SelectedIcon", d => d.OnSelectedIconChanged);

		private static readonly DependencyProperty SelectedContentProperty = DPM.Register<object, DropDownSelectorBase<TItemsControl, TItem>>
			("SelectedContent", d => d.OnSelectedContentChanged);

		internal bool IsSelectionHandling { get; set; }

		private object SelectedContent => GetValue(SelectedContentProperty);

		private IconBase SelectedIcon => (IconBase) GetValue(SelectedIconProperty);

		private protected override void OnItemsControlChanged(TItemsControl oldControl, TItemsControl newControl)
		{
			base.OnItemsControlChanged(oldControl, newControl);

			OnSelectorControllerChanged(GetSelectorController(oldControl), GetSelectorController(newControl));
			OnFocusNavigatorChanged(GetFocusNavigator(oldControl), GetFocusNavigator(newControl));
		}

		private void OnSelectedContentChanged()
		{
			UpdateContent();
		}

		private void OnSelectedIconChanged()
		{
			UpdateIcon();
		}

		private void UpdateContent()
		{
			ActualContent = SelectedContent;
		}

		private void UpdateIcon()
		{
			ActualIcon = SelectedIcon;
		}
	}
}