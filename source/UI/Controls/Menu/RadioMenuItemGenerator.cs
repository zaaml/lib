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
		#region Static Fields and Constants

		public static readonly DependencyProperty ItemTemplateProperty = DPM.Register<RadioMenuItemTemplate, RadioMenuItemGenerator>
			("ItemTemplate", g => g.OnItemTemplateChanged);

		#endregion

		#region Fields

		private readonly GeneratorDataTemplateHelper<RadioMenuItem, RadioMenuItem> _generatorDataTemplateHelper = new GeneratorDataTemplateHelper<RadioMenuItem, RadioMenuItem>();

		private MemberValueEvaluator _displayMemberEvaluator;

		#endregion

		#region Properties

		internal string DisplayMemberPath
		{
			get => _displayMemberEvaluator.ValuePath;
			set
			{
				if (string.Equals(DisplayMemberPath, value))
					return;

				_displayMemberEvaluator = new MemberValueEvaluator(value);

				OnGeneratorChanged();
			}
		}

		public MenuItemTemplate ItemTemplate
		{
			get => (MenuItemTemplate) GetValue(ItemTemplateProperty);
			set => SetValue(ItemTemplateProperty, value);
		}

		#endregion

		#region  Methods

		protected override void AttachItem(RadioMenuItem item, object itemSource)
		{
			_generatorDataTemplateHelper.AttachDataContext(item, itemSource);

			if (ItemTemplate != null)
				return;

			item.Header = _displayMemberEvaluator.GetValue(itemSource) ?? itemSource;
		}

		protected override RadioMenuItem CreateItem(object itemSource)
		{
			return _generatorDataTemplateHelper.Load(itemSource);
		}

		protected override void DetachItem(RadioMenuItem item, object itemSource)
		{
			if (ReferenceEquals(item.DataContext, itemSource))
				item.ClearValue(FrameworkElement.DataContextProperty);

			if (ReferenceEquals(item.Header, itemSource))
				item.ClearValue(HeaderedMenuItem.HeaderProperty);
		}

		protected override void DisposeItem(RadioMenuItem item, object itemSource)
		{
		}

		private void OnItemTemplateChanged()
		{
			_generatorDataTemplateHelper.DataTemplate = ItemTemplate;

			OnGeneratorChanged();
		}

		#endregion
	}
}