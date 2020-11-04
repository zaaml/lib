// <copyright file="InvokeCommand.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Input;
using Zaaml.Core;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class InvokeCommand : TriggerActionBase, IEventTriggerArgsSupport
	{
		#region Static Fields and Constants

		private static readonly InteractivityProperty CommandProperty = RegisterInteractivityProperty(OnCommandChanged);
		private static readonly InteractivityProperty CommandParameterProperty = RegisterInteractivityProperty(CommandParameterChanged);

		#endregion

		#region Fields

		private object _command;
		private object _commandParameter;
		private WeakReference _eventArgsStore;

		#endregion

		#region Properties

		public ICommand Command
		{
			get => GetValue(CommandProperty, ref _command) as ICommand;
			set => SetValue(CommandProperty, ref _command, value);
		}

		public object CommandParameter
		{
			get => GetValue(CommandParameterProperty, ref _commandParameter);
			set => SetValue(CommandParameterProperty, ref _commandParameter, value);
		}

		#endregion

		#region  Methods

		private static void CommandParameterChanged(InteractivityObject interactivityobject, object oldvalue, object newvalue)
		{
		}

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var sourceInvokeCommand = (InvokeCommand) source;

			CommandProperty.Copy(out _command, sourceInvokeCommand._command);
			CommandParameterProperty.Copy(out _commandParameter, sourceInvokeCommand._commandParameter);
		}

		protected override InteractivityObject CreateInstance()
		{
			return new InvokeCommand();
		}

		protected override void InvokeCore()
		{
			var command = Command;
			var parameter = CommandParameter;

			if (ReferenceEquals(CommandParameter, EventTriggerArgs.Instance))
				parameter = _eventArgsStore?.Target;

			if (command?.CanExecute(parameter) == true)
				command.Execute(parameter);
		}

		internal override void LoadCore(IInteractivityRoot root)
		{
			base.LoadCore(root);

			Load(CommandProperty, ref _command);
			Load(CommandParameterProperty, ref _commandParameter);
		}

		private static void OnCommandChanged(InteractivityObject interactivityobject, object oldvalue, object newvalue)
		{
		}

		[UsedImplicitly]
		internal void SetCommandParameterProperty(object value)
		{
			SetValue(CommandParameterProperty, ref _commandParameter, value);
		}

		[UsedImplicitly]
		internal void SetCommandProperty(object value)
		{
			SetValue(CommandProperty, ref _command, value);
		}

		internal override void UnloadCore(IInteractivityRoot root)
		{
			Unload(CommandProperty, ref _command);
			Unload(CommandParameterProperty, ref _commandParameter);

			base.UnloadCore(root);
		}

		#endregion

		#region Interface Implementations

		#region IEventTriggerArgsSupport

		void IEventTriggerArgsSupport.SetArgs(EventArgs args)
		{
			_eventArgsStore = args != null ? new WeakReference(args) : null;
		}

		#endregion

		#endregion
	}
}