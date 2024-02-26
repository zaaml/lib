// <copyright file="TypeIconSelector.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
	[ContentProperty(nameof(TypeIcons))]
	public sealed class TypeIconSelector : DependencyObject, IIconSelector
	{
		private static readonly DependencyPropertyKey TypeIconsPropertyKey = DPM.RegisterAttachedReadOnly<TypeIconCollection, TypeIconSelector>
			("TypeIconsPrivate");

		public static readonly DependencyProperty TypeIconsProperty = TypeIconsPropertyKey.DependencyProperty;

		public static readonly DependencyProperty DefaultIconProperty = DPM.Register<IconBase, TypeIconSelector>
			("DefaultIcon");

		private readonly Dictionary<Type, IconBase> _typeIcons = [];

		public IconBase DefaultIcon
		{
			get => (IconBase)GetValue(DefaultIconProperty);
			set => SetValue(DefaultIconProperty, value);
		}

		public TypeIconCollection TypeIcons => this.GetValueOrCreate(TypeIconsPropertyKey, () => new TypeIconCollection(this));

		internal void OnChangedInternal()
		{
			_typeIcons.Clear();
		}

		public IconBase Select(object source)
		{
			if (source == null)
				return null;

			var type = source.GetType();

			if (_typeIcons.TryGetValue(type, out var icon))
				return icon;

			var iconTypeDistance = int.MaxValue;

			foreach (var typeIcon in TypeIcons)
			{
				if (typeIcon.Type.IsAssignableFrom(type))
				{
					var iconType = typeIcon.Type;
					var distance = 0;

					while (iconType != null)
					{
						if (iconType == type)
							break;

						iconType = iconType.BaseType;
						distance++;
					}

					if (distance < iconTypeDistance)
					{
						icon = typeIcon.Icon;
						iconTypeDistance = distance;
					}
				}
			}

			icon ??= DefaultIcon;

			_typeIcons.Add(type, icon);

			return icon;
		}
	}
}