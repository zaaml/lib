// <copyright file="IDependencyObjectProxy.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Data
{
	internal interface IDependencyObjectProxy
	{
		DependencyObject Proxy { get; }

		DependencyProperty GetProxyProperty(string propertyName);
	}
}