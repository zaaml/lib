using System.ComponentModel;
using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Panels;

namespace Zaaml.UI.Controls.Primitives.TextPrimitives
{
	public sealed class TextPresenter : FixedTemplateControl<TextWrapPanel>
	{
		public static readonly DependencyProperty TextProperty = DPM.Register<string, TextPresenter>
			("Text", t => t.OnTextChanged);

		public static readonly DependencyProperty TextWrappingProperty = DPM.Register<TextWrapping, TextPresenter>
			("TextWrapping", TextWrapping.NoWrap, t => t.OnTextWrappingChanged);

		private void OnTextWrappingChanged(TextWrapping oldTextWrapping, TextWrapping newTextWrapping)
		{
			if (TemplateRoot != null)
				TemplateRoot.TextWrapping = newTextWrapping;
		}

		public TextWrapping TextWrapping
		{
			get => (TextWrapping) GetValue(TextWrappingProperty);
			set => SetValue(TextWrappingProperty, value);
		}

		private readonly FontOptions _fontOptions;

		public TextPresenter()
		{
			_fontOptions = new FontOptions();

			_fontOptions.Observe(this);

			_fontOptions.PropertyChanged += FontOptionsOnPropertyChanged;
		}

		public string Text
		{
			get => (string) GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		private void FontOptionsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateFontOptions();
		}

		private void UpdateFontOptions()
		{
			if (TemplateRoot == null)
				return;

			TemplateRoot.FontOptions = new FontOptionsStruct(_fontOptions.FontFamily, _fontOptions.FontSize,
				_fontOptions.FontStyle, _fontOptions.FontWeight);
		}

		private void OnTextChanged(string oldText, string newText)
		{
			if (TemplateRoot != null)
				TemplateRoot.Text = newText;
		}

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			TemplateRoot.Text = Text;

			UpdateFontOptions();
		}
	}
}