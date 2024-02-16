// <copyright file="DependencyServiceBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Services
{
	internal abstract class DependencyServiceBase<T> : DependencyObject, IDependencyObjectService<T> where T : DependencyObject
	{
		public bool IsAttached => Target != null;

		public T Target { get; private set; }

		private IDependencyObjectService<T> Attach(T target)
		{
			Target = target;

			OnAttach();

			return this;
		}

		private IDependencyObjectService<T> Detach()
		{
			OnDetach();

			Target = null;

			return this;
		}

		protected virtual void OnAttach()
		{
		}

		protected virtual void OnDetach()
		{
		}

		IDependencyObjectService IDependencyObjectService.Attach(DependencyObject dependencyObject)
		{
			return Attach((T)dependencyObject);
		}

		IDependencyObjectService IDependencyObjectService.Detach(DependencyObject dependencyObject)
		{
			return Detach();
		}

		IDependencyObjectService<T> IDependencyObjectService<T>.Attach(T dependencyObject)
		{
			return Attach(dependencyObject);
		}

		IDependencyObjectService<T> IDependencyObjectService<T>.Detach(T dependencyObject)
		{
			return Detach();
		}

		bool IDependencyObjectService<T>.IsAttached => IsAttached;

		public virtual void Dispose()
		{
		}
	}
}