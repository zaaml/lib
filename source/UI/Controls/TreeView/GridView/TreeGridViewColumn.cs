// <copyright file="TreeGridViewColumn.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.TreeView
{
	public class TreeGridViewColumn : GridViewColumn
	{
		private static readonly DependencyPropertyKey ActualCellStylePropertyKey = DPM.RegisterReadOnly<Style, TreeGridViewColumn>
			("ActualCellStyle", d => d.OnActualCellStylePropertyChangedPrivate);

		public static readonly DependencyProperty ActualCellStyleProperty = ActualCellStylePropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ActualHeaderStylePropertyKey = DPM.RegisterReadOnly<Style, TreeGridViewColumn>
			("ActualHeaderStyle", d => d.OnActualHeaderStylePropertyChangedPrivate);

		public static readonly DependencyProperty ActualHeaderStyleProperty = ActualHeaderStylePropertyKey.DependencyProperty;

		public static readonly DependencyProperty HeaderProperty = DPM.Register<object, TreeGridViewColumn>
			("Header", d => d.OnHeaderPropertyChangedPrivate);

		public static readonly DependencyProperty MemberProperty = DPM.Register<string, TreeGridViewColumn>
			("Member", d => d.OnMemberPropertyChangedPrivate);

		public static readonly DependencyProperty CellStyleProperty = DPM.Register<Style, TreeGridViewColumn>
			("CellStyle", d => d.OnCellStylePropertyChangedPrivate);

		public static readonly DependencyProperty HeaderStyleProperty = DPM.Register<Style, TreeGridViewColumn>
			("HeaderStyle", d => d.OnHeaderStylePropertyChangedPrivate);

		public static readonly DependencyProperty CellEditModeTriggerProperty = DPM.Register<GridViewCellEditModeTrigger, TreeGridViewColumn>
			("CellEditModeTrigger", GridViewCellEditModeTrigger.Focus, d => d.OnCellEditModeTriggerPropertyChangedPrivate);

		public static readonly DependencyProperty CellDisplayContentTemplateProperty = DPM.Register<DataTemplate, TreeGridViewColumn>
			("CellDisplayContentTemplate", d => d.OnCellDisplayContentTemplatePropertyChangedPrivate);

		public static readonly DependencyProperty CellEditContentTemplateProperty = DPM.Register<DataTemplate, TreeGridViewColumn>
			("CellEditContentTemplate", d => d.OnCellEditContentTemplatePropertyChangedPrivate);

		public static readonly DependencyProperty CellGeneratorProperty = DPM.Register<TreeGridViewCellGeneratorBase, TreeGridViewColumn>
			("CellGenerator", d => d.OnCellGeneratorPropertyChangedPrivate);

		public static readonly DependencyProperty HeaderContentTemplateProperty = DPM.Register<DataTemplate, TreeGridViewColumn>
			("HeaderContentTemplate", d => d.OnHeaderContentTemplatePropertyChangedPrivate);

		public static readonly DependencyProperty HeaderGeneratorProperty = DPM.Register<TreeGridViewHeaderGeneratorBase, TreeGridViewColumn>
			("HeaderGenerator", d => d.OnHeaderGeneratorPropertyChangedPrivate);

		private static readonly Binding NullBinding = new() { Mode = BindingMode.OneTime, Source = null, BindsDirectlyToSource = true };
		private DefaultTreeGridViewCellGenerator _defaultCellGeneratorImpl;
		private DefaultTreeGridViewHeaderGenerator _defaultHeaderGeneratorImpl;

		private TreeGridView _view;
		internal event EventHandler ActualCellGeneratorChanged;
		internal event EventHandler ActualHeaderGeneratorChanged;

		internal TreeGridViewCellGeneratorBase ActualCellGenerator => CellGenerator ?? DefaultCellGenerator;

		public Style ActualCellStyle
		{
			get => (Style)GetValue(ActualCellStyleProperty);
			private set => this.SetReadOnlyValue(ActualCellStylePropertyKey, value);
		}

		internal TreeGridViewHeaderGeneratorBase ActualHeaderGenerator => HeaderGenerator ?? DefaultHeaderGenerator;

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

		public TreeGridViewCellGeneratorBase CellGenerator
		{
			get => (TreeGridViewCellGeneratorBase)GetValue(CellGeneratorProperty);
			set => SetValue(CellGeneratorProperty, value);
		}

		public Style CellStyle
		{
			get => (Style)GetValue(CellStyleProperty);
			set => SetValue(CellStyleProperty, value);
		}

		public override GridViewColumnController ColumnController => View?.ViewController?.ColumnController;

		private TreeGridViewHeadersPresenter ColumnHeadersPresenter => View?.TreeViewControl?.ColumnHeadersPresenterInternal;

		private DefaultTreeGridViewCellGenerator DefaultCellGenerator => _defaultCellGeneratorImpl ??= new DefaultTreeGridViewCellGenerator(this);

		private DefaultTreeGridViewHeaderGenerator DefaultHeaderGenerator => _defaultHeaderGeneratorImpl ??= new DefaultTreeGridViewHeaderGenerator(this);

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

		public TreeGridViewHeaderGeneratorBase HeaderGenerator
		{
			get => (TreeGridViewHeaderGeneratorBase)GetValue(HeaderGeneratorProperty);
			set => SetValue(HeaderGeneratorProperty, value);
		}

		public Style HeaderStyle
		{
			get => (Style)GetValue(HeaderStyleProperty);
			set => SetValue(HeaderStyleProperty, value);
		}

		internal bool IsExpanderColumn { get; set; }

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

		public TreeGridView View
		{
			get => _view;
			internal set
			{
				if (ReferenceEquals(_view, value))
					return;

				_view = value;

				_view?.UpdateActualExpanderColumn();

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

		protected virtual void OnCellAttached(TreeGridViewCell cell)
		{
		}

		private protected override void OnCellAttachedCore(GridViewCell cellCore)
		{
			base.OnCellAttachedCore(cellCore);

			if (cellCore is TreeGridViewCell cell)
				OnCellAttached(cell);
			else if (cellCore is TreeGridViewHeader header)
				OnHeaderAttached(header);
		}

		protected virtual void OnCellDetached(TreeGridViewCell cell)
		{
		}

		private protected override void OnCellDetachedCore(GridViewCell cellCore)
		{
			if (cellCore is TreeGridViewCell cell)
				OnCellDetached(cell);
			else if (cellCore is TreeGridViewHeader header)
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

		private void OnCellGeneratorPropertyChangedPrivate(TreeGridViewCellGeneratorBase oldValue, TreeGridViewCellGeneratorBase newValue)
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

		protected virtual void OnHeaderAttached(TreeGridViewHeader header)
		{
		}

		private void OnHeaderContentTemplatePropertyChangedPrivate(DataTemplate oldValue, DataTemplate newValue)
		{
			DefaultHeaderGenerator.TemplateProperty.Value = newValue;

			OnHeaderStructurePropertyChanged();
		}

		protected virtual void OnHeaderDetached(TreeGridViewHeader header)
		{
		}

		private void OnHeaderGeneratorPropertyChangedPrivate(TreeGridViewHeaderGeneratorBase oldValue, TreeGridViewHeaderGeneratorBase newValue)
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
			ColumnHeadersPresenter?.UpdateStructure();
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