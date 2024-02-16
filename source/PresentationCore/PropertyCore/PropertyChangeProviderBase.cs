// <copyright file="PropertyChangeProviderBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core;

namespace Zaaml.PresentationCore.PropertyCore
{
	public abstract class PropertyChangeProviderBase : IPropertyChangeProvider
	{
		public virtual void OnPropertyChanged(object sender, object oldValue, object newValue, object property)
		{
			if (Equals(oldValue, newValue) == false)
				OnPropertyChanged(new PropertyValueChangedEventArgs(sender, oldValue, newValue, property));
		}

		protected virtual void OnPropertyChanged(PropertyValueChangedEventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}

		public virtual void Dispose()
		{
		}

		public event EventHandler<PropertyValueChangedEventArgs> PropertyChanged;

		public abstract object Source { get; }
	}
}