// <copyright file="DropDownTreeViewEditor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.DropDown;
using Zaaml.UI.Controls.Editors.Core;
using Zaaml.UI.Controls.TreeView;

namespace Zaaml.UI.Controls.Editors.DropDown
{
	[TemplateContractType(typeof(DropDownTreeViewEditorTemplateContract))]
	public class DropDownTreeViewEditor : DropDownEditorBase
	{
		public static readonly DependencyProperty IsEditableProperty = DPM.Register<bool, DropDownTreeViewEditor>
			("IsEditable", false);

		public static readonly DependencyProperty TreeViewProperty = DPM.Register<TreeViewControl, DropDownTreeViewEditor>
			("TreeView");

		static DropDownTreeViewEditor()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<DropDownTreeViewEditor>();
		}

		public DropDownTreeViewEditor()
		{
			this.OverrideStyleKey<DropDownTreeViewEditor>();
		}

		private DropDownTreeViewControl DropDownTreeViewControl => TemplateContract.DropDownTreeViewControl;

		public bool IsEditable
		{
			get => (bool) GetValue(IsEditableProperty);
			set => SetValue(IsEditableProperty, value);
		}

		private DropDownTreeViewEditorTemplateContract TemplateContract => (DropDownTreeViewEditorTemplateContract) TemplateContractInternal;

		public TreeViewControl TreeView
		{
			get => (TreeViewControl) GetValue(TreeViewProperty);
			set => SetValue(TreeViewProperty, value);
		}

		private void DropDownTreeViewControlOnEditingEnded(object sender, EditingEndedEventArgs e)
		{
			if (e.Result == EditingResult.Cancel)
				CancelEdit();
			else
				CommitEdit();
		}

		private void DropDownTreeViewControlOnEditingStarted(object sender, EventArgs e)
		{
			BeginEdit();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			DropDownTreeViewControl.EditingStarted += DropDownTreeViewControlOnEditingStarted;
			DropDownTreeViewControl.EditingEnded += DropDownTreeViewControlOnEditingEnded;
		}

		protected override void OnTemplateContractDetaching()
		{
			DropDownTreeViewControl.EditingStarted -= DropDownTreeViewControlOnEditingStarted;
			DropDownTreeViewControl.EditingEnded -= DropDownTreeViewControlOnEditingEnded;

			base.OnTemplateContractDetaching();
		}
	}

	public class DropDownTreeViewEditorTemplateContract : DropDownEditorBaseTemplateContract
	{
		[TemplateContractPart(Required = true)]
		public DropDownTreeViewControl DropDownTreeViewControl { get; private set; }
	}
}