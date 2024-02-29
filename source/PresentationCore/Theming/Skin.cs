// <copyright file="Skin.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Theming
{
	[ContentProperty(nameof(SkinDictionary))]
	public abstract class ThemeSkinBase : SkinBase
	{
		private static readonly SkinDictionary NullDictionary = new();

		public static readonly DependencyProperty SkinDictionaryProperty = DPM.Register<SkinDictionary, ThemeSkinBase>
			("SkinDictionary", default, d => d.OnDictionaryPropertyChangedPrivate, d => d.CoerceDictionary);

		private SkinDictionary _frozenDictionary = NullDictionary;

		protected ThemeSkinBase()
		{
		}

		internal ThemeSkinBase(SkinDictionary frozenDictionary)
		{
			_frozenDictionary = frozenDictionary;
		}

		private SkinDictionary ActualDictionary => SkinDictionary ?? NullDictionary;

		private SkinDictionary FrozenDictionary
		{
			get
			{
				if (IsFrozenDictionary)
					return _frozenDictionary;

				return _frozenDictionary = FreezeDictionary();
			}
		}

		private bool IsFrozenDictionary => ReferenceEquals(_frozenDictionary, NullDictionary) == false;

		internal override IEnumerable<KeyValuePair<string, object>> Resources => FrozenDictionary.ShallowResources;

		public SkinDictionary SkinDictionary
		{
			get => (SkinDictionary)GetValue(SkinDictionaryProperty);
			set => SetValue(SkinDictionaryProperty, value);
		}

		protected abstract Theme Theme { get; }

		internal Theme ThemeInternal => Theme;

		private SkinDictionary CoerceDictionary(SkinDictionary dictionary)
		{
			VerifyChange();

			return dictionary;
		}

		private SkinDictionary FreezeDictionary()
		{
			return Theme?.Freeze(ActualDictionary) ?? ActualDictionary.AsFrozen();
		}

		protected override object GetValue(string key)
		{
			return FrozenDictionary.GetValueOrDefault(key);
		}

		protected override void OnAttached(DependencyObject dependencyObject)
		{
			base.OnAttached(dependencyObject);

			if (IsFrozenDictionary)
				return;

			_frozenDictionary = FreezeDictionary();
		}

		private void OnDictionaryPropertyChangedPrivate(SkinDictionary oldValue, SkinDictionary newValue)
		{
		}

		private void VerifyChange()
		{
			if (IsFrozenDictionary)
				throw new InvalidOperationException("Skin is frozen and can not be modified");
		}
	}

	public abstract class ThemeSkin<TTheme> : ThemeSkinBase
		where TTheme : Theme
	{
		protected ThemeSkin(TTheme theme)
		{
			Theme = theme;
		}

		protected override Theme Theme { get; }
	}

	public sealed class GenericThemeSkin : ThemeSkinBase
	{
		public GenericThemeSkin()
		{
		}

		public GenericThemeSkin(SkinDictionary frozenDictionary) : base(frozenDictionary)
		{
		}

		protected override Theme Theme => null;
	}
}