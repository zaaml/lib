// <copyright file="SetBinding.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class SetBinding : PropertyActionBase
	{
		public Binding Binding { get; set; }

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var setPropertyBindingSource = (SetBinding)source;

			Binding = setPropertyBindingSource.Binding;
		}

		protected override InteractivityObject CreateInstance()
		{
			return new SetBinding();
		}

		protected override void InvokeCore()
		{
			var actualTarget = ActualTarget;
			var actualProperty = ActualProperty;
			var actualBinding = Binding;

			if (actualTarget == null || actualProperty == null || actualBinding == null)
				return;

			if (Binding != null)
				actualTarget.SetBinding(actualProperty, Binding);
		}
	}
}