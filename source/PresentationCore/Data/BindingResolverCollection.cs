// <copyright file="BindingResolverCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Data
{
	internal class BindingResolverCollection : DependencyObjectCollectionBase<BindingResolver>
	{
		public static readonly DependencyProperty ResolversProperty = DPM.RegisterAttached<BindingResolverCollection, BindingResolverCollection>
			("Resolvers");

		public static BindingResolverCollection GetResolvers(DependencyObject depObj)
		{
			return depObj.GetValueOrCreate(ResolversProperty, () => new BindingResolverCollection());
		}
	}
}