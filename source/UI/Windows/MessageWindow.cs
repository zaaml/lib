// <copyright file="MessageWindow.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Zaaml.Core;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Windows
{
	public sealed partial class MessageWindow : DialogWindowBase
	{
		private static readonly DependencyPropertyKey ActualMessageIconPropertyKey = DPM.RegisterReadOnly<ImageSource, MessageWindow>
			("ActualMessageIcon", w => w.OnActualMessageIconChanged);

		public static readonly DependencyProperty ActualMessageIconProperty = ActualMessageIconPropertyKey.DependencyProperty;

		private readonly List<MessageWindowButton> _actualButtons;
		private readonly ThemeResourceReference _messageIconResourceReference;
		private readonly Action<MessageWindowResult> _onMessageResult;

		static MessageWindow()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<MessageWindow>();
		}

		internal MessageWindow(MessageWindowOptions windowOptions, Action<MessageWindowResult> onMessageResult)
		{
			this.OverrideStyleKey<MessageWindow>();

			WindowOptions = windowOptions;

			_onMessageResult = onMessageResult;

			Message = windowOptions.MessageText;
			Title = windowOptions.Caption;
			IsResizable = false;

			DetachButtons();

			_actualButtons = windowOptions.Buttons switch
			{
				MessageWindowButtons.OK => [MessageWindowResultKind.OK.Create()],
				MessageWindowButtons.OKCancel => [MessageWindowResultKind.OK.Create(), MessageWindowResultKind.Cancel.Create()],
				MessageWindowButtons.Yes => [MessageWindowResultKind.Yes.Create()],
				MessageWindowButtons.YesNo => [MessageWindowResultKind.Yes.Create(), MessageWindowResultKind.No.Create()],
				MessageWindowButtons.YesNoCancel => [MessageWindowResultKind.Yes.Create(), MessageWindowResultKind.No.Create(), MessageWindowResultKind.Cancel.Create()],
				_ => throw new ArgumentOutOfRangeException()
			};

			AttachButtons();

			if (windowOptions.ImageSource != null)
				ActualMessageIcon = windowOptions.ImageSource;
			else
			{
				_messageIconResourceReference = ThemeManager.GetThemeResourceReference(ThemeKeyword.ApplicationBackgroundBrush);
				((INotifyPropertyChanged)_messageIconResourceReference).PropertyChanged += OnThemeResourceIconChanged;
			}

			Screen.VirtualScreenSizeChanged += OnSizeChanged;

			MoveWindowToCenterInternal();

			this.InvokeOnLayoutUpdate(() => { Focus(); });

			this.GetServiceOrCreate(() => new KeyboardHelperService()).Action += OnKeyboardAction;

			PlatformCtor();
		}

		protected override IEnumerable<WindowButton> ActualButtons => _actualButtons ?? Enumerable.Empty<WindowButton>();

		public ImageSource ActualMessageIcon
		{
			get => (ImageSource)GetValue(ActualMessageIconProperty);
			private set => this.SetReadOnlyValue(ActualMessageIconPropertyKey, value);
		}

		protected override bool ActualShowMaximizeButton => false;

		protected override bool ActualShowMinimizeButton => false;

		public string Message { get; }

		public MessageWindowOptions WindowOptions { get; }

		protected override Rect CalcClientBox(Size finalArrange)
		{
			var workingArea = Screen.FromElement(this).WorkingArea;
			var maxBox = new Size(workingArea.Width * 0.5, workingArea.Height * 0.8);

			Width = double.NaN;
			Height = double.NaN;

			Measure(XamlConstants.InfiniteSize);
			Measure(new Size(DesiredSize.Width.Clamp(0, maxBox.Width), double.PositiveInfinity));
			Measure(new Size(DesiredSize.Width, DesiredSize.Height.Clamp(0, maxBox.Height)));

			return DesiredSize.Rect();
		}

		protected override void CloseByEnter()
		{
			CloseImpl(GetDefaultOkResult(WindowOptions.Buttons));
		}

		protected override void CloseByEscape()
		{
			CloseImpl(GetDefaultCancelResult(WindowOptions.Buttons));
		}

		private void CloseImpl(MessageWindowResultKind resultKind)
		{
			_onMessageResult?.Invoke(new MessageWindowResult(resultKind, WindowOptions));

			Screen.VirtualScreenSizeChanged -= OnSizeChanged;

			if (_messageIconResourceReference != null)
				((INotifyPropertyChanged)_messageIconResourceReference).PropertyChanged -= OnThemeResourceIconChanged;

			Close();
		}

		private static MessageWindowResultKind GetDefaultCancelResult(MessageWindowButtons buttons)
		{
			return buttons switch
			{
				MessageWindowButtons.OK => MessageWindowResultKind.OK,
				MessageWindowButtons.OKCancel => MessageWindowResultKind.Cancel,
				MessageWindowButtons.Yes => MessageWindowResultKind.Yes,
				MessageWindowButtons.YesNo => MessageWindowResultKind.No,
				MessageWindowButtons.YesNoCancel => MessageWindowResultKind.Cancel,
				_ => MessageWindowResultKind.None
			};
		}

		private static MessageWindowResultKind GetDefaultOkResult(MessageWindowButtons buttons)
		{
			return buttons switch
			{
				MessageWindowButtons.OK => MessageWindowResultKind.OK,
				MessageWindowButtons.OKCancel => MessageWindowResultKind.OK,
				MessageWindowButtons.Yes => MessageWindowResultKind.Yes,
				MessageWindowButtons.YesNo => MessageWindowResultKind.Yes,
				MessageWindowButtons.YesNoCancel => MessageWindowResultKind.Yes,
				_ => MessageWindowResultKind.None
			};
		}

		private void OnActualMessageIconChanged()
		{
		}

		protected override void OnExecutedCloseCommand(object commandParameter)
		{
			var resultKind = WindowOptions.DefaultResult;

			if (commandParameter is MessageWindowResultKind kind)
				resultKind = kind;

			CloseImpl(resultKind);
		}

		private void OnKeyboardAction(object sender, KeyboardActionEventArgs keyboardActionEventArgs)
		{
			if (keyboardActionEventArgs.Action != KeyboardAction.Copy)
				return;

			try
			{
				Clipboard.SetText(string.Join(Environment.NewLine, WindowOptions.Caption, WindowOptions.MessageText));
			}
			catch (Exception ex)
			{
				LogService.LogError(ex);
			}
		}

		private void OnSizeChanged(object sender, EventArgs e)
		{
			MoveWindowToCenterInternal();
		}

		private void OnThemeResourceIconChanged(object sender, EventArgs eventArgs)
		{
			ActualMessageIcon = _messageIconResourceReference.Value as ImageSource;
		}

		partial void PlatformCtor();

		public static void Show(string message, Action<MessageWindowResult> onMessageResult = null)
		{
			ShowImpl(new MessageWindowOptions(message, string.Empty, MessageWindowButtons.OK, MessageBoxImage.None, MessageWindowResultKind.None), onMessageResult);
		}

		public static void Show(string message, string caption, Action<MessageWindowResult> onMessageResult = null)
		{
			ShowImpl(new MessageWindowOptions(message, caption, MessageWindowButtons.OK, MessageBoxImage.None, MessageWindowResultKind.None), onMessageResult);
		}

		public static void Show(string message, string caption, MessageWindowButtons buttons, Action<MessageWindowResult> onMessageResult = null)
		{
			ShowImpl(new MessageWindowOptions(message, caption, buttons, MessageBoxImage.None, MessageWindowResultKind.None), onMessageResult);
		}

		public static void Show(string message, string caption, MessageWindowButtons buttons, MessageBoxImage image, Action<MessageWindowResult> onMessageResult = null)
		{
			ShowImpl(new MessageWindowOptions(message, caption, buttons, image, MessageWindowResultKind.None), onMessageResult);
		}

		public static void Show(string message, string caption, MessageWindowButtons buttons, MessageBoxImage image, MessageWindowResultKind defaultResult,
			Action<MessageWindowResult> onMessageResult = null)
		{
			ShowImpl(new MessageWindowOptions(message, caption, buttons, image, defaultResult), onMessageResult);
		}

		public static void Show(string message, string caption, MessageWindowButtons buttons, ImageSource image, Action<MessageWindowResult> onMessageResult = null)
		{
			ShowImpl(new MessageWindowOptions(message, caption, buttons, image, MessageWindowResultKind.None), onMessageResult);
		}

		public static void Show(string message, string caption, MessageWindowButtons buttons, ImageSource image, MessageWindowResultKind defaultResult,
			Action<MessageWindowResult> onMessageResult = null)
		{
			ShowImpl(new MessageWindowOptions(message, caption, buttons, image, defaultResult), onMessageResult);
		}

		public static void Show(MessageWindowOptions options, Action<MessageWindowResult> onMessageResult = null)
		{
			ShowImpl(options, onMessageResult);
		}

		public static Task<MessageWindowResult> ShowAsync(string message)
		{
			var taskCompletionSource = new TaskCompletionSource<MessageWindowResult>();

			ShowImpl(new MessageWindowOptions(message, string.Empty, MessageWindowButtons.OK, MessageBoxImage.None, MessageWindowResultKind.None), OnMessageResult);

			return taskCompletionSource.Task;

			void OnMessageResult(MessageWindowResult r) => taskCompletionSource.SetResult(r);
		}

		public static Task<MessageWindowResult> ShowAsync(string message, string caption)
		{
			var taskCompletionSource = new TaskCompletionSource<MessageWindowResult>();

			ShowImpl(new MessageWindowOptions(message, caption, MessageWindowButtons.OK, MessageBoxImage.None, MessageWindowResultKind.None), OnMessageResult);

			return taskCompletionSource.Task;

			void OnMessageResult(MessageWindowResult r) => taskCompletionSource.SetResult(r);
		}

		public static Task<MessageWindowResult> ShowAsync(string message, string caption, MessageWindowButtons buttons)
		{
			var taskCompletionSource = new TaskCompletionSource<MessageWindowResult>();

			ShowImpl(new MessageWindowOptions(message, caption, buttons, MessageBoxImage.None, MessageWindowResultKind.None), OnMessageResult);

			return taskCompletionSource.Task;

			void OnMessageResult(MessageWindowResult r) => taskCompletionSource.SetResult(r);
		}

		public static Task<MessageWindowResult> ShowAsync(string message, string caption, MessageWindowButtons buttons, MessageBoxImage image)
		{
			var taskCompletionSource = new TaskCompletionSource<MessageWindowResult>();

			ShowImpl(new MessageWindowOptions(message, caption, buttons, image, MessageWindowResultKind.None), OnMessageResult);

			return taskCompletionSource.Task;

			void OnMessageResult(MessageWindowResult r) => taskCompletionSource.SetResult(r);
		}

		public static Task<MessageWindowResult> ShowAsync(string message, string caption, MessageWindowButtons buttons, MessageBoxImage image, MessageWindowResultKind defaultResult)
		{
			var taskCompletionSource = new TaskCompletionSource<MessageWindowResult>();

			ShowImpl(new MessageWindowOptions(message, caption, buttons, image, defaultResult), OnMessageResult);

			return taskCompletionSource.Task;

			void OnMessageResult(MessageWindowResult r) => taskCompletionSource.SetResult(r);
		}

		public static Task<MessageWindowResult> ShowAsync(string message, string caption, MessageWindowButtons buttons, ImageSource image)
		{
			var taskCompletionSource = new TaskCompletionSource<MessageWindowResult>();

			ShowImpl(new MessageWindowOptions(message, caption, buttons, image, MessageWindowResultKind.None), OnMessageResult);

			return taskCompletionSource.Task;

			void OnMessageResult(MessageWindowResult r) => taskCompletionSource.SetResult(r);
		}

		public static Task<MessageWindowResult> ShowAsync(string message, string caption, MessageWindowButtons buttons, ImageSource image, MessageWindowResultKind defaultResult)
		{
			var taskCompletionSource = new TaskCompletionSource<MessageWindowResult>();

			ShowImpl(new MessageWindowOptions(message, caption, buttons, image, defaultResult), OnMessageResult);

			return taskCompletionSource.Task;

			void OnMessageResult(MessageWindowResult r) => taskCompletionSource.SetResult(r);
		}

		public static Task<MessageWindowResult> ShowAsync(MessageWindowOptions options)
		{
			var taskCompletionSource = new TaskCompletionSource<MessageWindowResult>();

			ShowImpl(options, OnMessageResult);

			return taskCompletionSource.Task;

			void OnMessageResult(MessageWindowResult r) => taskCompletionSource.SetResult(r);
		}

		private static void ShowImpl(MessageWindowOptions windowOptions, Action<MessageWindowResult> onMessageResult)
		{
			var messageWindow = new MessageWindow(windowOptions, onMessageResult)
			{
				WindowStartupLocation = WindowStartupLocation.CenterScreen,
				SizeToContent = SizeToContent.Manual,
				HorizontalAlignment = HorizontalAlignment.Center,
			};

			messageWindow.ShowDialog();
		}

		internal override void ToggleMaximizeNormalState()
		{
		}
	}
}