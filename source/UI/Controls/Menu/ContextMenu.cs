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
	public enum DataContextMode
	{
		Default,
		Target,
		Owner
	}

	[TemplateContractType(typeof(ContextMenuTemplateContract))]
	public partial class ContextMenu : PopupMenu, IContextPopupControlInternal
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty DataContextModeProperty = DPM.Register<DataContextMode, ContextMenu>
			("DataContextMode", DataContextMode.Default, c => c.OnDataContextSourceChanged);

		private static readonly DependencyPropertyKey TargetPropertyKey = DPM.RegisterReadOnly<DependencyObject, ContextMenu>
			("Target");

		public static readonly DependencyProperty TargetProperty = TargetPropertyKey.DependencyProperty;

		#endregion

		#region Fields

		private bool _isShared;

		#endregion

		#region Ctors

		static ContextMenu()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ContextMenu>();
		}

		public ContextMenu()
		{
			Owners = new SharedItemOwnerCollection(this);

			this.OverrideStyleKey<ContextMenu>();
		}

		#endregion

		#region Properties

		public DataContextMode DataContextMode
		{
			get => (DataContextMode) GetValue(DataContextModeProperty);
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

		private SharedItemOwnerCollection Owners { get; }

		public DependencyObject Target
		{
			get => (DependencyObject) GetValue(TargetProperty);
			private set => this.SetReadOnlyValue(TargetPropertyKey, value);
		}

		#endregion

		#region  Methods

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

		internal virtual bool OwnerAttachSelector => true;

		#endregion

		#region Interface Implementations

		#region IContextPopupControlInternal

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

		#endregion

		#region ISharedItem

		bool ISharedItem.IsShared
		{
			get => IsShared;
			set => IsShared = value;
		}

		SharedItemOwnerCollection ISharedItem.Owners => Owners;

		#endregion

		#endregion

		#region  Nested Types

		private class DataContextBinding : Binding
		{
		}

		#endregion
	}

	public class ContextMenuTemplateContract : PopupMenuTemplateContract
	{
	}
}