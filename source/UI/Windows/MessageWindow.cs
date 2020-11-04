// <copyright file="MessageWindow.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

#if SILVERLIGHT
using System.Windows.Media.Imaging;
#endif

namespace Zaaml.UI.Windows
{
  public sealed partial class MessageWindow : DialogWindowBase
  {
    #region Static Fields and Constants

    private static readonly DependencyPropertyKey ActualMessageIconPropertyKey = DPM.RegisterReadOnly<ImageSource, MessageWindow>
      ("ActualMessageIcon", w => w.OnActualMessageIconChanged);

    public static readonly DependencyProperty ActualMessageIconProperty = ActualMessageIconPropertyKey.DependencyProperty;

    #endregion

    #region Fields

    private readonly List<MessageWindowButton> _actualButtons;
    private readonly ThemeResourceReference _messageIconResourceReference;
    private readonly Action<MessageWindowResult> _onMessageResult;

#if SILVERLIGHT
		private BitmapImage _bitmapIcon;
#endif

    #endregion

    #region Ctors

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

      switch (windowOptions.Buttons)
      {
        case MessageWindowButtons.OK:
          _actualButtons = new List<MessageWindowButton> {MessageWindowResultKind.OK.Create()};
          break;
        case MessageWindowButtons.OKCancel:
          _actualButtons = new List<MessageWindowButton> {MessageWindowResultKind.OK.Create(), MessageWindowResultKind.Cancel.Create()};
          break;
        case MessageWindowButtons.Yes:
          _actualButtons = new List<MessageWindowButton> {MessageWindowResultKind.Yes.Create()};
          break;
        case MessageWindowButtons.YesNo:
          _actualButtons = new List<MessageWindowButton> {MessageWindowResultKind.Yes.Create(), MessageWindowResultKind.No.Create()};
          break;
        case MessageWindowButtons.YesNoCancel:
          _actualButtons = new List<MessageWindowButton> {MessageWindowResultKind.Yes.Create(), MessageWindowResultKind.No.Create(), MessageWindowResultKind.Cancel.Create()};
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      AttachButtons();

      if (windowOptions.ImageSource != null)
        ActualMessageIcon = windowOptions.ImageSource;
      else
      {
        _messageIconResourceReference = ThemeManager.GetThemeReference(ThemeKeyword.ApplicationBackgroundBrush);
        ((INotifyPropertyChanged) _messageIconResourceReference).PropertyChanged += OnThemeResourceIconChanged;
      }

      Screen.VirtualScreenSizeChanged += OnSizeChanged;

      MoveWindowToCenterInternal();

      this.InvokeOnLayoutUpdate(() =>
      {
        Focus();
      });

      this.GetServiceOrCreate(() => new KeyboardHelperService()).Action += OnKeyboardAction;

      PlatformCtor();
    }

    partial void PlatformCtor();

    #endregion

    #region Properties

#if SILVERLIGHT
		private BitmapImage BitmapIcon
		{
			set
			{
				if (ReferenceEquals(_bitmapIcon, value))
					return;
				if (_bitmapIcon != null)
					_bitmapIcon.ImageOpened -= BitmapIconOnImageOpened;

				_bitmapIcon = value;

				if (_bitmapIcon != null)
					_bitmapIcon.ImageOpened += BitmapIconOnImageOpened;
			}
		}

    private void BitmapIconOnImageOpened(object sender, RoutedEventArgs routedEventArgs)
    {
      MoveWindowToCenterInt();
    }
#endif

    public ImageSource ActualMessageIcon
    {
      get => (ImageSource) GetValue(ActualMessageIconProperty);
      private set => this.SetReadOnlyValue(ActualMessageIconPropertyKey, value);
    }

    public string Message { get; }

    public MessageWindowOptions WindowOptions { get; }

    #endregion

    #region Methods

    private void OnActualMessageIconChanged()
    {
#if SILVERLIGHT
			BitmapIcon = ActualMessageIcon as BitmapImage;
#endif
    }

    private void OnThemeResourceIconChanged(object sender, EventArgs eventArgs)
    {
      ActualMessageIcon = _messageIconResourceReference.Value as ImageSource;
    }

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

      Action<MessageWindowResult> onMessageResult = r => taskCompletionSource.SetResult(r);

      ShowImpl(new MessageWindowOptions(message, string.Empty, MessageWindowButtons.OK, MessageBoxImage.None, MessageWindowResultKind.None), onMessageResult);

      return taskCompletionSource.Task;
    }

    public static Task<MessageWindowResult> ShowAsync(string message, string caption)
    {
      var taskCompletionSource = new TaskCompletionSource<MessageWindowResult>();

      Action<MessageWindowResult> onMessageResult = r => taskCompletionSource.SetResult(r);

      ShowImpl(new MessageWindowOptions(message, caption, MessageWindowButtons.OK, MessageBoxImage.None, MessageWindowResultKind.None), onMessageResult);

      return taskCompletionSource.Task;
    }

    public static Task<MessageWindowResult> ShowAsync(string message, string caption, MessageWindowButtons buttons)
    {
      var taskCompletionSource = new TaskCompletionSource<MessageWindowResult>();

      Action<MessageWindowResult> onMessageResult = r => taskCompletionSource.SetResult(r);

      ShowImpl(new MessageWindowOptions(message, caption, buttons, MessageBoxImage.None, MessageWindowResultKind.None), onMessageResult);

      return taskCompletionSource.Task;
    }

    public static Task<MessageWindowResult> ShowAsync(string message, string caption, MessageWindowButtons buttons, MessageBoxImage image)
    {
      var taskCompletionSource = new TaskCompletionSource<MessageWindowResult>();

      Action<MessageWindowResult> onMessageResult = r => taskCompletionSource.SetResult(r);

      ShowImpl(new MessageWindowOptions(message, caption, buttons, image, MessageWindowResultKind.None), onMessageResult);

      return taskCompletionSource.Task;
    }

    public static Task<MessageWindowResult> ShowAsync(string message, string caption, MessageWindowButtons buttons, MessageBoxImage image, MessageWindowResultKind defaultResult)
    {
      var taskCompletionSource = new TaskCompletionSource<MessageWindowResult>();

      Action<MessageWindowResult> onMessageResult = r => taskCompletionSource.SetResult(r);

      ShowImpl(new MessageWindowOptions(message, caption, buttons, image, defaultResult), onMessageResult);

      return taskCompletionSource.Task;
    }

    public static Task<MessageWindowResult> ShowAsync(string message, string caption, MessageWindowButtons buttons, ImageSource image)
    {
      var taskCompletionSource = new TaskCompletionSource<MessageWindowResult>();

      Action<MessageWindowResult> onMessageResult = r => taskCompletionSource.SetResult(r);

      ShowImpl(new MessageWindowOptions(message, caption, buttons, image, MessageWindowResultKind.None), onMessageResult);

      return taskCompletionSource.Task;
    }

    public static Task<MessageWindowResult> ShowAsync(string message, string caption, MessageWindowButtons buttons, ImageSource image, MessageWindowResultKind defaultResult)
    {
      var taskCompletionSource = new TaskCompletionSource<MessageWindowResult>();

      Action<MessageWindowResult> onMessageResult = r => taskCompletionSource.SetResult(r);

      ShowImpl(new MessageWindowOptions(message, caption, buttons, image, defaultResult), onMessageResult);

      return taskCompletionSource.Task;
    }

    public static Task<MessageWindowResult> ShowAsync(MessageWindowOptions options)
    {
      var taskCompletionSource = new TaskCompletionSource<MessageWindowResult>();

      Action<MessageWindowResult> onMessageResult = r => taskCompletionSource.SetResult(r);

      ShowImpl(options, onMessageResult);

      return taskCompletionSource.Task;
    }
		
    private static MessageWindowResultKind GetDefaultCancelResult(MessageWindowButtons buttons)
    {
      switch (buttons)
      {
        case MessageWindowButtons.OK:
          return MessageWindowResultKind.OK;
        case MessageWindowButtons.OKCancel:
          return MessageWindowResultKind.Cancel;
        case MessageWindowButtons.Yes:
          return MessageWindowResultKind.Yes;
        case MessageWindowButtons.YesNo:
          return MessageWindowResultKind.No;
        case MessageWindowButtons.YesNoCancel:
          return MessageWindowResultKind.Cancel;
        default:
          return MessageWindowResultKind.None;
      }
    }

    private static MessageWindowResultKind GetDefaultOkResult(MessageWindowButtons buttons)
    {
      switch (buttons)
      {
        case MessageWindowButtons.OK:
          return MessageWindowResultKind.OK;
        case MessageWindowButtons.OKCancel:
          return MessageWindowResultKind.OK;
        case MessageWindowButtons.Yes:
          return MessageWindowResultKind.Yes;
        case MessageWindowButtons.YesNo:
          return MessageWindowResultKind.Yes;
        case MessageWindowButtons.YesNoCancel:
          return MessageWindowResultKind.Yes;
        default:
          return MessageWindowResultKind.None;
      }
    }

    private void CloseImpl(MessageWindowResultKind resultKind)
    {
      _onMessageResult?.Invoke(new MessageWindowResult(resultKind, WindowOptions));

      Screen.VirtualScreenSizeChanged -= OnSizeChanged;

      if (_messageIconResourceReference != null)
        ((INotifyPropertyChanged) _messageIconResourceReference).PropertyChanged -= OnThemeResourceIconChanged;

#if SILVERLIGHT
			BitmapIcon = null;
			WindowLayer.Instance.RemoveWindow(this);
#else
      Close();
#endif
    }

    protected override void OnExecutedCloseCommand(object commandParameter)
    {
      var resultKind = WindowOptions.DefaultResult;

      if (commandParameter is MessageWindowResultKind kind)
        resultKind = kind;

      CloseImpl(resultKind);
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

    private void InvalidateDescendantsMeasure()
    {
	    foreach (var uie in this.GetVisualDescendantsAndSelf().OfType<UIElement>()) 
		    uie.InvalidateMeasure();
    }

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

    private void OnSizeChanged(object sender, EventArgs e)
    {
      MoveWindowToCenterInternal();
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

    protected override void CloseByEnter()
    {
      CloseImpl(GetDefaultOkResult(WindowOptions.Buttons));
    }

    protected override void CloseByEscape()
    {
      CloseImpl(GetDefaultCancelResult(WindowOptions.Buttons));
    }

    internal override void ToggleMaximizeNormalState()
    {
    }

    protected override bool ActualShowMaximizeButton => false;

    protected override bool ActualShowMinimizeButton => false;

    #endregion

    protected override IEnumerable<WindowButton> ActualButtons => _actualButtons ?? Enumerable.Empty<WindowButton>();
  }

  internal static class MessageWindowResultKindExtensions
  {
    #region  Methods

    public static MessageWindowButton Create(this MessageWindowResultKind result)
    {
      return new MessageWindowButton { Result = result };
    }

    #endregion
  }
}