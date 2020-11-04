using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.Core.Monads;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Services;

namespace Zaaml.PresentationCore.Extensions
{
	public static class DependencyObjectExtensions
	{
		#region Static Fields

		private static readonly DependencyProperty ContainerProperty = DPM.RegisterAttached<DependencyObjectServiceContainer>
			("Container", typeof (DependencyObjectExtensions));

		#endregion

		#region Methods

		public static void AddValueChanged(this DependencyObject depObj, DependencyProperty depProp, EventHandler<PropertyValueChangedEventArgs> handler)
		{
			depObj.GetServiceOrCreate(() => new PropertyChangeService()).AddValueChanged(depProp, handler);
		}

		public static void AddValueChanged(this DependencyObject depObj, string propertyName, EventHandler<PropertyValueChangedEventArgs> handler)
		{
			depObj.GetServiceOrCreate(() => new PropertyChangeService()).AddValueChanged(propertyName, handler);
		}

	  public static void SetReadOnlyValue(this DependencyObject depObj, DependencyPropertyKey propertyKey, object value)
	  {
	    depObj.SetValue(propertyKey, value);
	  }

	  internal static void SetCurrentValueInternal(this DependencyObject depObj, DependencyProperty property, object value)
	  {
#if SILVERLIGHT
	    depObj.SetValue(property, value);
#else
      depObj.SetCurrentValue(property, value);
#endif
    }

    public static object GetReadOnlyValue(this DependencyObject depObj, DependencyPropertyKey propertyKey)
	  {
	    return depObj.GetValue(propertyKey.DependencyProperty);
	  }

    public static T GetReadOnlyValue<T>(this DependencyObject depObj, DependencyPropertyKey propertyKey)
    {
      return (T)depObj.GetValue(propertyKey.DependencyProperty);
    }

    [DebuggerStepThrough]
		internal static void ClearValue<T>(this DependencyObject dependencyObject, DependencyProperty property, Action<T> cleanUpAction)
		{
			cleanUpAction(dependencyObject.GetValue<T>(property));
			dependencyObject.ClearValue(property);
		}

		[DebuggerStepThrough]
		internal static DependencyObjectServiceContainer GetContainer(this DependencyObject dependencyObject)
		{
			return dependencyObject.GetValueOrCreate(ContainerProperty, () => new DependencyObjectServiceContainer());
		}

    [DebuggerStepThrough]
    public static object ReadLocalExpandoValue(this DependencyObject depObj, string propertyName)
    {
      return depObj.ReadLocalValue(DependencyPropertyManager.GetExpandoProperty(propertyName));
    }

    [DebuggerStepThrough]
		public static object GetExpandoValue(this DependencyObject depObj, string propertyName)
		{
			return depObj.GetValue(DependencyPropertyManager.GetExpandoProperty(propertyName));
		}

		[DebuggerStepThrough]
		internal static TService GetService<TService>(this DependencyObject dependencyObject)
			where TService : class, IDependencyObjectService
		{
			return dependencyObject.GetContainer().GetService<TService>();
		}


		[DebuggerStepThrough]
		internal static TService GetServiceOrCreate<TService, T>(this T dependencyObject, Func<TService> factory)
			where TService : class, IDependencyObjectService<T> where T : DependencyObject
		{
			var serviceContainer = dependencyObject.GetContainer();
			var service = serviceContainer.GetService<TService>();

			if (service != null) 
				return service;

			service = factory();
			serviceContainer.RegisterService(service);
			service.Attach(dependencyObject);

			return service;
		}

		[DebuggerStepThrough]
		internal static TService GetServiceOrCreate<TService, T>(this T dependencyObject, Func<T, TService> factory)
			where TService : class, IDependencyObjectService<T> where T : DependencyObject
		{
			var serviceContainer = dependencyObject.GetContainer();
			var service = serviceContainer.GetService<TService>();

			if (service != null) 
				return service;

			service = factory(dependencyObject);
			serviceContainer.RegisterService(service);
			service.Attach(dependencyObject);

			return service;
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
			var serviceContainer = dependencyObject.GetContainer();
			var service = serviceContainer.GetService<TService>();

			if (service != null) 
				return service;

			service = factory();
			serviceContainer.RegisterService(service);
			service.Attach(dependencyObject);

			return service;
		}




	  [DebuggerStepThrough]
	  internal static TService GetServiceOrCreateOrDefault<TService, T>(this T dependencyObject, bool create, Func<TService> factory)
	    where TService : class, IDependencyObjectService<T> where T : DependencyObject
	  {
	    var serviceContainer = dependencyObject.GetContainer();
			var service = serviceContainer.GetService<TService>();

	    if (service != null || create == false) 
		    return service;

	    service = factory();
	    serviceContainer.RegisterService(service);
	    service.Attach(dependencyObject);

	    return service;
	  }

	  [DebuggerStepThrough]
	  internal static TService GetServiceOrCreateOrDefault<TService, T>(this T dependencyObject, bool create, Func<T, TService> factory)
	    where TService : class, IDependencyObjectService<T> where T : DependencyObject
	  {
	    var serviceContainer = dependencyObject.GetContainer();
			var service = serviceContainer.GetService<TService>();

	    if (service != null || create == false)
		    return service;

	    service = factory(dependencyObject);
	    serviceContainer.RegisterService(service);
	    service.Attach(dependencyObject);

	    return service;
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
	  internal static TService GetServiceOrCreateOrDefault<TService>(this DependencyObject dependencyObject, bool create, Func<TService> factory)
	    where TService : class, IDependencyObjectService<DependencyObject>
	  {
	    var serviceContainer = dependencyObject.GetContainer();
			var service = serviceContainer.GetService<TService>();

	    if (service != null || create == false)
		    return service;

	    service = factory();
	    serviceContainer.RegisterService(service);
	    service.Attach(dependencyObject);

	    return service;
	  }

		[DebuggerStepThrough]
		public static T GetValue<T>(this DependencyObject dependencyObject, DependencyProperty property)
		{
			return (T) dependencyObject.GetValue(property);
		}

		[DebuggerStepThrough]
    public static T GetValue<T>(this DependencyObject dependencyObject, DependencyPropertyKey propertyKey)
    {
      return (T)dependencyObject.GetValue(propertyKey.DependencyProperty);
    }

		[DebuggerStepThrough]
		public static T GetValueOrCreate<T>(this DependencyObject dependencyObject, DependencyProperty dependencyProperty,
			Func<T> factoryMethod) where T : class
		{
			var value = (T) dependencyObject.GetValue(dependencyProperty);

			if (value == null)
				dependencyObject.SetValue(dependencyProperty, value = factoryMethod());

			return value;
		}

	  [DebuggerStepThrough]
    public static T GetValueOrCreateOrDefault<T>(this DependencyObject dependencyObject, DependencyProperty dependencyProperty, bool create, Func<T> factoryMethod)
	  {
	    var value = (T)dependencyObject.GetValue(dependencyProperty);

	    if (value == null && create)
	      dependencyObject.SetValue(dependencyProperty, value = factoryMethod());

	    return value;
    }

	  [DebuggerStepThrough]
	  public static T GetValueOrCreateOrDefault<T>(this DependencyObject dependencyObject, DependencyPropertyKey dependencyPropertyKey, bool create, Func<T> factoryMethod)
	  {
	    var value = (T)dependencyObject.GetValue(dependencyPropertyKey.DependencyProperty);

	    if (value == null && create)
	      dependencyObject.SetReadOnlyValue(dependencyPropertyKey, value = factoryMethod());

	    return value;
	  }

    [DebuggerStepThrough]
		public static T GetValueOrCreate<T>(this DependencyObject dependencyObject, DependencyPropertyKey dependencyPropertyKey,
			Func<T> factoryMethod) where T : class
		{
			var value = (T) dependencyObject.GetValue(dependencyPropertyKey.DependencyProperty);

			if (value == null)
				dependencyObject.SetReadOnlyValue(dependencyPropertyKey, value = factoryMethod());

			return value;
		}

		[DebuggerStepThrough]
    internal static void RemoveService<TService>(this DependencyObject dependencyObject)
			where TService : class, IDependencyObjectService
		{
			var service = dependencyObject.GetService<TService>();

			if (service == null)
				return;

			service.Detach(dependencyObject);
			CleanUpService(service);

			dependencyObject.GetContainer().RemoveService<TService>();
		}

		[DebuggerStepThrough]
    internal static void RemoveService<TService, T>(this T dependencyObject)
			where TService : class, IDependencyObjectService<T> where T : DependencyObject
		{
			var service = dependencyObject.GetService<TService>();

			if (service == null) 
				return;

			service.Detach(dependencyObject);
			CleanUpService(service);
			dependencyObject.GetContainer().RemoveService<TService>();
		}

		[DebuggerStepThrough]
    internal static void SetService<TService>(this DependencyObject dependencyObject, TService service)
      where TService : class, IDependencyObjectService
    {
      var localService = dependencyObject.GetService<TService>();

      if (localService != null)
      {
        service.Detach(dependencyObject);
        CleanUpService(localService);
      }

      dependencyObject.GetContainer().SetService<TService>(service);
      service.Attach(dependencyObject);
    }

		[DebuggerStepThrough]
		public static void RemoveValueChanged(this DependencyObject depObj, DependencyProperty depProp, EventHandler<PropertyValueChangedEventArgs> handler)
		{
			depObj.GetService<PropertyChangeService>().Do(s => s.RemoveValueChanged(depProp, handler));
		}

		[DebuggerStepThrough]
		public static void RemoveValueChanged(this DependencyObject depObj, string propertyName, EventHandler<PropertyValueChangedEventArgs> handler)
		{
			depObj.GetService<PropertyChangeService>().Do(s => s.RemoveValueChanged(propertyName, handler));
		}

		public static Binding ReadLocalBinding(this DependencyObject target, DependencyProperty dependencyProperty)
		{
			var localBindingExpression = target.ReadLocalValue(dependencyProperty) as BindingExpression;

			return localBindingExpression?.ParentBinding;
		}
		
		public static BindingExpression ReadLocalBindingExpression(this DependencyObject target, DependencyProperty dependencyProperty)
		{
			return target.ReadLocalValue(dependencyProperty) as BindingExpression;
		}

		public static void RestoreLocalValue(this DependencyObject target, DependencyProperty dependencyProperty, object localValue)
		{
			if (localValue == DependencyProperty.UnsetValue)
				target.ClearValue(dependencyProperty);
			else if (localValue is BindingExpression localValueBindingExpression)
				target.SetBinding(dependencyProperty, localValueBindingExpression.ParentBinding);
			else
				target.SetValue(dependencyProperty, localValue);
		}

		[DebuggerStepThrough]
		public static void SetBinding(this DependencyObject depObj, DependencyProperty depProp, Binding binding)
		{
			BindingOperations.SetBinding(depObj, depProp, binding);
		}

    [DebuggerStepThrough]
    public static void SetExpandoBinding(this DependencyObject depObj, string propertyName, Binding binding)
    {
      BindingOperations.SetBinding(depObj, DependencyPropertyManager.GetExpandoProperty(propertyName), binding);
    }

    [DebuggerStepThrough]
    public static void ClearExpandoValue(this DependencyObject depObj, string propertyName, object value)
	  {
	    ClearExpandoValue(depObj, propertyName, value, false);
	  }


    [DebuggerStepThrough]
    public static void ClearExpandoValue(this DependencyObject depObj, string propertyName, object value, bool skipPropertyChangedHandler)
	  {
      var prop = DependencyPropertyManager.GetExpandoProperty(propertyName);

      if (skipPropertyChangedHandler == false)
      {
        depObj.ClearValue(prop);

        return;
      }

      try
      {
        depObj.SuspendPropertyChangedCallback(prop);

				depObj.ClearValue(prop);
      }
      finally
      {
        depObj.ResumePropertyChangedCallback(prop);
      }
    }

		[DebuggerStepThrough]
		public static void SetExpandoValue(this DependencyObject depObj, string propertyName, object value, bool skipPropertyChangedHandler)
		{
			if (skipPropertyChangedHandler == false)
			{
				SetExpandoValue(depObj, propertyName, value);

				return;
			}

			var prop = DependencyPropertyManager.GetExpandoProperty(propertyName);

			try
			{
				depObj.SuspendPropertyChangedCallback(prop);

				SetValue(depObj, prop, value);
			}
			finally 
			{
				depObj.ResumePropertyChangedCallback(prop);
			}
		}

		[DebuggerStepThrough]
		public static void SetValue(this DependencyObject dependencyObject, DependencyProperty property, object value, bool skipPropertyChangedHandler)
		{
			if (skipPropertyChangedHandler == false)
			{
				SetValue(dependencyObject, property, value);

				return;
			}

			try
			{
				dependencyObject.SuspendPropertyChangedCallback(property);

				SetValue(dependencyObject, property, value);
			}
			finally
			{
				dependencyObject.ResumePropertyChangedCallback(property);
			}
		}

		[DebuggerStepThrough]
		public static void SetValue<T>(this DependencyObject dependencyObject, DependencyProperty property, T value, bool skipPropertyChangedHandler)
		{
			if (skipPropertyChangedHandler == false)
			{
				SetValue(dependencyObject, property, value);

				return;
			}

			try
			{
				dependencyObject.SuspendPropertyChangedCallback(property);

				SetValue(dependencyObject, property, value);
			}
			finally
			{
				dependencyObject.ResumePropertyChangedCallback(property);
			}
		}

		[DebuggerStepThrough]
		public static void SetValue<T>(this DependencyObject dependencyObject, DependencyPropertyKey key, T value, bool skipPropertyChangedHandler)
		{
			if (skipPropertyChangedHandler == false)
			{
				SetValue(dependencyObject, key, value);

				return;
			}

			try
			{
				dependencyObject.SuspendPropertyChangedCallback(key.DependencyProperty);

				SetValue(dependencyObject, key, value);
			}
			finally
			{
				dependencyObject.ResumePropertyChangedCallback(key.DependencyProperty);
			}
		}

		[DebuggerStepThrough]
		public static void SetValue(this DependencyObject dependencyObject, DependencyPropertyKey key, object value, bool skipPropertyChangedHandler)
		{
			if (skipPropertyChangedHandler == false)
			{
				SetValue(dependencyObject, key, value);

				return;
			}

			try
			{
				dependencyObject.SuspendPropertyChangedCallback(key.DependencyProperty);

				SetValue(dependencyObject, key, value);
			}
			finally
			{
				dependencyObject.ResumePropertyChangedCallback(key.DependencyProperty);
			}
		}

		[DebuggerStepThrough]
		public static void ClearValue(this DependencyObject dependencyObject, DependencyPropertyKey key, bool skipPropertyChangedHandler)
		{
			if (skipPropertyChangedHandler == false)
			{
				ClearValue(dependencyObject, key);

				return;
			}

			try
			{
				dependencyObject.SuspendPropertyChangedCallback(key.DependencyProperty);

				ClearValue(dependencyObject, key);
			}
			finally
			{
				dependencyObject.ResumePropertyChangedCallback(key.DependencyProperty);
			}
		}

		[DebuggerStepThrough]
		public static void SetExpandoValue(this DependencyObject depObj, string propertyName, object value)
		{
			depObj.SetValue(DependencyPropertyManager.GetExpandoProperty(propertyName), value);
		}

		[DebuggerStepThrough]
		public static void SetValue<T>(this DependencyObject dependencyObject, DependencyProperty property, T value)
		{
			dependencyObject.SetValue(property, value);
		}

		[DebuggerStepThrough]
    public static void SetValue<T>(this DependencyObject dependencyObject, DependencyPropertyKey key, T value)
    {
      SetValue(dependencyObject, key, (object)value);
    }

		[DebuggerStepThrough]
    public static void SetValue(this DependencyObject dependencyObject, DependencyPropertyKey key, object value)
    {
#if SILVERLIGHT
      key.IsLocked = false;
      dependencyObject.SetValue(key.DependencyProperty, value);
      key.IsLocked = true;
#else
      dependencyObject.SetValue(key, value);
#endif
    }

		[DebuggerStepThrough]
    public static void ClearValue(this DependencyObject dependencyObject, DependencyPropertyKey key)
    {
#if SILVERLIGHT
      key.IsLocked = false;
      dependencyObject.ClearValue(key.DependencyProperty);
      key.IsLocked = true;
#else
      dependencyObject.ClearValue(key);
#endif
    }

		[DebuggerStepThrough]
		internal static void SetValue<T>(this DependencyObject dependencyObject, DependencyProperty property, T value, Action<T> cleanUpAction)
		{
			cleanUpAction(dependencyObject.GetValue<T>(property));
			dependencyObject.SetValue(property, value);
		}

		internal static RestoreValueHelper SetDisposableValue(this DependencyObject depObj, DependencyProperty dependencyProperty, object value)
		{
			var restoreValueHelper = new RestoreValueHelper(depObj, dependencyProperty);
			var binding = value is BindingExpression bindingExpression ? bindingExpression.ParentBinding : value as Binding;

			if (binding != null)
				depObj.SetBinding(dependencyProperty, binding);
			else
				depObj.SetValue(dependencyProperty, value);

			return restoreValueHelper;
		}

		[DebuggerStepThrough]
		private static void CleanUpService(IDependencyObjectService service)
		{
			service.Dispose();
		}

#endregion
	}

	internal class RestoreValueHelper : IDisposable
	{
#region Fields

		private readonly DependencyProperty _property;
		private readonly WeakReference _target;
		private bool _isDisposed;

#endregion

#region Ctors

		public RestoreValueHelper(DependencyObject target, DependencyProperty property) :
			this(target, property, target.ReadLocalValue(property))
		{
		}

		public RestoreValueHelper(DependencyObject target, DependencyProperty property, object value)
		{
			_property = property;
			_target = new WeakReference(target);
			Value = value;
		}

#endregion

#region Properties

		public object Value { get; set; }

#endregion

#region IDisposable Members

		public void Dispose()
		{
			if (_isDisposed)
				return;

			_isDisposed = true;

		  _target.GetTarget<DependencyObject>()?.RestoreLocalValue(_property, Value);
		}

#endregion
	}
}