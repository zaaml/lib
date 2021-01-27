// <copyright file="ListGridViewColumn.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	public class ListGridViewColumn : GridColumn
	{
		public static readonly DependencyProperty HeaderProperty = DPM.Register<object, ListGridViewColumn>
			("Header", default, d => d.OnHeaderPropertyChangedPrivate);

		public static readonly DependencyProperty MemberProperty = DPM.Register<string, ListGridViewColumn>
			("Member", default, d => d.OnMemberPropertyChangedPrivate);

		public static readonly DependencyProperty HeaderContentTemplateProperty = DPM.Register<DataTemplate, ListGridViewColumn>
			("HeaderContentTemplate", d => d.OnHeaderContentTemplatePropertyChangedPrivate);

		public static readonly DependencyProperty CellDisplayContentTemplateProperty = DPM.Register<DataTemplate, ListGridViewColumn>
			("CellDisplayContentTemplate", d => d.OnCellDisplayContentTemplatePropertyChangedPrivate);

		public static readonly DependencyProperty HeaderPaddingProperty = DPM.Register<Thickness, ListGridViewColumn>
			("HeaderPadding", d => d.OnHeaderPaddingPropertyChangedPrivate);

		public static readonly DependencyProperty CellPaddingProperty = DPM.Register<Thickness, ListGridViewColumn>
			("CellPadding", d => d.OnCellPaddingPropertyChangedPrivate);

		private static readonly Binding NullBinding = new Binding {Mode = BindingMode.OneTime, Source = null, BindsDirectlyToSource = true};

		internal Binding ActualHeaderBinding { get; set; }

		internal Binding ActualMemberBinding { get; set; }

		public DataTemplate CellDisplayContentTemplate
		{
			get => (DataTemplate) GetValue(CellDisplayContentTemplateProperty);
			set => SetValue(CellDisplayContentTemplateProperty, value);
		}

		public Thickness CellPadding
		{
			get => (Thickness) GetValue(CellPaddingProperty);
			set => SetValue(CellPaddingProperty, value);
		}

		private ListViewItemGridColumnHeadersPresenter ColumnHeadersPresenter => ListGridView?.ListViewControl?.ColumnHeadersPresenterInternal;

		public object Header
		{
			get => GetValue(HeaderProperty);
			set => SetValue(HeaderProperty, value);
		}

		public DataTemplate HeaderContentTemplate
		{
			get => (DataTemplate) GetValue(HeaderContentTemplateProperty);
			set => SetValue(HeaderContentTemplateProperty, value);
		}

		public Thickness HeaderPadding
		{
			get => (Thickness) GetValue(HeaderPaddingProperty);
			set => SetValue(HeaderPaddingProperty, value);
		}

		internal ListGridView ListGridView { get; set; }

		public string Member
		{
			get => (string) GetValue(MemberProperty);
			set => SetValue(MemberProperty, value);
		}

		private void OnCellDisplayContentTemplatePropertyChangedPrivate(DataTemplate oldValue, DataTemplate newValue)
		{
			OnCellStructurePropertyChanged();
		}

		private void OnCellPaddingPropertyChangedPrivate(Thickness oldValue, Thickness newValue)
		{
			OnCellStructurePropertyChanged();
		}

		private void OnCellStructurePropertyChanged()
		{
			Controller?.OnCellStructurePropertyChanged(this);
		}

		private void OnHeaderContentTemplatePropertyChangedPrivate(DataTemplate oldValue, DataTemplate newValue)
		{
			OnHeaderStructurePropertyChanged();
		}

		private void OnHeaderPaddingPropertyChangedPrivate(Thickness oldValue, Thickness newValue)
		{
			OnHeaderStructurePropertyChanged();
		}

		private void OnHeaderPropertyChangedPrivate(object oldValue, object newValue)
		{
			ActualHeaderBinding = Header != null ? new Binding {Source = Header, BindsDirectlyToSource = true} : NullBinding;
		}

		private void OnHeaderStructurePropertyChanged()
		{
			ColumnHeadersPresenter?.UpdateStructure();
		}

		private void OnMemberPropertyChangedPrivate(string oldValue, string newValue)
		{
			ActualMemberBinding = Member != null ? new Binding(Member) : NullBinding;
		}
	}
}