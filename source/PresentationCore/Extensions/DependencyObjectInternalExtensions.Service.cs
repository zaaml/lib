// <copyright file="DependencyObjectInternalExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Windows;
using Zaaml.PresentationCore.Services;

namespace Zaaml.PresentationCore.Extensions
{
	internal static partial class DependencyObjectInternalExtensions
	{
		[DebuggerStepThrough]
		internal static TService GetService<TService>(this DependencyObject dependencyObject)
			where TService : class, IDependencyObjectService
		{
			return DependencyObjectServiceManager.GetService<TService>(dependencyObject);
		}

		[DebuggerStepThrough]
		internal static TService GetServiceOrCreate<TService, T>(this T dependencyObject, Func<TService> factory)
			where TService : class, IDependencyObjectService<T>
			where T : DependencyObject
		{
			return DependencyObjectServiceManager.GetServiceOrCreate(dependencyObject, factory);
		}

		[DebuggerStepThrough]
		internal static TService GetServiceOrCreate<TService, T>(this T dependencyObject, Func<T, TService> factory)
			where TService : class, IDependencyObjectService<T> where T : DependencyObject
		{
			return DependencyObjectServiceManager.GetServiceOrCreate(dependencyObject, factory);
		}

		[DebuggerStepThrough]
		internal static TService GetServiceOrCreate<TService>(this DependencyObject dependencyObject)
			where TService : class, IDependencyObjectService<DependencyObject>, new()
		{
			return dependencyObject.GetServiceOrCreate(() => new TService());
		}

		[DebuggerStepThrough]
		internal static TService GetServiceOrCreate<TService, T>(this T dependencyObject)
			where TService : class, IDependencyObjectService<T>, new() where T : DependencyObject
		{
			return dependencyObject.GetServiceOrCreate(() => new TService());
		}

		[DebuggerStepThrough]
		internal static TService GetServiceOrCreate<TService>(this DependencyObject dependencyObject, Func<TService> factory)
			where TService : class, IDependencyObjectService<DependencyObject>
		{
			return DependencyObjectServiceManager.GetServiceOrCreate(dependencyObject, factory);
		}

		[DebuggerStepThrough]
		internal static TService GetServiceOrCreateOrDefault<TService, T>(this T dependencyObject, bool create, Func<TService> factory)
			where TService : class, IDependencyObjectService<T> where T : DependencyObject
		{
			return DependencyObjectServiceManager.GetServiceOrCreateOrDefault(dependencyObject, create, factory);
		}

		[DebuggerStepThrough]
		internal static TService GetServiceOrCreateOrDefault<TService, T>(this T dependencyObject, bool create, Func<T, TService> factory)
			where TService : class, IDependencyObjectService<T> where T : DependencyObject
		{
			return DependencyObjectServiceManager.GetServiceOrCreateOrDefault(dependencyObject, create, factory);
		}

		[DebuggerStepThrough]
		internal static TService GetServiceOrCreateOrDefault<TService>(this DependencyObject dependencyObject, bool create)
			where TService : class, IDependencyObjectService<DependencyObject>, new()
		{
			return dependencyObject.GetServiceOrCreateOrDefault(create, () => new TService());
		}

		[DebuggerStepThrough]
		internal static TService GetServiceOrCreateOrDefault<TService, T>(this T dependencyObject, bool create)
			where TService : class, IDependencyObjectService<T>, new() where T : DependencyObject
		{
			return dependencyObject.GetServiceOrCreateOrDefault(create, () => new TService());
		}

		[DebuggerStepThrough]
		internal static void RemoveService<TService>(this DependencyObject dependencyObject)
			where TService : class, IDependencyObjectService
		{
			DependencyObjectServiceManager.RemoveService<TService>(dependencyObject);
		}

		[DebuggerStepThrough]
		internal static void RemoveService<TService, T>(this T dependencyObject)
			where TService : class, IDependencyObjectService<T> where T : DependencyObject
		{
			DependencyObjectServiceManager.RemoveService<TService, T>(dependencyObject);
		}

		[DebuggerStepThrough]
		internal static void SetService<TService>(this DependencyObject dependencyObject, TService service)
			where TService : class, IDependencyObjectService
		{
			DependencyObjectServiceManager.SetService(dependencyObject, service);
		}
	}
}