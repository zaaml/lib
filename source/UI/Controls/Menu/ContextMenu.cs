// <copyright file="ContextMenu.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.Menu
{
	[TemplateContractType(typeof(ContextMenuTemplateContract))]
	public partial class ContextMenu : PopupMenu, IContextPopupControlInternal
	{
		public static readonly DependencyProperty DataContextModeProperty = DPM.Register<DataContextMode, ContextMenu>
			("DataContextMode", DataContextMode.Default, c => c.OnDataContextSourceChanged);

		private static readonly DependencyPropertyKey TargetPropertyKey = DPM.RegisterReadOnly<DependencyObject, ContextMenu>
			("Target");

		public static readonly DependencyProperty TargetProperty = TargetPropertyKey.DependencyProperty;

		private bool _isShared;

		static ContextMenu()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ContextMenu>();
		}

		public ContextMenu()
		{
			Owners = new SharedItemOwnerCollection(this);

			this.OverrideStyleKey<ContextMenu>();
		}

		public DataContextMode DataContextMode
		{
			get => (DataContextMode)GetValue(DataContextModeProperty);
			set => SetValue(DataContextModeProperty, value);
		}

		private bool IsShared
		{
			get => _isShared;
			set
			{
				if (_isShared == value)
					return;

				_isShared = value;

				OnIsSharedChanged();
			}
		}

		internal virtual bool OwnerAttachSelector => true;

		private SharedItemOwnerCollection Owners { get; }

		public DependencyObject Target
		{
			get => (DependencyObject)GetValue(TargetProperty);
			private set => this.SetReadOnlyValue(TargetPropertyKey, value);
		}

		private void OnDataContextSourceChanged()
		{
			var dataContextSource = DataContextMode;

			switch (dataContextSource)
			{
				case DataContextMode.Default:

					var dataContextBinding = this.ReadLocalBinding(DataContextProperty) as DataContextBinding;

					if (ReferenceEquals(dataContextBinding?.Source, this))
						ClearValue(DataContextProperty);

					break;
				case DataContextMode.Target:

					SetBinding(DataContextProperty, new DataContextBinding { Path = new PropertyPath("Target.DataContext"), Source = this, Mode = BindingMode.OneWay });

					break;
				default:

					SetBinding(DataContextProperty, new DataContextBinding { Path = new PropertyPath("Owner.DataContext"), Source = this, Mode = BindingMode.OneWay });

					break;
			}
		}

		private void OnIsSharedChanged()
		{
			PlatformOnIsSharedChanged();
		}

		partial void PlatformOnIsSharedChanged();

		FrameworkElement IContextPopupControlInternal.Owner
		{
			get => Owner;
			set => Owner = value;
		}

		DependencyObject IContextPopupControlInternal.Target
		{
			get => Target;
			set => Target = value;
		}

		bool IContextPopupControlInternal.OwnerAttachSelector => OwnerAttachSelector;

		bool ISharedItem.IsShared
		{
			get => IsShared;
			set => IsShared = value;
		}

		SharedItemOwnerCollection ISharedItem.Owners => Owners;

		private class DataContextBinding : Binding
		{
		}
	}

	public class ContextMenuTemplateContract : PopupMenuTemplateContract
	{
	}
}