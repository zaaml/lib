// <copyright file="DependencyObjectServiceContainer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Zaaml.PresentationCore.Services
{
	internal class DependencyObjectServiceContainer
	{
		private readonly Dictionary<Type, object> _serviceContainer = [];

		[DebuggerStepThrough]
		public object GetService(Type type)
		{
			return _serviceContainer.GetValueOrDefault(type);
		}

		[DebuggerStepThrough]
		public T GetService<T>() where T : class
		{
			return (T)_serviceContainer.GetValueOrDefault(typeof(T));
		}

		[DebuggerStepThrough]
		public object GetServiceOrCreate(Type type, Func<object> serviceFactory)
		{
			return GetService(type) ?? RegisterService(type, serviceFactory());
		}

		[DebuggerStepThrough]
		public T GetServiceOrCreate<T>(Func<T> serviceFactory) where T : class
		{
			return GetService<T>() ?? RegisterService(serviceFactory());
		}

		[DebuggerStepThrough]
		public object RegisterService(Type type, object service)
		{
			return _serviceContainer[type] = service;
		}

		[DebuggerStepThrough]
		public T RegisterService<T>(T service) where T : class
		{
			return (T)RegisterService(typeof(T), service);
		}

		[DebuggerStepThrough]
		public void RemoveService(Type type)
		{
			_serviceContainer.Remove(type);
		}

		[DebuggerStepThrough]
		public void RemoveService<T>()
		{
			RemoveService(typeof(T));
		}

		[DebuggerStepThrough]
		public void SetService(Type type, object service)
		{
			_serviceContainer[type] = service;
		}

		[DebuggerStepThrough]
		public void SetService<T>(object service)
		{
			_serviceContainer[typeof(T)] = service;
		}
	}
}