// <copyright file="TypeIcon.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
	public sealed class TypeIcon : DependencyObject
	{
		public static readonly DependencyProperty TypeProperty = DPM.Register<Type, TypeIcon>
			("Type", d => d.OnTypePropertyChangedPrivate);

		public static readonly DependencyProperty IconProperty = DPM.Register<IconBase, TypeIcon>
			("Icon", default, d => d.OnIconPropertyChangedPrivate);

		public IconBase Icon
		{
			get => (IconBase)GetValue(IconProperty);
			set => SetValue(IconProperty, value);
		}

		public Type Type
		{
			get => (Type)GetValue(TypeProperty);
			set => SetValue(TypeProperty, value);
		}

		private void OnIconPropertyChangedPrivate(IconBase oldValue, IconBase newValue)
		{
		}

		private void OnTypePropertyChangedPrivate(Type oldValue, Type newValue)
		{
		}
	}
}