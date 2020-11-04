// <copyright file="StyleResourceItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore
{
	public sealed class StyleResourceItem : InheritanceContextObject
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty StyleProperty = DPM.Register<Style, StyleResourceItem>
			("Style", s => s.OnStyleChanged);

		private static readonly Binding ResourceOwnerBinding = new Binding { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(FrameworkElement), 1) };

		private static readonly DependencyProperty ResourceOwnerProperty = DPM.Register<FrameworkElement, StyleResourceItem>
			("ResourceOwner", s => s.OnResourceOwnerChanged);

		#endregion

		#region Ctors

		public StyleResourceItem()
		{
			this.SetBinding(ResourceOwnerProperty, ResourceOwnerBinding);
		}

		#endregion

		#region Properties

		public Style Style
		{
			get => (Style) GetValue(StyleProperty);
			set => SetValue(StyleProperty, value);
		}

		private ResourceDictionary StyleResourceDictionary { get; } = new ResourceDictionary();

		#endregion

		#region  Methods

		private void OnResourceOwnerChanged(FrameworkElement oldOwner, FrameworkElement newOwner)
		{
			oldOwner?.Resources.MergedDictionaries.Remove(StyleResourceDictionary);
			newOwner?.Resources.MergedDictionaries.Add(StyleResourceDictionary);
		}

		private void OnStyleChanged(Style oldStyle, Style newStyle)
		{
			if (oldStyle != null)
			{
				if (oldStyle.TargetType != null)
					StyleResourceDictionary.Remove(oldStyle.TargetType);
			}

			if (newStyle != null)
			{
				if (newStyle.TargetType != null)
					StyleResourceDictionary.Add(newStyle.TargetType, newStyle);
			}
		}

		#endregion
	}
}