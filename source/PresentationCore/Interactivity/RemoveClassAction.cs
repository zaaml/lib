// <copyright file="RemoveClassAction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class RemoveClassAction : ClassTriggerActionBase
	{
		protected override InteractivityObject CreateInstance()
		{
			return new RemoveClassAction();
		}

		protected override void InvokeClassAction(DependencyObject dependencyObject, string className)
		{
			dependencyObject.RemoveClass(className);
		}
	}
}