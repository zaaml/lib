// <copyright file="EditorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Editors.Core
{
	[TemplateContractType(typeof(EditorBaseTemplateContract))]
	public abstract class EditorBase : TemplateContractControl
	{
		private static readonly DependencyPropertyKey IsEditingPropertyKey = DPM.RegisterReadOnly<bool, EditorBase>
			("IsEditing", s => s.OnIsEditingPropertyChangedPrivate);

		public static readonly DependencyProperty IsReadOnlyProperty = DPM.Register<bool, EditorBase>
			("IsReadOnly");

		public static readonly DependencyProperty IsEditingProperty = IsEditingPropertyKey.DependencyProperty;

		public event EventHandler EditingStarted;
		public event EventHandler<EditingEndedEventArgs> EditingEnded;

		public bool IsEditing
		{
			get => (bool) GetValue(IsEditingProperty);
			private set => this.SetReadOnlyValue(IsEditingPropertyKey, value);
		}

		public bool IsReadOnly
		{
			get => (bool) GetValue(IsReadOnlyProperty);
			set => SetValue(IsReadOnlyProperty, value);
		}

		public bool BeginEdit()
		{
			return BeginEditCore();
		}

		private protected bool BeginEditCore()
		{
			if (IsEditing)
				return false;

			IsEditing = true;

			OnBeginEdit();

			if (IsEditing == false)
				return false;

			EnterEditState();

			return IsEditing;
		}

		public bool CancelEdit()
		{
			return CancelEditCore();
		}

		private protected bool CancelEditCore(bool force = false)
		{
			if (IsEditing == false)
				return false;

			var cancelResult = OnCancelEdit();

			if (cancelResult == false && force == false)
				return false;

			IsEditing = false;

			OnEndEdit(EditingEndedEventArgs.CancelArgs);

			if (IsEditing)
				return false;

			LeaveEditState();

			return IsEditing == false;
		}

		public bool CommitEdit()
		{
			return CommitEditCore();
		}

		private protected bool CommitEditCore(bool force = false)
		{
			if (IsEditing == false)
				return false;

			var commitResult = OnCommitEdit();

			if (commitResult == false && force == false)
				return false;

			IsEditing = false;

			OnEndEdit(EditingEndedEventArgs.CommitArgs);

			if (IsEditing)
				return false;

			LeaveEditState();

			return IsEditing == false;
		}

		private protected virtual void EnterEditState()
		{
		}

		private protected virtual void LeaveEditState()
		{
		}

		protected virtual void OnBeginEdit()
		{
			EditingStarted?.Invoke(this, EventArgs.Empty);
		}

		protected virtual bool OnCancelEdit()
		{
			return true;
		}

		protected virtual bool OnCommitEdit()
		{
			return true;
		}

		protected virtual void OnEndEdit(EditingEndedEventArgs args)
		{
			EditingEnded?.Invoke(this, args);
		}

		protected virtual void OnIsEditingChanged()
		{
		}

		private void OnIsEditingPropertyChangedPrivate()
		{
			OnIsEditingChanged();

			UpdateVisualState(true);
		}
	}
}