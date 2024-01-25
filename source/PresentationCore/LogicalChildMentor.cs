// <copyright file="LogicalChildMentor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Windows;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore
{
	internal interface ILogicalMentorOwner
	{
		void AddLogicalChild(object child);

		void RemoveLogicalChild(object child);

		IEnumerator BaseLogicalChildren { get; }
	}

	internal abstract class LogicalChildMentor
	{
		private static readonly DependencyProperty MentorProperty = DPM.RegisterAttached<LogicalChildMentor, LogicalChildMentor>
			("Mentor");

		public void AddLogicalChild(object logicalChild)
		{
			if (logicalChild == null)
				return;

			if (GetMentor(logicalChild) != null)
				return;

			var dependencyObject = logicalChild as DependencyObject;

			if (dependencyObject?.GetLogicalParent() != null)
				return;

			AddLogicalChildCore(logicalChild);

			dependencyObject?.SetValue(MentorProperty, this);
		}

		protected abstract void AddLogicalChildCore(object logicalChild);

		public static void AttachLogical(object logicalChild)
		{
			GetMentor(logicalChild)?.AddLogicalChildCore(logicalChild);
		}

		public static LogicalChildMentor<TControl> Create<TControl>(TControl control) where TControl : System.Windows.Controls.Control, ILogicalMentorOwner
		{
			return new LogicalChildMentor<TControl>(control);
		}

		public static void DetachLogical(object logicalChild)
		{
			GetMentor(logicalChild)?.RemoveLogicalChildCore(logicalChild);
		}

		private static LogicalChildMentor GetMentor(object logicalChild)
		{
			if (logicalChild is DependencyObject dependencyObject)
				return (LogicalChildMentor) dependencyObject.GetValue(MentorProperty);

			return null;
		}

		public void OnLogicalChildPropertyChanged<T>(T oldChild, T newChild) where T : class
		{
			RemoveLogicalChild(oldChild);
			AddLogicalChild(newChild);
		}

		public void RemoveLogicalChild(object logicalChild)
		{
			if (logicalChild == null)
				return;

			if (ReferenceEquals(this, GetMentor(logicalChild)) == false)
				return;

			if (logicalChild is DependencyObject dependencyObject)
				dependencyObject.ClearValue(MentorProperty);

			RemoveLogicalChildCore(logicalChild);
		}

		protected abstract void RemoveLogicalChildCore(object logicalChild);
	}

	internal sealed class LogicalChildMentor<TControl> : LogicalChildMentor where TControl : System.Windows.Controls.Control, ILogicalMentorOwner
	{
		private readonly List<object> _logicalChildren = new();

		public LogicalChildMentor(TControl control)
		{
			Control = control;
		}

		public TControl Control { get; }

		protected override void AddLogicalChildCore(object logicalChild)
		{
			_logicalChildren.Add(logicalChild);

			Control.AddLogicalChild(logicalChild);
		}

		public IEnumerator GetLogicalChildren()
		{
			return _logicalChildren.Count == 0 ? Control.BaseLogicalChildren : EnumeratorUtils.Concat(Control.BaseLogicalChildren, (IEnumerator) _logicalChildren.GetEnumerator());
		}

		protected override void RemoveLogicalChildCore(object logicalChild)
		{
			Control.RemoveLogicalChild(logicalChild);

			_logicalChildren.Remove(logicalChild);
		}
	}
}