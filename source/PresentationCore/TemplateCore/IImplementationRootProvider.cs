// <copyright file="IImplemntationRootProvider.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.TemplateCore
{
	internal interface IImplementationRootProvider
	{
		#region Properties

		FrameworkElement ImplementationRoot { get; }

		#endregion
	}
}
