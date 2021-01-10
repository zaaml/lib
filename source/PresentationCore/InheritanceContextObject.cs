// <copyright file="InheritanceContextObject.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore
{
	public class InheritanceContextObject : Freezable, IDependencyPropertyChangedInvocator
	{
		private IInheritanceContext _inheritanceContext;

		internal event DependencyPropertyChangedEventHandler DependencyPropertyChangedInternal;

		internal IInheritanceContext InheritanceContext
		{
			get => _inheritanceContext;
			set
			{
				if (ReferenceEquals(_inheritanceContext, value))
					return;

				if (_inheritanceContext != null)
					DetachContext(_inheritanceContext);

				_inheritanceContext = value;

				if (_inheritanceContext != null)
					AttachContext(_inheritanceContext);
			}
		}

		internal virtual void AttachContext(IInheritanceContext inheritanceContext)
		{
		}

		protected override Freezable CreateInstanceCore()
		{
			return null;
		}

		internal virtual void DetachContext(IInheritanceContext inheritanceContext)
		{
		}

		private protected virtual void InvokeDependencyPropertyChangedEvent(DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			DependencyPropertyChangedInternal?.Invoke(this, dependencyPropertyChangedEventArgs);
		}

		void IDependencyPropertyChangedInvocator.InvokeDependencyPropertyChangedEvent(DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
		}
	}
}