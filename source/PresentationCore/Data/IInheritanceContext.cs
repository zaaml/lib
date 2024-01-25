// <copyright file="IInheritanceContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Data
{
	internal interface IInheritanceContext
	{
		DependencyObject Owner { get; set; }

		void Attach(DependencyObject depObj);

		void Detach(DependencyObject depObj);
	}
}