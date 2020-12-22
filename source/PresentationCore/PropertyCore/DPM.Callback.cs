// <copyright file="DPM.Callback.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.PresentationCore.PropertyCore
{
	public static partial class DPM
	{
		#region  Methods

		public static PropertyChangedCallback Callback<TTarget, TProperty>(Func<TTarget, Action<TProperty, TProperty>> handlerFactory, bool suspendable = false)
			where TTarget : DependencyObject
		{
			if (handlerFactory == null) 
				return null;

			PropertyChangedCallback callback = (d, e) =>
			{
				var target = d.CastTarget<TTarget>();
				var handler = handlerFactory(target);
				
				handler(e.OldValue.CastProperty<TProperty>(), e.NewValue.CastProperty<TProperty>());
			};

			return callback.WrapSuspendableCallback(suspendable);
		}

		public static PropertyChangedCallback Callback<TTarget, TProperty>(Func<TTarget, Action<TProperty>> handlerFactory, bool suspendable = false)
			where TTarget : DependencyObject
		{
			if (handlerFactory == null) 
				return null;

			PropertyChangedCallback callback = (d, e) =>
			{
				var target = d.CastTarget<TTarget>();
				var handler = handlerFactory(target);
				
				handler(e.OldValue.CastProperty<TProperty>());
			};

			return callback.WrapSuspendableCallback(suspendable);
		}

		public static PropertyChangedCallback Callback<TTarget>(Func<TTarget, Action> handlerFactory, bool suspendable = false) where TTarget : DependencyObject
		{
			if (handlerFactory == null) 
				return null;

			PropertyChangedCallback callback = (d, e) =>
			{
				var target = d.CastTarget<TTarget>();
				var handler = handlerFactory(target);
				
				handler();
			};

			return callback.WrapSuspendableCallback(suspendable);
		}

		public static PropertyChangedCallback Callback<TTarget>(Func<TTarget, Action<DependencyPropertyChangedEventArgs>> handlerFactory, bool suspendable = false) where TTarget : DependencyObject
		{
			if (handlerFactory == null) 
				return null;

			PropertyChangedCallback callback = (d, e) =>
			{
				var target = d.CastTarget<TTarget>();
				var handler = handlerFactory(target);
				
				handler(e);
			};

			return callback.WrapSuspendableCallback(suspendable);
		}
		
		public static PropertyChangedCallback Callback<TTarget, TProperty>(Func<TTarget, Action<DependencyProperty, TProperty, TProperty>> handlerFactory, bool suspendable = false)
			where TTarget : DependencyObject
		{
			if (handlerFactory == null) 
				return null;

			PropertyChangedCallback callback = (d, e) =>
			{
				var target = d.CastTarget<TTarget>();
				var handler = handlerFactory(target);
				
				handler(e.Property, e.OldValue.CastProperty<TProperty>(), e.NewValue.CastProperty<TProperty>());
			};

			return callback.WrapSuspendableCallback(suspendable);
		}

		public static PropertyChangedCallback Callback<TTarget, TProperty>(Func<TTarget, Action<DependencyProperty, TProperty>> handlerFactory, bool suspendable = false)
			where TTarget : DependencyObject
		{
			if (handlerFactory == null) 
				return null;

			PropertyChangedCallback callback = (d, e) =>
			{
				var target = d.CastTarget<TTarget>();
				var handler = handlerFactory(target);
				
				handler(e.Property, e.OldValue.CastProperty<TProperty>());
			};

			return callback.WrapSuspendableCallback(suspendable);
		}

		public static PropertyChangedCallback Callback<TTarget>(Func<TTarget, Action<DependencyProperty>> handlerFactory, bool suspendable = false)
			where TTarget : DependencyObject
		{
			if (handlerFactory == null) 
				return null;

			PropertyChangedCallback callback = (d, e) =>
			{
				var target = d.CastTarget<TTarget>();
				var handler = handlerFactory(target);
				
				handler(e.Property);
			};

			return callback.WrapSuspendableCallback(suspendable);
		}

		public static PropertyChangedCallback Callback<TTarget>(Action<TTarget> action, bool suspendable = false) where TTarget : DependencyObject
		{
			if (action == null)
				return null;

			PropertyChangedCallback callback = (d, e) =>
			{
				var target = d.CastTarget<TTarget>();
				
				action(target);
			};

			return callback.WrapSuspendableCallback(suspendable);
		}

		private static TProperty CastProperty<TProperty>(this object value)
		{
		  var type = typeof(TProperty);
		  
		  if (value is TProperty == false && value == null && type.IsValueType && Nullable.GetUnderlyingType(type) == null)
				throw new InvalidOperationException("Invalid property value");

			return (TProperty) value;
		}

		private static TTarget CastTarget<TTarget>(this DependencyObject dependencyObject) where TTarget : DependencyObject
		{
			var owner = dependencyObject as TTarget;
			
			if (owner == null)
				throw new InvalidOperationException("DependencyProperty cannot be set on object with type: " + typeof(TTarget));

			return owner;
		}

		//public static PropertyChangedCallback Callback<TTarget, TProperty>(Func<TTarget, Action<ValueChangedEventArgs<TProperty>>> handlerFactory)
		//  where TTarget : DependencyObject
		//{
		//  if (handlerFactory == null) return null;
		//  return (d, e) =>
		//  {
		//    var target = d.CastTarget<TTarget>();
		//    var handler = handlerFactory(target);
		//    handler(ValueChangedEventArgs.Create(e.OldValue.CastProperty<TProperty>(), e.NewValue.CastProperty<TProperty>()));
		//  };
		//}

		public static CoerceValueCallback Coerce<TTarget>(Func<TTarget, object, object> handler) where TTarget : DependencyObject
		{
			if (handler == null) 
				return null;
			
			return (o, v) => handler((TTarget) o, v);
		}

		public static CoerceValueCallback Coerce<TTarget>(Func<TTarget, Func<object, object>> handlerFactory) where TTarget : DependencyObject
		{
			if (handlerFactory == null)
				return null;

			object CoerceCallback(DependencyObject d, object v)
			{
				var target = d.CastTarget<TTarget>();
				var handler = handlerFactory(target);
				
				return handler(v);
			}

			return CoerceCallback;
		}

		public static CoerceValueCallback Coerce<TTarget, TProperty>(Func<TTarget, Func<TProperty, TProperty>> handlerFactory) where TTarget : DependencyObject
		{
			if (handlerFactory == null)
				return null;

			object CoerceCallback(DependencyObject d, object v)
			{
				var target = d.CastTarget<TTarget>();
				var handler = handlerFactory(target);
				
				return handler(v.CastProperty<TProperty>());
			}

			return CoerceCallback;
		}

	  public static CoerceValueCallback Coerce<TTarget, TProperty>(Func<TTarget, TProperty, TProperty> handler) where TTarget : DependencyObject
	  {
	    if (handler == null) 
		    return null;

	    object CoerceCallback(DependencyObject d, object v)
	    {
		    var target = d.CastTarget<TTarget>();

		    return handler(target, v.CastProperty<TProperty>());
	    }

	    return CoerceCallback;
	  }

    public static PropertyChangedCallback StaticCallback<TTarget, TProperty>(Action<TTarget, TProperty, TProperty> handler) where TTarget : DependencyObject
		{
			if (handler == null) 
				return null;
			
			return (d, e) =>
			{
				var target = d.CastTarget<TTarget>();
				
				handler(target, e.OldValue.CastProperty<TProperty>(), e.NewValue.CastProperty<TProperty>());
			};
		}

		public static PropertyChangedCallback StaticCallback<TTarget, TProperty>(Action<TTarget, TProperty> handler) where TTarget : DependencyObject
		{
			if (handler == null) 
				return null;
			
			return (d, e) => handler(d.CastTarget<TTarget>(), e.NewValue.CastProperty<TProperty>());
		}

		public static PropertyChangedCallback StaticCallback<TProperty>(Action<DependencyObject, TProperty> handler)
		{
			if (handler == null) 
				return null;
			
			return (d, e) => handler(d, e.NewValue.CastProperty<TProperty>());
		}

		//public static PropertyChangedCallback StaticCallback<TTarget, TProperty>(Action<TTarget, ValueChangedEventArgs<TProperty>> handler)
		//  where TTarget : DependencyObject
		//{
		//  return (d, e) =>
		//  {
		//    var target = d.CastTarget<TTarget>();
		//    handler(target, ValueChangedEventArgs.Create(e.OldValue.CastProperty<TProperty>(), e.NewValue.CastProperty<TProperty>()));
		//  };
		//}

		//public static PropertyChangedCallback StaticCallback<TProperty>(Action<DependencyObject, ValueChangedEventArgs<TProperty>> handler)
		//{
		//  if (handler == null) return null;
		//  return (d, e) => handler(d, ValueChangedEventArgs.Create(e.OldValue.CastProperty<TProperty>(), e.NewValue.CastProperty<TProperty>()));
		//}

		public static PropertyChangedCallback StaticCallback<TTarget>(Action<TTarget> handler) where TTarget : DependencyObject
		{
			if (handler == null) 
				return null;
			
			return (d, e) => handler(d.CastTarget<TTarget>());
		}

		public static PropertyChangedCallback StaticCallback<TProperty>(Action<DependencyObject, TProperty, TProperty> handler)
		{
			if (handler == null)
				return null;
			
			return (d, e) => handler(d, e.OldValue.CastProperty<TProperty>(), e.NewValue.CastProperty<TProperty>());
		}

		public static PropertyChangedCallback StaticCallback(Action<DependencyObject> handler)
		{
			if (handler == null)
				return null;
			
			return (d, e) => handler(d);
		}

		private static PropertyChangedCallback WrapSuspendableCallback(this PropertyChangedCallback callback, bool suspendable)
		{
			if (suspendable)
			{
				return (o, args) =>
				{
					if (o.IsPropertyChangedCallbackSuspended(args.Property))
						return;

					callback(o, args);
				};
			}

			return callback;
		}

		#endregion
	}
}