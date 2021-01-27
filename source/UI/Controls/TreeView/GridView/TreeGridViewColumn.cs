// <copyright file="TreeGridViewColumn.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	public class TreeGridViewColumn : GridColumn
	{
		public static readonly DependencyProperty HeaderProperty = DPM.Register<object, TreeGridViewColumn>
			("Header", default, d => d.OnHeaderPropertyChangedPrivate);

		public static readonly DependencyProperty MemberProperty = DPM.Register<string, TreeGridViewColumn>
			("Member", default, d => d.OnMemberPropertyChangedPrivate);

		public static readonly DependencyProperty HeaderContentTemplateProperty = DPM.Register<DataTemplate, TreeGridViewColumn>
			("HeaderContentTemplate", d => d.OnHeaderContentTemplatePropertyChangedPrivate);

		public static readonly DependencyProperty CellDisplayContentTemplateProperty = DPM.Register<DataTemplate, TreeGridViewColumn>
			("CellDisplayContentTemplate", d => d.OnCellDisplayContentTemplatePropertyChangedPrivate);

		public static readonly DependencyProperty HeaderPaddingProperty = DPM.Register<Thickness, TreeGridViewColumn>
			("HeaderPadding", d => d.OnHeaderPaddingPropertyChangedPrivate);

		public static readonly DependencyProperty CellPaddingProperty = DPM.Register<Thickness, TreeGridViewColumn>
			("CellPadding", d => d.OnCellPaddingPropertyChangedPrivate);

		private static readonly Binding NullBinding = new Binding {Mode = BindingMode.OneTime, Source = null, BindsDirectlyToSource = true};
		private TreeGridView _treeGridView;

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

		private TreeViewItemGridColumnHeadersPresenter ColumnHeadersPresenter => TreeGridView?.TreeViewControl?.ColumnHeadersPresenterInternal;

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

		internal bool IsExpanderColumn { get; set; }

		public string Member
		{
			get => (string) GetValue(MemberProperty);
			set => SetValue(MemberProperty, value);
		}

		internal TreeGridView TreeGridView
		{
			get => _treeGridView;
			set
			{
				if (ReferenceEquals(_treeGridView, value))
					return;

				_treeGridView = value;
				
				_treeGridView?.UpdateActualExpanderColumn();
			}
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