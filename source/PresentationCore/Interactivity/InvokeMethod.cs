// <copyright file="InvokeMethod.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using Zaaml.Core;

namespace Zaaml.PresentationCore.Interactivity
{
	public class InvokeMethod : TargetTriggerActionBase
	{
		private Action _method;

		public string MethodName { get; set; }

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);
			var invokeMethodSource = (InvokeMethod)source;
			MethodName = invokeMethodSource.MethodName;
		}

		protected override InteractivityObject CreateInstance()
		{
			return new InvokeMethod();
		}

		private void EnsureMethod()
		{
			if (_method != null || MethodName == null)
				return;

			_method = DummyAction.Instance;

			try
			{
				var actualTarget = ActualTarget;
				if (actualTarget == null)
					return;

				var actualTargetType = actualTarget.GetType();

				var instanceMethodInfo = actualTargetType.GetMethod(MethodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod);
				if (instanceMethodInfo != null && instanceMethodInfo.GetParameters().Any() == false)
				{
					_method = (Action)Delegate.CreateDelegate(typeof(Action), actualTarget, MethodName);
					return;
				}

				var staticMethodInfo = actualTargetType.GetMethod(MethodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod);
				if (staticMethodInfo != null && staticMethodInfo.GetParameters().Any() == false)
					_method = (Action)Delegate.CreateDelegate(typeof(Action), actualTargetType, MethodName);
			}
			catch (Exception e)
			{
				LogService.LogError(e);
			}
		}

		protected override void InvokeCore()
		{
			EnsureMethod();

			_method?.Invoke();
		}

		protected override void OnActualTargetChanged(DependencyObject oldTarget)
		{
			base.OnActualTargetChanged(oldTarget);
			_method = null;
		}
	}
}