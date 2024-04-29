// <copyright file="RadioMenuItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Menu
{
	public class RadioMenuItemGenerator : RadioMenuItemGeneratorBase
	{
		public static readonly DependencyProperty ItemTemplateProperty = DPM.Register<RadioMenuItemTemplate, RadioMenuItemGenerator>
			("ItemTemplate", g => g.OnItemTemplateChanged);

		private readonly GeneratorDataTemplateHelper<RadioMenuItem, RadioMenuItem> _generatorDataTemplateHelper = new();

		private MemberEvaluator _displayMemberEvaluator;

		internal string DisplayMember
		{
			get => _displayMemberEvaluator.Member;
			set
			{
				if (string.Equals(DisplayMember, value))
					return;

				_displayMemberEvaluator = new MemberEvaluator(value);

				OnGeneratorChanged();
			}
		}

		public MenuItemTemplate ItemTemplate
		{
			get => (MenuItemTemplate)GetValue(ItemTemplateProperty);
			set => SetValue(ItemTemplateProperty, value);
		}

		protected override void AttachItem(RadioMenuItem item, object source)
		{
			_generatorDataTemplateHelper.AttachDataContext(item, source);

			if (ItemTemplate != null)
				return;

			item.Header = _displayMemberEvaluator.GetValue(source) ?? source;
		}

		protected override RadioMenuItem CreateItem(object source)
		{
			return _generatorDataTemplateHelper.Load(source);
		}

		protected override void DetachItem(RadioMenuItem item, object source)
		{
			if (ReferenceEquals(item.DataContext, source))
				item.ClearValue(FrameworkElement.DataContextProperty);

			if (ReferenceEquals(item.Header, source))
				item.ClearValue(HeaderedMenuItem.HeaderProperty);
		}

		protected override void DisposeItem(RadioMenuItem item, object source)
		{
		}

		private void OnItemTemplateChanged()
		{
			_generatorDataTemplateHelper.DataTemplate = ItemTemplate;

			OnGeneratorChanged();
		}
	}
}