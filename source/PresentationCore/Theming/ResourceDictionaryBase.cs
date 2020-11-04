// <copyright file="ResourceDictionaryBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Theming
{
	public class ResourceDictionaryBase : ResourceDictionary, IActualUriSource, IResourceValue
	{
		#region Fields

		private string _actualKey;

		#endregion

		#region Properties

		internal string ActualKey
		{
			get => _actualKey;
			set
			{
				if (string.Equals(_actualKey, value, StringComparison.OrdinalIgnoreCase))
					return;

				_actualKey = value;

				OnActualKeyChanged();
			}
		}

		internal Uri LoadUri { get; set; }

		#endregion

		#region  Methods

		internal virtual void OnActualKeyChanged()
		{
		}

		public override string ToString()
		{
			return ActualKey;
		}

		#endregion

		#region Interface Implementations

		#region IActualUriSource

		Uri IActualUriSource.ActualSource { get; set; }

		#endregion

		#region IResourceValue

		string IResourceValue.Key
		{
			get => ActualKey;
			set => ActualKey = value;
		}

		#endregion

		#endregion
	}
}