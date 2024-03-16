// <copyright file="DropDownListViewEditor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.DropDown;
using Zaaml.UI.Controls.Editors.Core;
using Zaaml.UI.Controls.ListView;

namespace Zaaml.UI.Controls.Editors.DropDown
{
	[TemplateContractType(typeof(DropDownListViewEditorTemplateContract))]
	public class DropDownListViewEditor : DropDownEditorBase
	{
		public static readonly DependencyProperty IsTextEditableProperty = DPM.Register<bool, DropDownListViewEditor>
			("IsTextEditable", false);

		public static readonly DependencyProperty DisplayModeProperty = DPM.Register<DropDownEditableSelectorDisplayMode, DropDownListViewEditor>
			("DisplayMode", DropDownEditableSelectorDisplayMode.TextEditor);

		public static readonly DependencyProperty ListViewControlProperty = DPM.Register<ListViewControl, DropDownListViewEditor>
			("ListViewControl");

		private bool _suspendEditCommands;

		static DropDownListViewEditor()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<DropDownListViewEditor>();
		}

		public DropDownListViewEditor()
		{
			this.OverrideStyleKey<DropDownListViewEditor>();
		}

		public DropDownEditableSelectorDisplayMode DisplayMode
		{
			get => (DropDownEditableSelectorDisplayMode)GetValue(DisplayModeProperty);
			set => SetValue(DisplayModeProperty, value);
		}

		private DropDownListViewControl DropDownListViewControl => TemplateContract.DropDownListViewControl;

		public bool IsTextEditable
		{
			get => (bool)GetValue(IsTextEditableProperty);
			set => SetValue(IsTextEditableProperty, value.Box());
		}

		public ListViewControl ListViewControl
		{
			get => (ListViewControl)GetValue(ListViewControlProperty);
			set => SetValue(ListViewControlProperty, value);
		}

		private DropDownListViewEditorTemplateContract TemplateContract => (DropDownListViewEditorTemplateContract)TemplateContractCore;

		private protected override void EnterEditState()
		{
			base.EnterEditState();

			if (_suspendEditCommands)
				return;

			try
			{
				_suspendEditCommands = true;

				DropDownListViewControl.BeginEdit();
			}
			finally
			{
				_suspendEditCommands = false;
			}
		}

		private void OnDropDownListViewControlEditingEnded(object sender, EditingEndedEventArgs e)
		{
			try
			{
				_suspendEditCommands = true;

				if (e.Result == EditingResult.Cancel)
					CancelEdit();
				else
					CommitEdit();
			}
			finally
			{
				_suspendEditCommands = false;
			}
		}

		private void OnDropDownListViewControlEditingStarted(object sender, EventArgs e)
		{
			try
			{
				_suspendEditCommands = true;

				BeginEdit();
			}
			finally
			{
				_suspendEditCommands = false;
			}
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			DropDownListViewControl.EditingStarted += OnDropDownListViewControlEditingStarted;
			DropDownListViewControl.EditingEnded += OnDropDownListViewControlEditingEnded;

			DropDownListViewControl.ShouldHandleFocusNavigationKey = false;
		}

		protected override void OnTemplateContractDetaching()
		{
			DropDownListViewControl.EditingStarted -= OnDropDownListViewControlEditingStarted;
			DropDownListViewControl.EditingEnded -= OnDropDownListViewControlEditingEnded;

			base.OnTemplateContractDetaching();
		}
	}

	public class DropDownListViewEditorTemplateContract : DropDownEditorBaseTemplateContract
	{
		[TemplateContractPart(Required = true)]
		public DropDownListViewControl DropDownListViewControl { get; [UsedImplicitly] private set; }
	}
}