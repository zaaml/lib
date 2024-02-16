using System;
using Zaaml.Core;

namespace Zaaml.PresentationCore.PropertyCore
{
	public interface IPropertyChangeProvider<in TOwner, in TPropertyValue>
	{
		void OnPropertyChanged(TOwner sender, TPropertyValue oldValue, TPropertyValue newValue);
	}

	public interface IPropertyChangeProvider : IDisposable
	{
		event EventHandler<PropertyValueChangedEventArgs> PropertyChanged;

		object Source { get; }
	}
}