// <copyright file="ExpandoBindingExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.MarkupExtensions;

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
	public sealed class ExpandoBindingExtension : SourcedBindingBaseExtension
	{
		public string Path { get; set; }

		protected override System.Windows.Data.Binding GetBindingCore(IServiceProvider serviceProvider)
		{
			var binding = new System.Windows.Data.Binding
			{
				Path = ExpandoPathExtension.CreatePropertyPath(Path)
			};

			InitBinding(binding);
			InitBindingSource(binding);

			return binding;
		}
	}
}