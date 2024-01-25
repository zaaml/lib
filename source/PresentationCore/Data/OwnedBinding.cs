// <copyright file="OwnedBinding.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Data
{
	internal sealed class OwnedBinding<T> : Binding
	{
		public OwnedBinding(T owner)
		{
			Owner = owner;
		}

		public OwnedBinding(T owner, string path) : base(path)
		{
			Owner = owner;
		}

		public T Owner { get; }
	}

	internal static class OwnedBinding
	{
		public static T ReadOwner<T>(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
		{
			return dependencyObject.ReadLocalBinding(dependencyProperty) is OwnedBinding<T> ownedBinding ? ownedBinding.Owner : default;
		}
	}

	internal static class OwnedBindingExtensions
	{
		public static T ReadBindingOwner<T>(this DependencyObject dependencyObject, DependencyProperty dependencyProperty)
		{
			return OwnedBinding.ReadOwner<T>(dependencyObject, dependencyProperty);
		}
	}
}