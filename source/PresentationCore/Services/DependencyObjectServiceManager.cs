// <copyright file="DependencyObjectServiceManager.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Services
{
	internal static class DependencyObjectServiceManager
	{
		private static readonly DependencyProperty ContainerProperty = DPM.RegisterAttached<DependencyObjectServiceContainer>
			("Container", typeof(DependencyObjectExtensions));

		[DebuggerStepThrough]
		private static void CleanUpService(IDependencyObjectService service)
		{
			service.Dispose();
		}

		[DebuggerStepThrough]
		private static DependencyObjectServiceContainer GetContainer(DependencyObject dependencyObject)
		{
			return dependencyObject.GetValueOrCreate(ContainerProperty, () => new DependencyObjectServiceContainer());
		}

		public static TService GetService<TService>(DependencyObject dependencyObject) where TService : class, IDependencyObjectService
		{
			return GetContainer(dependencyObject).GetService<TService>();
		}

		[DebuggerStepThrough]
		internal static TService GetServiceOrCreate<TService>(DependencyObject dependencyObject, Func<TService> factory)
			where TService : class, IDependencyObjectService<DependencyObject>
		{
			var serviceContainer = GetContainer(dependencyObject);
			var service = serviceContainer.GetService<TService>();

			if (service != null)
				return service;

			service = factory();
			serviceContainer.RegisterService(service);
			service.Attach(dependencyObject);

			return service;
		}

		public static TService GetServiceOrCreate<TService, T>(T dependencyObject, Func<TService> factory) where TService : class, IDependencyObjectService<T> where T : DependencyObject
		{
			var serviceContainer = GetContainer(dependencyObject);
			var service = serviceContainer.GetService<TService>();

			if (service != null)
				return service;

			service = factory();
			serviceContainer.RegisterService(service);
			service.Attach(dependencyObject);

			return service;
		}

		[DebuggerStepThrough]
		public static TService GetServiceOrCreate<TService, T>(T dependencyObject, Func<T, TService> factory)
			where TService : class, IDependencyObjectService<T> where T : DependencyObject
		{
			var serviceContainer = GetContainer(dependencyObject);
			var service = serviceContainer.GetService<TService>();

			if (service != null)
				return service;

			service = factory(dependencyObject);
			serviceContainer.RegisterService(service);
			service.Attach(dependencyObject);

			return service;
		}

		[DebuggerStepThrough]
		public static TService GetServiceOrCreateOrDefault<TService, T>(T dependencyObject, bool create, Func<TService> factory)
			where TService : class, IDependencyObjectService<T> where T : DependencyObject
		{
			var serviceContainer = GetContainer(dependencyObject);
			var service = serviceContainer.GetService<TService>();

			if (service != null || create == false)
				return service;

			service = factory();
			serviceContainer.RegisterService(service);
			service.Attach(dependencyObject);

			return service;
		}

		[DebuggerStepThrough]
		public static TService GetServiceOrCreateOrDefault<TService, T>(T dependencyObject, bool create, Func<T, TService> factory)
			where TService : class, IDependencyObjectService<T> where T : DependencyObject
		{
			var serviceContainer = GetContainer(dependencyObject);
			var service = serviceContainer.GetService<TService>();

			if (service != null || create == false)
				return service;

			service = factory(dependencyObject);
			serviceContainer.RegisterService(service);
			service.Attach(dependencyObject);

			return service;
		}

		[DebuggerStepThrough]
		public static void RemoveService<TService>(DependencyObject dependencyObject)
			where TService : class, IDependencyObjectService
		{
			var service = dependencyObject.GetService<TService>();

			if (service == null)
				return;

			service.Detach(dependencyObject);
			CleanUpService(service);

			GetContainer(dependencyObject).RemoveService<TService>();
		}

		[DebuggerStepThrough]
		public static void RemoveService<TService, T>(T dependencyObject)
			where TService : class, IDependencyObjectService<T> where T : DependencyObject
		{
			var service = dependencyObject.GetService<TService>();

			if (service == null)
				return;

			service.Detach(dependencyObject);
			CleanUpService(service);

			GetContainer(dependencyObject).RemoveService<TService>();
		}

		[DebuggerStepThrough]
		public static void SetService<TService>(DependencyObject dependencyObject, TService service)
			where TService : class, IDependencyObjectService
		{
			var localService = dependencyObject.GetService<TService>();

			if (localService != null)
			{
				service.Detach(dependencyObject);
				CleanUpService(localService);
			}

			GetContainer(dependencyObject).SetService<TService>(service);
			service.Attach(dependencyObject);
		}
	}
}