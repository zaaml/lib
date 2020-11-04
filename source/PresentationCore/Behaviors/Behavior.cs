// <copyright file="Behavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Behaviors
{
	public abstract class BehaviorBase : InheritanceContextObject
	{
		public FrameworkElement FrameworkElement { get; private set; }

		public bool IsAttached { get; private set; }

		internal virtual void Attach(FrameworkElement frameworkElement)
		{
			FrameworkElement = frameworkElement;
			IsAttached = true;
			OnAttached();
		}

		internal virtual void Detach()
		{
			OnDetaching();
			IsAttached = false;
			FrameworkElement = null;
		}

		protected virtual void OnAttached()
		{
		}

		protected virtual void OnDetaching()
		{
		}
	}

	public class Behavior<T> : BehaviorBase where T : FrameworkElement
	{
		public T Target { get; protected internal set; }

		internal override void Attach(FrameworkElement frameworkElement)
		{
			Target = (T) frameworkElement;

			base.Attach(frameworkElement);
		}

		internal override void Detach()
		{
			base.Detach();

			Target = null;
		}
	}

	public class BehaviorCollection : InheritanceContextDependencyObjectCollection<BehaviorBase>
	{
		private FrameworkElement _frameworkElement;

		public BehaviorCollection(FrameworkElement frameworkElement)
		{
			_frameworkElement = frameworkElement;
		}

		internal FrameworkElement FrameworkElement
		{
			get => _frameworkElement;
			set
			{
				if (ReferenceEquals(_frameworkElement, value))
					return;

				if (_frameworkElement != null)
					foreach (var behavior in this)
						behavior.Detach();

				_frameworkElement = value;

				if (_frameworkElement != null)
					foreach (var behavior in this)
						behavior.Attach(_frameworkElement);
			}
		}

		protected override void OnItemAdded(BehaviorBase behavior)
		{
			if (_frameworkElement != null)
				behavior.Attach(_frameworkElement);
		}

		protected override void OnItemRemoved(BehaviorBase behavior)
		{
			if (_frameworkElement != null)
				behavior.Detach();
		}
	}
}