// <copyright file="MouseOverPopupTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore.PropertyCore.Extensions;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public sealed class MouseOverPopupTrigger : SourcePopupTrigger
	{
		protected override void AttachTarget(FrameworkElement source)
		{
			source.MouseEnter += OnSourceMouseEnter;
			source.MouseLeave += OnSourceMouseLeave;
		}

		protected override void DetachTarget(FrameworkElement source)
		{
			source.MouseEnter -= OnSourceMouseEnter;
			source.MouseLeave -= OnSourceMouseLeave;
		}

		protected override void OnPopupChanged(Popup oldValue, Popup newValue)
		{
			base.OnPopupChanged(oldValue, newValue);

			if (oldValue != null)
			{
				oldValue.Panel.MouseEnter -= OnPopupPanelMouseEnter;
				oldValue.Panel.MouseLeave -= OnPopupPanelMouseLeave;
			}

			if (newValue != null)
			{
				newValue.Panel.MouseEnter += OnPopupPanelMouseEnter;
				newValue.Panel.MouseLeave += OnPopupPanelMouseLeave;
			}
		}

		private void OnPopupPanelMouseEnter(object sender, MouseEventArgs e)
		{
			if (this.HasLocalValue(SourceProperty) == false)
				Open();
		}

		private void OnPopupPanelMouseLeave(object sender, MouseEventArgs e)
		{
			if (this.HasLocalValue(SourceProperty) == false)
				Close();
		}

		private void OnSourceMouseEnter(object sender, MouseEventArgs e)
		{
			Open();
		}

		private void OnSourceMouseLeave(object sender, MouseEventArgs e)
		{
			Close();
		}
	}
}