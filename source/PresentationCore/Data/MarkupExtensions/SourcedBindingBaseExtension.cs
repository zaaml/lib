// <copyright file="SourcedBindingBaseExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Data;

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
	public abstract class SourcedBindingBaseExtension : BindingBaseExtension
	{
		private BindingSource _bindingSource;

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

		protected void InitBindingSource(System.Windows.Data.Binding binding)
		{
			_bindingSource.InitSource(binding);
		}
	}
}