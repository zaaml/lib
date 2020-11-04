using System.ComponentModel;

namespace Zaaml.UI.Windows
{
	public class SkipCheckBoxOptions : INotifyPropertyChanged
	{
		#region Fields

		private bool _isChecked;
		private bool _isVisible;
		private string _text;

		#endregion

		#region Properties

		public bool IsChecked
		{
			get => _isChecked;
			set
			{
				_isChecked = value;
				OnPropertyChanged(nameof(IsChecked));
			}
		}

		public bool IsVisible
		{
			get => _isVisible;
			set
			{
				_isVisible = value;
				OnPropertyChanged(nameof(IsVisible));
			}
		}

		public string Text
		{
			get => _text;
			set
			{
				_text = value;
				OnPropertyChanged(nameof(Text));
			}
		}

		#endregion

		#region Methods

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}