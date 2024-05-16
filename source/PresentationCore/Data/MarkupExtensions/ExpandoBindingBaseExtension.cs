// <copyright file="ExpandoBindingBaseExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Data;
using Zaaml.PresentationCore.MarkupExtensions;
using NativeBinding = System.Windows.Data.Binding;

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
	public abstract class ExpandoBindingBaseExtension : BindingBaseExtension
	{
		internal ExpandoBindingBaseExtension()
		{
		}

		public string Path { get; set; }
		protected abstract RelativeSource Source { get; }

		protected override NativeBinding GetBindingCore(IServiceProvider serviceProvider)
		{
			var binding = new NativeBinding
			{
				Path = ExpandoPathExtension.CreatePropertyPath(Path),
				RelativeSource = Source
			};

			InitBinding(binding);

			return binding;
		}
	}
}