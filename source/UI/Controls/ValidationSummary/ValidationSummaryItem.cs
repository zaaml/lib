// <copyright file="ValidationSummaryItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;
using ZaamlLocalization = Zaaml.UI.Localization;

namespace Zaaml.UI.Controls.ValidationSummary
{
	public class ValidationSummaryItem : InheritanceContextObject
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty HeaderProperty = DPM.Register<string, ValidationSummaryItem>
			("Header");

		public static readonly DependencyProperty IsActiveProperty = DPM.Register<bool, ValidationSummaryItem>
			("IsActive", true, v => v.OnIsActiveChanged);

		public static readonly DependencyProperty ItemTypeProperty = DPM.Register<ValidationSummaryItemType, ValidationSummaryItem>
			("ItemType", v => v.OnItemTypeChanged);

		public static readonly DependencyProperty MessageProperty = DPM.Register<string, ValidationSummaryItem>
			("Message", v => v.OnMessageChanged);

		#endregion

		#region Fields

		internal event EventHandler DisplayRelatedPropertyChanged;

		#endregion

		#region Ctors

		public ValidationSummaryItem()
			: this(null)
		{
		}

		public ValidationSummaryItem(string message)
			: this(message, null, ValidationSummaryItemType.ObjectError, null)
		{
		}

		public ValidationSummaryItem(string message, string header, ValidationSummaryItemType itemType, ValidationSummarySource source)
		{
			Header = header;
			Message = message;
			ItemType = itemType;
			Source = source;
		}

		#endregion

		#region Properties

		public string Header
		{
			get => (string) GetValue(HeaderProperty);
			set => SetValue(HeaderProperty, value);
		}

		public bool IsActive
		{
			get => (bool) GetValue(IsActiveProperty);
			set => SetValue(IsActiveProperty, value);
		}

		public ValidationSummaryItemType ItemType
		{
			get => (ValidationSummaryItemType) GetValue(ItemTypeProperty);
			set => SetValue(ItemTypeProperty, value);
		}

		public string Message
		{
			get => (string) GetValue(MessageProperty);
			set => SetValue(MessageProperty, value);
		}

		internal int ReferenceCount { get; set; }

		public ValidationSummarySource Source { get; }

		#endregion

		#region  Methods

		protected virtual void OnDisplayRelatedPropertyChanged()
		{
			DisplayRelatedPropertyChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnIsActiveChanged()
		{
			OnDisplayRelatedPropertyChanged();
		}

		private void OnItemTypeChanged()
		{
			OnDisplayRelatedPropertyChanged();
		}

		private void OnMessageChanged()
		{
			OnDisplayRelatedPropertyChanged();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, ZaamlLocalization.ValidationSummaryItem, Header, Message);
		}

		#endregion
	}
}
