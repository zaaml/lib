using System.Windows;
using System.Windows.Input;

namespace Zaaml.PresentationCore.CommandCore
{
	internal static class CommandHelper
	{
		#region  Methods

		public static bool CanExecute(ICommand command, object commandParameter, DependencyObject commandTarget)
		{
			if (command == null)
				return false;

			var routedCommand = command as RoutedCommand;
#if SILVERLIGHT
			return routedCommand?.CanExecute(commandParameter, commandTarget) ?? command.CanExecute(commandParameter);
#else
			return routedCommand?.CanExecute(commandParameter, (IInputElement)commandTarget) ?? command.CanExecute(commandParameter);
#endif
		}

		public static void Execute(ICommand command, object commandParameter, DependencyObject commandTarget)
		{
			if (command == null) return;

			var targetCommand = command as RoutedCommand;
			if (targetCommand != null)
			{
#if SILVERLIGHT
				targetCommand.Execute(commandParameter, commandTarget);
#else
				targetCommand.Execute(commandParameter, (IInputElement)commandTarget);
#endif
			}
			else
				command.Execute(commandParameter);
		}

#endregion
	}
}