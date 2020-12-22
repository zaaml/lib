// <copyright file="RadioMenuItemGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Control = System.Windows.Controls.Control;

namespace Zaaml.UI.Controls.Menu
{
	[ContentProperty(nameof(Items))]
	[TemplateContractType(typeof(RadioMenuItemGroupTemplateContract))]
	public class RadioMenuItemGroup : MenuItemGroupBase<RadioMenuItem, RadioMenuItemsPresenterHost, RadioMenuItemsPanel>, ISelector<RadioMenuItem>, ISupportInitialize
	{
		#region Static Fields and Constants

		private static readonly DependencyPropertyKey ItemsPropertyKey = DPM.RegisterReadOnly<RadioMenuItemCollection, RadioMenuItemGroup>
			("ItemsInt");

		public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

		public static readonly DependencyProperty SelectedIndexProperty = DPM.Register<int, RadioMenuItemGroup>
			("SelectedIndex", -1, s => s.SelectorController.OnSelectedIndexPropertyChanged, s => s.SelectorController.CoerceSelectedIndex);

		public static readonly DependencyProperty SelectedItemProperty = DPM.Register<RadioMenuItem, RadioMenuItemGroup>
			("SelectedItem", s => s.SelectorController.OnSelectedItemPropertyChanged, s => s.SelectorController.CoerceSelectedItem);

		public static readonly DependencyProperty SelectedSourceProperty = DPM.Register<object, RadioMenuItemGroup>
			("SelectedSource", s => s.SelectorController.OnSelectedSourcePropertyChanged, s => s.SelectorController.CoerceSelectedSource);

		public static readonly DependencyProperty SelectedValueProperty = DPM.Register<object, RadioMenuItemGroup>
			("SelectedValue", s => s.SelectorController.OnSelectedValuePropertyChanged, s => s.SelectorController.CoerceSelectedValue);

		public static readonly DependencyProperty SelectedValueMemberPathProperty = DPM.Register<string, RadioMenuItemGroup>
			("SelectedValueMemberPath", s => s.OnSelectedValuePathChanged);

		public static readonly DependencyProperty SelectedValueSourceProperty = DPM.Register<SelectedValueSource, RadioMenuItemGroup>
			("SelectedValueSource", SelectedValueSource.Auto, s => s.OnSelectedValueSourceChanged);

		public static readonly DependencyProperty ItemGeneratorProperty = DPM.Register<RadioMenuItemGeneratorBase, RadioMenuItemGroup>
			("ItemGenerator", g => g.OnItemGeneratorChanged);

		#endregion

		#region Fields

		private byte _packedValue;
		private MemberValueEvaluator _selectedValueEvaluator;

		#endregion

		#region Ctors

		static RadioMenuItemGroup()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<RadioMenuItemGroup>();
		}

		public RadioMenuItemGroup()
		{
			this.OverrideStyleKey<RadioMenuItemGroup>();

			Items = new RadioMenuItemCollection(this);
			MenuItemsPresenter = new RadioMenuItemsPresenter
			{
				Items = Items,
				ActualOrientation = Orientation.Vertical
			};

			SelectorController = new SelectorController<RadioMenuItemGroup, RadioMenuItem>(this, new RadioMenuItemGroupSelectorAdvisor(this))
			{
				AllowNullSelection = false
			};
		}

		#endregion

		#region Properties

		internal bool IsInitializing
		{
			get => PackedDefinition.IsInitializing.GetValue(_packedValue);
			private set => PackedDefinition.IsInitializing.SetValue(ref _packedValue, value);
		}

		public RadioMenuItemGeneratorBase ItemGenerator
		{
			get => (RadioMenuItemGeneratorBase) GetValue(ItemGeneratorProperty);
			set => SetValue(ItemGeneratorProperty, value);
		}

		public RadioMenuItemCollection Items
		{
			get => (RadioMenuItemCollection) GetValue(ItemsProperty);
			private set => this.SetReadOnlyValue(ItemsPropertyKey, value);
		}

		internal override IMenuItemCollection ItemsCore => Items;

		protected override MenuItemsPresenterBase<RadioMenuItem, RadioMenuItemsPanel> MenuItemsPresenter { get; }

		public int SelectedIndex
		{
			get => (int) GetValue(SelectedIndexProperty);
			set => SetValue(SelectedIndexProperty, value);
		}

		public RadioMenuItem SelectedItem
		{
			get => (RadioMenuItem) GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}

		public object SelectedSource
		{
			get => GetValue(SelectedSourceProperty);
			set => SetValue(SelectedSourceProperty, value);
		}

		public object SelectedValue
		{
			get => GetValue(SelectedValueProperty);
			set => SetValue(SelectedValueProperty, value);
		}

		public string SelectedValueMemberPath
		{
			get => (string) GetValue(SelectedValueMemberPathProperty);
			set => SetValue(SelectedValueMemberPathProperty, value);
		}

		public SelectedValueSource SelectedValueSource
		{
			get => (SelectedValueSource) GetValue(SelectedValueSourceProperty);
			set => SetValue(SelectedValueSourceProperty, value);
		}

		private SelectorController<RadioMenuItemGroup, RadioMenuItem> SelectorController { get; }

		#endregion

		#region  Methods

		private void EnsureSelection()
		{
			if (IsInitializing == false)
				SelectorController.EnsureSelection();
		}

		private object GetItemValue(object item)
		{
			try
			{
				return _selectedValueEvaluator.GetValue(item);
			}
			catch (Exception e)
			{
				LogService.LogError(e);
			}

			return null;
		}

		internal override void OnDisplayMemberPathChangedInternal(string oldDisplayMemberPath, string newDisplayMemberPath)
		{
			base.OnDisplayMemberPathChangedInternal(oldDisplayMemberPath, newDisplayMemberPath);
			Items.DefaultRadioGenerator.DisplayMember = newDisplayMemberPath;
		}

		private void OnItemGeneratorChanged(RadioMenuItemGeneratorBase oldGenerator, RadioMenuItemGeneratorBase newGenerator)
		{
			Items.Generator = newGenerator;
		}

		protected override void OnMenuItemAdded(MenuItemBase menuItem)
		{
			base.OnMenuItemAdded(menuItem);
			EnsureSelection();
		}

		protected override void OnMenuItemRemoved(MenuItemBase menuItem)
		{
			base.OnMenuItemRemoved(menuItem);
			EnsureSelection();
		}

		private void OnSelectedValuePathChanged(string oldValue, string newValue)
		{
			try
			{
				_selectedValueEvaluator = new MemberValueEvaluator(newValue);
			}
			catch (Exception ex)
			{
				LogService.LogError(ex);
			}

			SelectorController.SyncValue();
		}

		private void OnSelectedValueSourceChanged()
		{
			SelectorController.SyncValue();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();
			UpdateRadioGroup();
		}

		private void UpdateRadioGroup()
		{
			var radioGroup = RadioMenuItem.GetRadioGroup(this);
			if (radioGroup != null)
				radioGroup.CurrentRadio = SelectedItem;
		}

		#endregion

		#region Interface Implementations

		#region ISelector<RadioMenuItem>

		DependencyProperty ISelector<RadioMenuItem>.SelectedIndexProperty => SelectedIndexProperty;

		DependencyProperty ISelector<RadioMenuItem>.SelectedItemProperty => SelectedItemProperty;

		DependencyProperty ISelector<RadioMenuItem>.SelectedSourceProperty => SelectedSourceProperty;

		DependencyProperty ISelector<RadioMenuItem>.SelectedValueProperty => SelectedValueProperty;

		object ISelector<RadioMenuItem>.GetValue(RadioMenuItem item, object source)
		{
			switch (SelectedValueSource)
			{
				case SelectedValueSource.Auto:
					return GetItemValue(Items.SourceInternal == null ? item : source);
				case SelectedValueSource.Item:
					return GetItemValue(item);
				case SelectedValueSource.Source:
					return GetItemValue(source);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		void ISelector<RadioMenuItem>.OnSelectedIndexChanged(int oldIndex, int newIndex)
		{
		}

		void ISelector<RadioMenuItem>.OnSelectedItemChanged(RadioMenuItem oldItem, RadioMenuItem newItem)
		{
		}

		void ISelector<RadioMenuItem>.OnSelectedSourceChanged(object oldSource, object newSource)
		{
		}

		void ISelector<RadioMenuItem>.OnSelectedValueChanged(object oldValue, object newValue)
		{
		}

		void ISelector<RadioMenuItem>.OnSelectionChanged(Selection<RadioMenuItem> oldSelection, Selection<RadioMenuItem> newSelection)
		{
			UpdateRadioGroup();
		}

		#endregion

		#region ISupportInitialize

		void ISupportInitialize.BeginInit()
		{
#if !SILVERLIGHT
			BeginInit();
#endif
			IsInitializing = true;
			SelectorController.BeginInit();
			SelectorController.AllowNullSelection = true;
			SelectorController.PreferSelection = false;
		}

		void ISupportInitialize.EndInit()
		{
			IsInitializing = false;
			SelectorController.EndInit();
			SelectorController.AllowNullSelection = false;
			SelectorController.PreferSelection = true;

#if !SILVERLIGHT
			EndInit();
#endif
		}

		#endregion

		#endregion

		#region  Nested Types

		private static class PackedDefinition
		{
			#region Static Fields and Constants

			public static readonly PackedBoolItemDefinition IsInitializing;

			#endregion

			#region Ctors

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				IsInitializing = allocator.AllocateBoolItem();
			}

			#endregion
		}

		#endregion
	}

	public class RadioMenuItemGroupTemplateContract : MenuItemGroupTemplateContractBase<RadioMenuItem, RadioMenuItemsPresenterHost, RadioMenuItemsPanel>
	{
		#region Properties

		[TemplateContractPart(Required = true)]
		public RadioMenuItemsPresenterHost ItemsPresenterHost { get; [UsedImplicitly] private set; }

		protected override RadioMenuItemsPresenterHost ItemsPresenterHostCore => ItemsPresenterHost;

		#endregion
	}

	internal sealed class RadioMenuItemGroupSelectorAdvisor : ItemCollectionSelectorAdvisor<Control, RadioMenuItem>
	{
		public RadioMenuItemGroupSelectorAdvisor(RadioMenuItemGroup radioMenuItemGroup) : base(radioMenuItemGroup, radioMenuItemGroup.Items)
		{
		}
		
		public override bool GetItemSelected(RadioMenuItem item)
		{
			return item.IsChecked == true;
		}

		public override void SetItemSelected(RadioMenuItem item, bool value)
		{
			item.SetCurrentValueInternal(ToggleMenuItem.IsCheckedProperty, value);
		}
	}
}