// <copyright file="BindingProxy.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Interactivity
{
	internal interface IBindingProxyListener
	{
		#region  Methods

		void OnPropertyChanged(BindingProxy bindingProxy, object oldValue, object newValue);

		#endregion
	}

	internal class BindingProxyAttribute : Attribute
	{
		#region Properties

		public string PropertyChangedCallback { get; set; }
		public string ProxyMember { get; set; }

		#endregion
	}


	internal abstract class BindingProxyBase : IDependencyPropertyListener, IDisposable
	{
		#region Fields

		private readonly DependencyObject _dataObject;
		private readonly DependencyProperty _serviceProperty;

		#endregion

		#region Ctors

		protected BindingProxyBase(Binding binding, DependencyObject dataObject)
		{
			_serviceProperty = dataObject.GetDependencyPropertyService().CaptureServiceProperty(typeof(object), this);

			// Call sequence is essential here. When setting binding the OnPropertyChanged will be called immediately, but it should not propagate event on initialization,
			// _dataObject != null will be a flag indicating initialization is finished.
			dataObject.SetBinding(_serviceProperty, binding);
			_dataObject = dataObject;
		}

		#endregion

		#region Properties

		public Binding Binding => _dataObject.ReadLocalBinding(_serviceProperty);

		public object Value => _dataObject.GetValue(_serviceProperty);

		#endregion

		#region  Methods

		protected abstract void OnPropertyChanged(DependencyObject depObj, DependencyProperty dependencyProperty, object oldValue, object newValue);

		#endregion

		#region Interface Implementations

		#region IDependencyPropertyListener

		void IDependencyPropertyListener.OnPropertyChanged(DependencyObject depObj, DependencyProperty dependencyProperty, object oldValue, object newValue)
		{
			if (_dataObject != null)
				OnPropertyChanged(depObj, dependencyProperty, oldValue, newValue);
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			_dataObject.GetDependencyPropertyService().ReleaseServiceProperty(_serviceProperty);
		}

		#endregion

		#endregion
	}

	internal class BindingProxy : BindingProxyBase
	{
		#region Fields

		private readonly IBindingProxyListener _listener;

		#endregion

		#region Ctors

		public BindingProxy(Binding binding, DependencyObject dataObject) : base(binding, dataObject)
		{
		}

		public BindingProxy(Binding binding, DependencyObject dataObject, IBindingProxyListener listener) : base(binding, dataObject)
		{
			_listener = listener;
		}

		#endregion

		#region  Methods

		protected override void OnPropertyChanged(DependencyObject depObj, DependencyProperty dependencyProperty, object oldValue, object newValue)
		{
			_listener?.OnPropertyChanged(this, oldValue, newValue);
		}

		#endregion
	}
}