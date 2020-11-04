// <copyright file="TypeDescriptorContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore
{
	internal class TypeDescriptorContext : ITypeDescriptorContext, IDisposable
	{
		#region Static Fields and Constants

		private static readonly Stack<TypeDescriptorContext> TypeDescriptorContextPool = new Stack<TypeDescriptorContext>(4);

		#endregion

		#region Fields

		private bool _isBusy;

		private IServiceProvider _serviceProvider;

		#endregion

		#region Ctors

		private TypeDescriptorContext()
		{
		}

		#endregion

		#region  Methods

		public static TypeDescriptorContext FromServiceProvider(IServiceProvider serviceProvider)
		{
			var typeDescriptorContext = TypeDescriptorContextPool.Count > 0 ? TypeDescriptorContextPool.Pop() : new TypeDescriptorContext();
			return typeDescriptorContext.Mount(serviceProvider);
		}

		private TypeDescriptorContext Mount(IServiceProvider serviceProvider)
		{
			if (_isBusy)
				throw new InvalidOperationException();

			_serviceProvider = serviceProvider;

			_isBusy = true;
			return this;
		}

		#endregion

		#region Interface Implementations

		#region IDisposable

		void IDisposable.Dispose()
		{
			_serviceProvider = null;

			if (_isBusy)
				TypeDescriptorContextPool.Push(this);
			else
				return;

			_isBusy = false;
		}

		#endregion

		#region IServiceProvider

		public object GetService(Type serviceType)
		{
			return _serviceProvider?.GetService(serviceType);
		}

		#endregion

		#region ITypeDescriptorContext

		public bool OnComponentChanging()
		{
			return false;
		}

		public void OnComponentChanged()
		{
		}

		public IContainer Container => null;

		public object Instance => _serviceProvider?.GetTargetObject();

		public PropertyDescriptor PropertyDescriptor => null;

		#endregion

		#endregion
	}
}
