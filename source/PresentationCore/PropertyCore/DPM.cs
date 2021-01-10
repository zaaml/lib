// <copyright file="DPM.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.PresentationCore.PropertyCore
{
	public static partial class DPM
	{
		#region  Methods

		public static DependencyProperty Register<TProperty, TOwner>(string name) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(default(TProperty)));
		}

	  public static DependencyProperty Register<TProperty, TOwner>(string name, PropertyChangedCallback handler) where TOwner : DependencyObject
	  {
	    return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(default(TProperty), handler));
	  }

    public static DependencyProperty Register<TProperty, TOwner>(string name, Action<TOwner> handler, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(default(TProperty), Callback(handler, suspendable)));
		}

		public static DependencyProperty Register<TProperty, TOwner>(string name, Func<TOwner, Action> handlerFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(default(TProperty), Callback(handlerFactory, suspendable)));
		}

		public static DependencyProperty Register<TProperty, TOwner>(string name, Func<TOwner, Action> handlerFactory, Func<TOwner, Func<object, object>> coerceFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(default(TProperty), Callback(handlerFactory, suspendable), Coerce(coerceFactory)));
		}

		public static DependencyProperty Register<TProperty, TOwner>(string name, Func<TOwner, Action<TProperty>> handlerFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(default(TProperty), Callback(handlerFactory, suspendable)));
		}

		public static DependencyProperty Register<TProperty, TOwner>(string name, Func<TOwner, Action<TProperty>> handlerFactory, Func<TOwner, Func<object, object>> coerceFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(default(TProperty), Callback(handlerFactory, suspendable), Coerce(coerceFactory)));
		}

		public static DependencyProperty Register<TProperty, TOwner>(string name, Func<TOwner, Action<TProperty, TProperty>> handlerFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(default(TProperty), Callback(handlerFactory, suspendable)));
		}

		public static DependencyProperty Register<TProperty, TOwner>(string name, Func<TOwner, Action<TProperty, TProperty>> handlerFactory, Func<TOwner, Func<object, object>> coerceFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(default(TProperty), Callback(handlerFactory, suspendable), Coerce(coerceFactory)));
		}


		public static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue));
		}

	  public static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue, PropertyChangedCallback handler) where TOwner : DependencyObject
	  {
	    return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, handler));
	  }

    public static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue, Action<TOwner> handler, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, Callback(handler, suspendable)));
		}

		public static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action> handlerFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, Callback(handlerFactory, suspendable)));
		}

		public static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action> handlerFactory, Func<TOwner, Func<object, object>> coerceFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, Callback(handlerFactory, suspendable), Coerce(coerceFactory)));
		}

		public static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<TProperty>> handlerFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, Callback(handlerFactory, suspendable)));
		}

		public static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<TProperty>> handlerFactory, Func<TOwner, Func<object, object>> coerceFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, Callback(handlerFactory, suspendable), Coerce(coerceFactory)));
		}

		public static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<TProperty, TProperty>> handlerFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, Callback(handlerFactory, suspendable)));
		}

		public static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<TProperty, TProperty>> handlerFactory, Func<TOwner, Func<TProperty, TProperty>> coerceFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, Callback(handlerFactory, suspendable), Coerce(coerceFactory)));
		}

	  public static DependencyProperty Register<TProperty, TOwner>(string name, Func<TOwner, Action<TProperty, TProperty>> handlerFactory, Func<TOwner, Func<TProperty, TProperty>> coerceFactory, bool suspendable = false) where TOwner : DependencyObject
	  {
	    return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(default(TProperty), Callback(handlerFactory, suspendable), Coerce(coerceFactory)));
	  }

    public static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<DependencyPropertyChangedEventArgs>> handlerFactory, Func<TOwner, Func<TProperty, TProperty>> coerceFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, Callback(handlerFactory, suspendable), Coerce(coerceFactory)));
		}

		public static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<DependencyProperty, TProperty, TProperty>> handlerFactory, Func<TOwner, Func<TProperty, TProperty>> coerceFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, Callback(handlerFactory, suspendable), Coerce(coerceFactory)));
		}

		public static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<DependencyProperty, TProperty>> handlerFactory, Func<TOwner, Func<TProperty, TProperty>> coerceFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, Callback(handlerFactory, suspendable), Coerce(coerceFactory)));
		}

		public static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<DependencyProperty>> handlerFactory, Func<TOwner, Func<TProperty, TProperty>> coerceFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, Callback(handlerFactory, suspendable), Coerce(coerceFactory)));
		}

		public static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<DependencyPropertyChangedEventArgs>> handlerFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, Callback(handlerFactory, suspendable)));
		}

		public static DependencyProperty Register<TProperty, TOwner>(string name, Func<TOwner, Action<DependencyPropertyChangedEventArgs>> handlerFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(default(TProperty), Callback(handlerFactory, suspendable)));
		}

		public static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<DependencyProperty, TProperty, TProperty>> handlerFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, Callback(handlerFactory, suspendable)));
		}

		public static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<DependencyProperty, TProperty>> handlerFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, Callback(handlerFactory, suspendable)));
		}

		public static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<DependencyProperty>> handlerFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.Register(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, Callback(handlerFactory, suspendable)));
		}

		public static DependencyProperty Register<TProperty, TOwner>(string name, TProperty defaultValue, PropertyChangedCallback handler, CoerceValueCallback coerce)
		{
			return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, handler, coerce));
		}

		public static DependencyProperty Register<TProperty, TOwner>(string name, PropertyChangedCallback handler, CoerceValueCallback coerce)
		{
			return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(default(TProperty), handler, coerce));
		}

		public static DependencyProperty RegisterAttached<TProperty, TOwner>(string name)
		{
			return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(default(TProperty)));
		}

		public static DependencyProperty RegisterAttached<TProperty, TOwner>(string name, TProperty defaultValue)
		{
			return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue));
		}

		public static DependencyProperty RegisterAttached<TProperty, TOwner>(string name, TProperty defaultValue, Action<DependencyObject> handler)
		{
			return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, StaticCallback(handler)));
		}

		public static DependencyProperty RegisterAttached<TProperty, TOwner>(string name, TProperty defaultValue, Action<DependencyObject, TProperty, TProperty> handler)
		{
			return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, StaticCallback(handler)));
		}

	  public static DependencyProperty RegisterAttached<TProperty, TOwner>(string name, TProperty defaultValue, PropertyChangedCallback handler, CoerceValueCallback coerce)
	  {
	    return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, handler, coerce));
	  }

	  public static DependencyProperty RegisterAttached<TProperty, TOwner>(string name, TProperty defaultValue, Action<DependencyObject, TProperty, TProperty> handler, Func<DependencyObject, TProperty, TProperty> coerce)
	  {
	    return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, StaticCallback(handler), Coerce(coerce)));
	  }

    public static DependencyProperty RegisterAttached<TProperty, TOwner>(string name, Action<DependencyObject, TProperty, TProperty> handler)
		{
			return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(default(TProperty), StaticCallback(handler)));
		}

		public static DependencyProperty RegisterAttached<TProperty, TOwner>(string name, Action<DependencyObject> handler)
		{
			return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(default(TProperty), StaticCallback(handler)));
		}

		public static DependencyProperty RegisterAttached<TProperty, TOwner>(string name, PropertyChangedCallback handler)
		{
			return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(default(TProperty), handler));
		}

		public static DependencyProperty RegisterAttached<TProperty, TOwner>(string name, TProperty defaultValue, PropertyChangedCallback handler)
		{
			return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, handler));
		}

	  public static DependencyProperty RegisterAttached<TProperty, TOwner>(string name, PropertyMetadata propertyMetadata)
	  {
	    return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), typeof(TOwner), propertyMetadata);
	  }


    public static DependencyProperty RegisterAttached<TProperty>(string name, Type ownerType)
		{
			return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), ownerType, new PropertyMetadataXm(default(TProperty)));
		}

		public static DependencyProperty RegisterAttached<TProperty>(string name, Type ownerType, TProperty defaultValue)
		{
			return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), ownerType, new PropertyMetadataXm(defaultValue));
		}

		public static DependencyProperty RegisterAttached<TProperty>(string name, Type ownerType, TProperty defaultValue, Action<DependencyObject> handler)
		{
			return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), ownerType, new PropertyMetadataXm(defaultValue, StaticCallback(handler)));
		}

		public static DependencyProperty RegisterAttached<TProperty>(string name, Type ownerType, TProperty defaultValue, Action<DependencyObject, TProperty, TProperty> handler)
		{
			return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), ownerType, new PropertyMetadataXm(defaultValue, StaticCallback(handler)));
		}

	  public static DependencyProperty RegisterAttached<TProperty>(string name, Type ownerType, TProperty defaultValue, Action<DependencyObject, TProperty, TProperty> handler, Func<DependencyObject, TProperty, TProperty> coerce)
	  {
	    return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), ownerType, new PropertyMetadataXm(defaultValue, StaticCallback(handler), Coerce(coerce)));
	  }

    public static DependencyProperty RegisterAttached<TProperty>(string name, Type ownerType, Action<DependencyObject, TProperty, TProperty> handler)
		{
			return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), ownerType, new PropertyMetadataXm(default(TProperty), StaticCallback(handler)));
		}

	  public static DependencyProperty RegisterAttached<TProperty>(string name, Type ownerType, Action<DependencyObject, TProperty, TProperty> handler, Func<DependencyObject, TProperty, TProperty> coerce)
	  {
	    return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), ownerType, new PropertyMetadataXm(default(TProperty), StaticCallback(handler), Coerce(coerce)));
	  }

    public static DependencyProperty RegisterAttached<TProperty>(string name, Type ownerType, Action<DependencyObject> handler)
		{
			return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), ownerType, new PropertyMetadataXm(default(TProperty), StaticCallback(handler)));
		}

		public static DependencyProperty RegisterAttached<TProperty>(string name, Type ownerType, PropertyChangedCallback handler)
		{
			return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), ownerType, new PropertyMetadataXm(default(TProperty), handler));
		}

		public static DependencyProperty RegisterAttached<TProperty>(string name, Type ownerType, TProperty defaultValue, PropertyChangedCallback handler)
		{
			return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), ownerType, new PropertyMetadataXm(defaultValue, handler));
		}

	  public static DependencyProperty RegisterAttached<TProperty>(string name, Type ownerType, TProperty defaultValue, PropertyChangedCallback handler, CoerceValueCallback coerce)
	  {
	    return DependencyPropertyManager.RegisterAttached(name, typeof(TProperty), ownerType, new PropertyMetadataXm(defaultValue, handler, coerce));
	  }



    public static DependencyPropertyKey RegisterAttachedReadOnly<TProperty, TOwner>(string name)
		{
			return DependencyPropertyManager.RegisterAttachedReadOnly(name, typeof(TProperty), typeof(TOwner), new ReadOnlyPropertyMetadataInternal(default(TProperty)));
		}

		public static DependencyPropertyKey RegisterAttachedReadOnly<TProperty, TOwner>(string name, TProperty defaultValue)
		{
			return DependencyPropertyManager.RegisterAttachedReadOnly(name, typeof(TProperty), typeof(TOwner), new ReadOnlyPropertyMetadataInternal(defaultValue));
		}

		public static DependencyPropertyKey RegisterAttachedReadOnly<TProperty, TOwner>(string name, TProperty defaultValue, Action<DependencyObject> handler)
		{
			return DependencyPropertyManager.RegisterAttachedReadOnly(name, typeof(TProperty), typeof(TOwner), new ReadOnlyPropertyMetadataInternal(defaultValue, StaticCallback(handler)));
		}

		public static DependencyPropertyKey RegisterAttachedReadOnly<TProperty, TOwner>(string name, TProperty defaultValue, Action<DependencyObject, TProperty, TProperty> handler)
		{
			return DependencyPropertyManager.RegisterAttachedReadOnly(name, typeof(TProperty), typeof(TOwner), new ReadOnlyPropertyMetadataInternal(defaultValue, StaticCallback(handler)));
		}

		public static DependencyPropertyKey RegisterAttachedReadOnly<TProperty, TOwner>(string name, Action<DependencyObject, TProperty, TProperty> handler)
		{
			return DependencyPropertyManager.RegisterAttachedReadOnly(name, typeof(TProperty), typeof(TOwner), new ReadOnlyPropertyMetadataInternal(default(TProperty), StaticCallback(handler)));
		}

		public static DependencyPropertyKey RegisterAttachedReadOnly<TProperty, TOwner>(string name, Action<DependencyObject> handler)
		{
			return DependencyPropertyManager.RegisterAttachedReadOnly(name, typeof(TProperty), typeof(TOwner), new ReadOnlyPropertyMetadataInternal(default(TProperty), StaticCallback(handler)));
		}

		public static DependencyPropertyKey RegisterAttachedReadOnly<TProperty, TOwner>(string name, PropertyChangedCallback handler)
		{
			return DependencyPropertyManager.RegisterAttachedReadOnly(name, typeof(TProperty), typeof(TOwner), new ReadOnlyPropertyMetadataInternal(default(TProperty), handler));
		}

		public static DependencyPropertyKey RegisterAttachedReadOnly<TProperty, TOwner>(string name, TProperty defaultValue, PropertyChangedCallback handler)
		{
			return DependencyPropertyManager.RegisterAttachedReadOnly(name, typeof(TProperty), typeof(TOwner), new ReadOnlyPropertyMetadataInternal(defaultValue, handler));
		}


		public static DependencyPropertyKey RegisterAttachedReadOnly<TProperty>(string name, Type ownerType)
		{
			return DependencyPropertyManager.RegisterAttachedReadOnly(name, typeof(TProperty), ownerType, new ReadOnlyPropertyMetadataInternal(default(TProperty)));
		}

		public static DependencyPropertyKey RegisterAttachedReadOnly<TProperty>(string name, Type ownerType, TProperty defaultValue)
		{
			return DependencyPropertyManager.RegisterAttachedReadOnly(name, typeof(TProperty), ownerType, new ReadOnlyPropertyMetadataInternal(defaultValue));
		}

		public static DependencyPropertyKey RegisterAttachedReadOnly<TProperty>(string name, Type ownerType, TProperty defaultValue, Action<DependencyObject> handler)
		{
			return DependencyPropertyManager.RegisterAttachedReadOnly(name, typeof(TProperty), ownerType, new ReadOnlyPropertyMetadataInternal(defaultValue, StaticCallback(handler)));
		}

		public static DependencyPropertyKey RegisterAttachedReadOnly<TProperty>(string name, Type ownerType, TProperty defaultValue, Action<DependencyObject, TProperty, TProperty> handler)
		{
			return DependencyPropertyManager.RegisterAttachedReadOnly(name, typeof(TProperty), ownerType, new ReadOnlyPropertyMetadataInternal(defaultValue, StaticCallback(handler)));
		}

		public static DependencyPropertyKey RegisterAttachedReadOnly<TProperty>(string name, Type ownerType, Action<DependencyObject, TProperty, TProperty> handler)
		{
			return DependencyPropertyManager.RegisterAttachedReadOnly(name, typeof(TProperty), ownerType, new ReadOnlyPropertyMetadataInternal(default(TProperty), StaticCallback(handler)));
		}

		public static DependencyPropertyKey RegisterAttachedReadOnly<TProperty>(string name, Type ownerType, Action<DependencyObject> handler)
		{
			return DependencyPropertyManager.RegisterAttachedReadOnly(name, typeof(TProperty), ownerType, new ReadOnlyPropertyMetadataInternal(default(TProperty), StaticCallback(handler)));
		}

		public static DependencyPropertyKey RegisterAttachedReadOnly<TProperty>(string name, Type ownerType, PropertyChangedCallback handler)
		{
			return DependencyPropertyManager.RegisterAttachedReadOnly(name, typeof(TProperty), ownerType, new ReadOnlyPropertyMetadataInternal(default(TProperty), handler));
		}

		public static DependencyPropertyKey RegisterAttachedReadOnly<TProperty>(string name, Type ownerType, TProperty defaultValue, PropertyChangedCallback handler)
		{
			return DependencyPropertyManager.RegisterAttachedReadOnly(name, typeof(TProperty), ownerType, new ReadOnlyPropertyMetadataInternal(defaultValue, handler));
		}

		public static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.RegisterReadOnly(name, typeof(TProperty), typeof(TOwner), new ReadOnlyPropertyMetadataInternal(default(TProperty)));
		}

		public static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, Action<TOwner> handler, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.RegisterReadOnly(name, typeof(TProperty), typeof(TOwner), new ReadOnlyPropertyMetadataInternal(default(TProperty), Callback(handler, suspendable)));
		}

		public static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, Func<TOwner, Action> handlerFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.RegisterReadOnly(name, typeof(TProperty), typeof(TOwner), new ReadOnlyPropertyMetadataInternal(default(TProperty), Callback(handlerFactory, suspendable)));
		}

		public static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, Func<TOwner, Action<TProperty>> handlerFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.RegisterReadOnly(name, typeof(TProperty), typeof(TOwner), new ReadOnlyPropertyMetadataInternal(default(TProperty), Callback(handlerFactory, suspendable)));
		}

		public static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, Func<TOwner, Action<TProperty, TProperty>> handlerFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.RegisterReadOnly(name, typeof(TProperty), typeof(TOwner), new ReadOnlyPropertyMetadataInternal(default(TProperty), Callback(handlerFactory, suspendable)));
		}


		public static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, TProperty defaultValue) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.RegisterReadOnly(name, typeof(TProperty), typeof(TOwner), new ReadOnlyPropertyMetadataInternal(defaultValue));
		}

		public static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, TProperty defaultValue, Action<TOwner> handler, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.RegisterReadOnly(name, typeof(TProperty), typeof(TOwner), new ReadOnlyPropertyMetadataInternal(defaultValue, Callback(handler, suspendable)));
		}

		public static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action> handlerFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.RegisterReadOnly(name, typeof(TProperty), typeof(TOwner), new ReadOnlyPropertyMetadataInternal(defaultValue, Callback(handlerFactory, suspendable)));
		}

		public static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<TProperty>> handlerFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.RegisterReadOnly(name, typeof(TProperty), typeof(TOwner), new ReadOnlyPropertyMetadataInternal(defaultValue, Callback(handlerFactory, suspendable)));
		}

		public static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<TProperty, TProperty>> handlerFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.RegisterReadOnly(name, typeof(TProperty), typeof(TOwner), new ReadOnlyPropertyMetadataInternal(defaultValue, Callback(handlerFactory, suspendable)));
		}

		public static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, Func<TOwner, Action<TProperty, TProperty>> handlerFactory, Func<TOwner, Func<object, object>> coerceFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.RegisterReadOnly(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(default(TProperty), Callback(handlerFactory, suspendable), Coerce(coerceFactory)));
		}




		
		public static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, Func<TOwner, Action> handlerFactory, Func<TOwner, Func<object, object>> coerceFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.RegisterReadOnly(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(default(TProperty), Callback(handlerFactory, suspendable), Coerce(coerceFactory)));
		}

		public static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, Func<TOwner, Action<TProperty>> handlerFactory, Func<TOwner, Func<object, object>> coerceFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.RegisterReadOnly(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(default(TProperty), Callback(handlerFactory, suspendable), Coerce(coerceFactory)));
		}

		

		public static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action> handlerFactory, Func<TOwner, Func<object, object>> coerceFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.RegisterReadOnly(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, Callback(handlerFactory, suspendable), Coerce(coerceFactory)));
		}

		public static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<TProperty, TProperty>> handlerFactory, Func<TOwner, Func<TProperty, TProperty>> coerceFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.RegisterReadOnly(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, Callback(handlerFactory, suspendable), Coerce(coerceFactory)));
		}

	  public static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, Func<TOwner, Action<TProperty, TProperty>> handlerFactory, Func<TOwner, Func<TProperty, TProperty>> coerceFactory, bool suspendable = false) where TOwner : DependencyObject
	  {
	    return DependencyPropertyManager.RegisterReadOnly(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(default(TProperty), Callback(handlerFactory, suspendable), Coerce(coerceFactory)));
	  }

    public static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<DependencyPropertyChangedEventArgs>> handlerFactory, Func<TOwner, Func<TProperty, TProperty>> coerceFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.RegisterReadOnly(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, Callback(handlerFactory, suspendable), Coerce(coerceFactory)));
		}

		public static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<DependencyProperty, TProperty, TProperty>> handlerFactory, Func<TOwner, Func<TProperty, TProperty>> coerceFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.RegisterReadOnly(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, Callback(handlerFactory, suspendable), Coerce(coerceFactory)));
		}

		public static DependencyPropertyKey RegRegisterReadOnly<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<DependencyProperty, TProperty>> handlerFactory, Func<TOwner, Func<TProperty, TProperty>> coerceFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.RegisterReadOnly(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, Callback(handlerFactory, suspendable), Coerce(coerceFactory)));
		}

		public static DependencyPropertyKey RegisterReadOnly<TProperty, TOwner>(string name, TProperty defaultValue, Func<TOwner, Action<DependencyProperty>> handlerFactory, Func<TOwner, Func<TProperty, TProperty>> coerceFactory, bool suspendable = false) where TOwner : DependencyObject
		{
			return DependencyPropertyManager.RegisterReadOnly(name, typeof(TProperty), typeof(TOwner), new PropertyMetadataXm(defaultValue, Callback(handlerFactory, suspendable), Coerce(coerceFactory)));
		}

		#endregion
	}
}