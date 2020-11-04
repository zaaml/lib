// <copyright file="SortDescription.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;

namespace Zaaml.UI.Data
{
	public sealed class SortDescription : INotifyPropertyChanged
	{
		#region Fields

		private SortDirection _direction;
		private string _propertyName;

		#endregion

		#region Properties

		public SortDirection Direction
		{
			get => _direction;
			set
			{
				if (_direction == value)
					return;

				_direction = value;

				OnPropertyChanged(nameof(Direction));
			}
		}

		public string PropertyName
		{
			get => _propertyName;
			set
			{
				if (string.Equals(_propertyName, value))
					return;

				_propertyName = value;

				OnPropertyChanged(nameof(PropertyName));
			}
		}

		internal System.ComponentModel.SortDescription ToComponentModel()
		{
			return new System.ComponentModel.SortDescription(PropertyName, Direction == SortDirection.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);
		}

		#endregion

		#region  Methods

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		#region Interface Implementations

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#endregion
	}
}
