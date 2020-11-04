// <copyright file="ValidationMetadata.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;

namespace Zaaml.UI.Controls.ValidationSummary
{
	internal class ValidationMetadata : INotifyPropertyChanged
	{
		#region Fields

		private string _caption;
		private string _description;
		private bool _isRequired;

		#endregion

		#region Properties

		public string Caption
		{
			get => _caption;
			set
			{
				if (_caption == value)
					return;

				_caption = value;
				NotifyPropertyChanged("Caption");
			}
		}

		public string Description
		{
			get => _description;
			set
			{
				if (_description == value)
					return;

				_description = value;
				NotifyPropertyChanged("Description");
			}
		}

		public bool IsRequired
		{
			get => _isRequired;
			set
			{
				if (_isRequired == value)
					return;
				_isRequired = value;
				NotifyPropertyChanged("IsRequired");
			}
		}

		#endregion

		#region  Methods

		private void NotifyPropertyChanged(string propertyName)
		{
			var propertyChanged = PropertyChanged;
			propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		#region Interface Implementations

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#endregion
	}
}
