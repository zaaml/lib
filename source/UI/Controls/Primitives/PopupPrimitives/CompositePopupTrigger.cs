// <copyright file="CompositePopupTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	[ContentProperty(nameof(Triggers))]
	public sealed class CompositePopupTrigger : PopupTrigger
	{
		private static readonly DependencyPropertyKey TriggersPropertyKey = DPM.RegisterReadOnly<PopupTriggerCollection, CompositePopupTrigger>
			("TriggersInternal");

		public static readonly DependencyProperty TriggersProperty = TriggersPropertyKey.DependencyProperty;

		public PopupTriggerCollection Triggers => this.GetValueOrCreate(TriggersPropertyKey, () => new PopupTriggerCollection(this));

		protected override void OnPopupChanged(Popup oldValue, Popup newValue)
		{
			base.OnPopupChanged(oldValue, newValue);

			foreach (var trigger in Triggers)
				trigger.Popup = newValue;
		}

		internal void UpdateIsOpen()
		{
			if (Triggers.Any(t => t.ActualIsOpen))
				Open();
			else
				Close();
		}
	}
}