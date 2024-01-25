// <copyright file="AssetCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Data
{
	public class AssetCollection : InheritanceContextDependencyObjectCollection<InheritanceContextObject>
	{
		internal AssetCollection(FrameworkElement frameworkElement)
		{
			Owner = frameworkElement;
		}
	}
}