using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Zaaml.PresentationCore.Theming
{
  internal static class StyleExtensions
  {
    #region Methods

    public static T FindSetter<T>(this System.Windows.Style nativeStyle, DependencyProperty property) where T : class
    {
      var style = nativeStyle;

      do
      {
        var setter = style.Setters.OfType<Setter>().FirstOrDefault(s => s.Property == property);
        if (setter != null)
          return setter.Value as T;

        style = style.BasedOn;
      } while (style != null);

      return null;
    }

		public static IEnumerable<StyleBase> EnumerateBaseStyles(this StyleBase style)
		{
			return StyleBase.EnumerateBaseStyles(style);
		}

		public static IEnumerable<StyleBase> EnumerateBaseStylesAndSelf(this StyleBase style)
		{
			return StyleBase.EnumerateBaseStylesAndSelf(style);
		}

		#endregion
	}
}