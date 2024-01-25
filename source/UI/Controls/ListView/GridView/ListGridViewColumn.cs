// <copyright file="ListGridViewColumn.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.ListView
{
	public class ListGridViewColumn : GridViewColumn
	{
		private static readonly DependencyPropertyKey ActualCellStylePropertyKey = DPM.RegisterReadOnly<Style, ListGridViewColumn>
			("ActualCellStyle", d => d.OnActualCellStylePropertyChangedPrivate);

		public static readonly DependencyProperty ActualCellStyleProperty = ActualCellStylePropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ActualHeaderStylePropertyKey = DPM.RegisterReadOnly<Style, ListGridViewColumn>
			("ActualHeaderStyle", d => d.OnActualHeaderStylePropertyChangedPrivate);

		public static readonly DependencyProperty ActualHeaderStyleProperty = ActualHeaderStylePropertyKey.DependencyProperty;

		public static readonly DependencyProperty HeaderProperty = DPM.Register<object, ListGridViewColumn>
			("Header", d => d.OnHeaderPropertyChangedPrivate);

		public static readonly DependencyProperty MemberProperty = DPM.Register<string, ListGridViewColumn>
			("Member", d => d.OnMemberPropertyChangedPrivate);

		public static readonly DependencyProperty CellStyleProperty = DPM.Register<Style, ListGridViewColumn>
			("CellStyle", d => d.OnCellStylePropertyChangedPrivate);

		public static readonly DependencyProperty HeaderStyleProperty = DPM.Register<Style, ListGridViewColumn>
			("HeaderStyle", d => d.OnHeaderStylePropertyChangedPrivate);

		public static readonly DependencyProperty CellEditModeTriggerProperty = DPM.Register<GridViewCellEditModeTrigger, ListGridViewColumn>
			("CellEditModeTrigger", GridViewCellEditModeTrigger.Focus, d => d.OnCellEditModeTriggerPropertyChangedPrivate);
		
		public static readonly DependencyProperty CellDisplayContentTemplateProperty = DPM.Register<DataTemplate, ListGridViewColumn>
			("CellDisplayContentTemplate", d => d.OnCellDisplayContentTemplatePropertyChangedPrivate);

		public static readonly DependencyProperty CellEditContentTemplateProperty = DPM.Register<DataTemplate, ListGridViewColumn>
			("CellEditContentTemplate", d => d.OnCellEditContentTemplatePropertyChangedPrivate);

		public static readonly DependencyProperty CellGeneratorProperty = DPM.Register<ListGridViewCellGeneratorBase, ListGridViewColumn>
			("CellGenerator", d => d.OnCellGeneratorPropertyChangedPrivate);

		public static readonly DependencyProperty HeaderContentTemplateProperty = DPM.Register<DataTemplate, ListGridViewColumn>
			("HeaderContentTemplate", d => d.OnHeaderContentTemplatePropertyChangedPrivate);

		public static readonly DependencyProperty HeaderGeneratorProperty = DPM.Register<ListGridViewHeaderGeneratorBase, ListGridViewColumn>
			("HeaderGenerator", d => d.OnHeaderGeneratorPropertyChangedPrivate);
		
		private static readonly Binding NullBinding = new() { Mode = BindingMode.OneTime, Source = null, BindsDirectlyToSource = true };
		private DefaultListGridViewCellGenerator _defaultCellGeneratorImpl;
		private DefaultListGridViewHeaderGenerator _defaultHeaderGeneratorImpl;
		private ListGridView _view;

		internal event EventHandler ActualCellGeneratorChanged;
		internal event EventHandler ActualHeaderGeneratorChanged;

		internal ListGridViewCellGeneratorBase ActualCellGenerator => CellGenerator ?? DefaultCellGenerator;

		public Style ActualCellStyle
		{
			get => (Style)GetValue(ActualCellStyleProperty);
			private set => this.SetReadOnlyValue(ActualCellStylePropertyKey, value);
		}

		internal ListGridViewHeaderGeneratorBase ActualHeaderGenerator => HeaderGenerator ?? DefaultHeaderGenerator;

		public Style ActualHeaderStyle
		{
			get => (Style)GetValue(ActualHeaderStyleProperty);
			private set => this.SetReadOnlyValue(ActualHeaderStylePropertyKey, value);
		}

		public DataTemplate CellDisplayContentTemplate
		{
			get => (DataTemplate)GetValue(CellDisplayContentTemplateProperty);
			set => SetValue(CellDisplayContentTemplateProperty, value);
		}

		public DataTemplate CellEditContentTemplate
		{
			get => (DataTemplate)GetValue(CellEditContentTemplateProperty);
			set => SetValue(CellEditContentTemplateProperty, value);
		}

		public GridViewCellEditModeTrigger CellEditModeTrigger
		{
			get => (GridViewCellEditModeTrigger)GetValue(CellEditModeTriggerProperty);
			set => SetValue(CellEditModeTriggerProperty, value);
		}

		public ListGridViewCellGeneratorBase CellGenerator
		{
			get => (ListGridViewCellGeneratorBase)GetValue(CellGeneratorProperty);
			set => SetValue(CellGeneratorProperty, value);
		}

		public Style CellStyle
		{
			get => (Style)GetValue(CellStyleProperty);
			set => SetValue(CellStyleProperty, value);
		}

		public override GridViewColumnController ColumnController => View?.ViewController?.ColumnController;

		private DefaultListGridViewCellGenerator DefaultCellGenerator => _defaultCellGeneratorImpl ??= new DefaultListGridViewCellGenerator(this);

		private DefaultListGridViewHeaderGenerator DefaultHeaderGenerator => _defaultHeaderGeneratorImpl ??= new DefaultListGridViewHeaderGenerator(this);

		public object Header
		{
			get => GetValue(HeaderProperty);
			set => SetValue(HeaderProperty, value);
		}

		public DataTemplate HeaderContentTemplate
		{
			get => (DataTemplate)GetValue(HeaderContentTemplateProperty);
			set => SetValue(HeaderContentTemplateProperty, value);
		}

		public ListGridViewHeaderGeneratorBase HeaderGenerator
		{
			get => (ListGridViewHeaderGeneratorBase)GetValue(HeaderGeneratorProperty);
			set => SetValue(HeaderGeneratorProperty, value);
		}

		private ListGridViewHeadersPresenter HeadersPresenter => View?.ListViewControl?.GridViewHeadersPresenterInternal;

		public Style HeaderStyle
		{
			get => (Style)GetValue(HeaderStyleProperty);
			set => SetValue(HeaderStyleProperty, value);
		}

		public string Member
		{
			get => (string)GetValue(MemberProperty);
			set => SetValue(MemberProperty, value);
		}

		public Binding MemberBinding
		{
			get => DefaultCellGenerator.MemberProperty.MemberBinding;
			set => DefaultCellGenerator.MemberProperty.MemberBinding = value;
		}

		public ListGridView View
		{
			get => _view;
			internal set
			{
				if (ReferenceEquals(_view, value))
					return;

				_view = value;

				OnViewChanged();
			}
		}

		private void OnActualCellStylePropertyChangedPrivate(Style oldValue, Style newValue)
		{
			DefaultCellGenerator.StyleProperty.Value = newValue;

			OnCellStructurePropertyChanged();
		}

		private void OnActualHeaderStylePropertyChangedPrivate(Style oldValue, Style newValue)
		{
			DefaultHeaderGenerator.StyleProperty.Value = newValue;

			OnHeaderStructurePropertyChanged();
		}

		protected virtual void OnCellAttached(ListGridViewCell cell)
		{
		}

		private protected override void OnCellAttachedCore(GridViewCell cellCore)
		{
			base.OnCellAttachedCore(cellCore);

			if (cellCore is ListGridViewCell cell)
				OnCellAttached(cell);
			else if (cellCore is ListGridViewHeader header)
				OnHeaderAttached(header);
		}

		protected virtual void OnCellDetached(ListGridViewCell cell)
		{
		}

		private protected override void OnCellDetachedCore(GridViewCell cellCore)
		{
			if (cellCore is ListGridViewCell cell)
				OnCellDetached(cell);
			else if (cellCore is ListGridViewHeader header)
				OnHeaderDetached(header);

			base.OnCellDetachedCore(cellCore);
		}

		private void OnCellDisplayContentTemplatePropertyChangedPrivate(DataTemplate oldValue, DataTemplate newValue)
		{
			DefaultCellGenerator.DisplayTemplateProperty.Value = newValue;

			OnCellStructurePropertyChanged();
		}

		private void OnCellEditContentTemplatePropertyChangedPrivate(DataTemplate oldValue, DataTemplate newValue)
		{
			DefaultCellGenerator.EditTemplateProperty.Value = newValue;

			OnCellStructurePropertyChanged();
		}

		private void OnCellEditModeTriggerPropertyChangedPrivate(GridViewCellEditModeTrigger oldValue, GridViewCellEditModeTrigger newValue)
		{
			DefaultCellGenerator.EditModeTriggerProperty.Value = newValue;

			OnCellStructurePropertyChanged();
		}

		private void OnCellGeneratorPropertyChangedPrivate(ListGridViewCellGeneratorBase oldValue, ListGridViewCellGeneratorBase newValue)
		{
			ActualCellGeneratorChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnCellStructurePropertyChanged()
		{
			ColumnController?.OnCellStructurePropertyChanged(this);
		}

		private void OnCellStylePropertyChangedPrivate(Style oldValue, Style newValue)
		{
			UpdateActualCellStyle();
		}

		protected virtual void OnHeaderAttached(ListGridViewHeader header)
		{
		}

		private void OnHeaderContentTemplatePropertyChangedPrivate(DataTemplate oldValue, DataTemplate newValue)
		{
			DefaultHeaderGenerator.TemplateProperty.Value = newValue;

			OnHeaderStructurePropertyChanged();
		}

		protected virtual void OnHeaderDetached(ListGridViewHeader header)
		{
		}

		private void OnHeaderGeneratorPropertyChangedPrivate(ListGridViewHeaderGeneratorBase oldValue, ListGridViewHeaderGeneratorBase newValue)
		{
			ActualHeaderGeneratorChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnHeaderPropertyChangedPrivate(object oldValue, object newValue)
		{
			DefaultHeaderGenerator.MemberProperty.MemberBinding = Header != null ? new Binding { Source = Header, BindsDirectlyToSource = true } : NullBinding;

			OnHeaderStructurePropertyChanged();
		}

		private void OnHeaderStructurePropertyChanged()
		{
			HeadersPresenter?.UpdateStructure();
		}

		private void OnHeaderStylePropertyChangedPrivate(Style oldValue, Style newValue)
		{
			UpdateActualHeaderStyle();
		}

		private void OnMemberPropertyChangedPrivate(string oldValue, string newValue)
		{
			DefaultCellGenerator.MemberProperty.Member = newValue;

			OnCellStructurePropertyChanged();
		}

		private void OnViewChanged()
		{
			UpdateActualCellStyle();
			UpdateActualHeaderStyle();
		}

		internal void UpdateActualCellStyle()
		{
			ActualCellStyle = CellStyle ?? View?.CellStyle;
		}

		internal void UpdateActualHeaderStyle()
		{
			ActualHeaderStyle = HeaderStyle ?? View?.HeaderStyle;
		}
	}
}