// <copyright file="Binding.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

#if SILVERLIGHT
using System.ComponentModel;
#endif
using System.Windows.Data;
using NativeBinding = System.Windows.Data.Binding;

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
	public sealed class Binding : PathBindingBase
	{
		private BindingSource _bindingSource;

		public bool BindsDirectlyToSource { get; set; }

		public string ElementName
		{
			get => _bindingSource.ElementName;
			set => _bindingSource.ElementName = value;
		}

		public RelativeSource RelativeSource
		{
			get => _bindingSource.RelativeSource;
			set => _bindingSource.RelativeSource = value;
		}

		public object Source
		{
			get => _bindingSource.Source;
			set => _bindingSource.Source = value;
		}

		protected override void InitSource(NativeBinding binding)
		{
			_bindingSource.InitSource(binding);
		}
	}
}