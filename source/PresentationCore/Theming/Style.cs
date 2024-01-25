// <copyright file="Style.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Interactivity;
using Setter = Zaaml.PresentationCore.Interactivity.Setter;
using SetterBase = Zaaml.PresentationCore.Interactivity.SetterBase;
using TriggerBase = Zaaml.PresentationCore.Interactivity.TriggerBase;

namespace Zaaml.PresentationCore.Theming
{
	[ContentProperty("Setters")]
	public sealed class Style : StyleBase
	{
		private StyleResourceDictionary _resources;
		private StyleSetterCollection _setters;
		private StyleTriggerCollection _triggers;

		public StyleBase BasedOn
		{
			get => BasedOnCore;
			set => BasedOnCore = value;
		}

		public ResourceDictionary Resources => _resources ??= new StyleResourceDictionary(this);

		public SetterCollectionBase Setters => _setters ??= new StyleSetterCollection(this);

		protected override IEnumerable<SetterBase> SettersCore
		{
			get
			{
				var settersCore = _setters ?? Enumerable.Empty<SetterBase>();

				var skin = Skin;

				if (skin == null)
					return settersCore;

				var setter = new Setter
				{
					Property = Extension.StyleSkinProperty
				};

				if (skin is DeferSkin deferSkin)
					setter.Value = new ThemeResourceExtension { Key = deferSkin.Key, Converter = IsStyleExtensionConverter.Instance };
				else
					setter.Value = skin;

				return settersCore.Prepend(setter);
			}
		}

		[TypeConverter(typeof(SkinTypeConverter))]
		public SkinBase Skin { get; set; }

		public TriggerCollectionBase Triggers => _triggers ??= new StyleTriggerCollection(this);

		protected override IEnumerable<TriggerBase> TriggersCore => _triggers ?? Enumerable.Empty<TriggerBase>();

		internal void Merge(List<StyleBase> styles)
		{
			Setters.AddRange(MergeSetters(styles));
			Triggers.AddRange(MergeTriggers(styles));
		}
	}
}