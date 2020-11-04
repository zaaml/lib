// <copyright file="KnownColors.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace Zaaml.PresentationCore
{
  public static class KnownColors
  {
    #region Static Fields and Constants

    private static readonly Dictionary<string, Color> ColorsDictionary;
    private static readonly Dictionary<Color, string> ColorNamesDictionary;

    #endregion

    #region Ctors

    static KnownColors()
    {
      ColorsDictionary = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase);
      ColorNamesDictionary = new Dictionary<Color, string>();
      var colorsType = typeof(KnownColors);
      var colorType = typeof(Color);

      var propertyInfos = colorsType.GetProperties(BindingFlags.Static | BindingFlags.Public);

#if SILVERLIGHT
			if (propertyInfos == null)
				return;
#endif

      foreach (var propertyInfo in propertyInfos.Where(p => p.PropertyType == colorType))
      {
        var color = (Color) propertyInfo.GetValue(null, null);
        var name = propertyInfo.Name;
        ColorsDictionary[name] = color;
        ColorNamesDictionary[color] = name;
      }
    }

    #endregion

    #region Properties

    public static Color AliceBlue => Color.FromArgb(255, 240, 248, 255);
    public static Color AntiqueWhite => Color.FromArgb(255, 250, 235, 215);
    public static Color Aqua => Color.FromArgb(255, 0, 255, 255);
    public static Color Aquamarine => Color.FromArgb(255, 127, 255, 212);
    public static Color Azure => Color.FromArgb(255, 240, 255, 255);
    public static Color Beige => Color.FromArgb(255, 245, 245, 220);
    public static Color Bisque => Color.FromArgb(255, 255, 228, 196);
    public static Color Black => Color.FromArgb(255, 0, 0, 0);
    public static Color BlanchedAlmond => Color.FromArgb(255, 255, 235, 205);
    public static Color Blue => Color.FromArgb(255, 0, 0, 255);
    public static Color BlueViolet => Color.FromArgb(255, 138, 43, 226);
    public static Color Brown => Color.FromArgb(255, 165, 42, 42);
    public static Color BurlyWood => Color.FromArgb(255, 222, 184, 135);
    public static Color CadetBlue => Color.FromArgb(255, 95, 158, 160);
    public static Color Chartreuse => Color.FromArgb(255, 127, 255, 0);
    public static Color Chocolate => Color.FromArgb(255, 210, 105, 30);
    public static Color Coral => Color.FromArgb(255, 255, 127, 80);
    public static Color CornflowerBlue => Color.FromArgb(255, 100, 149, 237);
    public static Color Cornsilk => Color.FromArgb(255, 255, 248, 220);
    public static Color Crimson => Color.FromArgb(255, 220, 20, 60);
    public static Color Cyan => Color.FromArgb(255, 0, 255, 255);
    public static Color DarkBlue => Color.FromArgb(255, 0, 0, 139);
    public static Color DarkCyan => Color.FromArgb(255, 0, 139, 139);
    public static Color DarkGoldenrod => Color.FromArgb(255, 184, 134, 11);
    public static Color DarkGray => Color.FromArgb(255, 169, 169, 169);
    public static Color DarkGreen => Color.FromArgb(255, 0, 100, 0);
    public static Color DarkKhaki => Color.FromArgb(255, 189, 183, 107);
    public static Color DarkMagenta => Color.FromArgb(255, 139, 0, 139);
    public static Color DarkOliveGreen => Color.FromArgb(255, 85, 107, 47);
    public static Color DarkOrange => Color.FromArgb(255, 255, 140, 0);
    public static Color DarkOrchid => Color.FromArgb(255, 153, 50, 204);
    public static Color DarkRed => Color.FromArgb(255, 139, 0, 0);
    public static Color DarkSalmon => Color.FromArgb(255, 233, 150, 122);
    public static Color DarkSeaGreen => Color.FromArgb(255, 143, 188, 143);
    public static Color DarkSlateBlue => Color.FromArgb(255, 72, 61, 139);
    public static Color DarkSlateGray => Color.FromArgb(255, 47, 79, 79);
    public static Color DarkTurquoise => Color.FromArgb(255, 0, 206, 209);
    public static Color DarkViolet => Color.FromArgb(255, 148, 0, 211);
    public static Color DeepPink => Color.FromArgb(255, 255, 20, 147);
    public static Color DeepSkyBlue => Color.FromArgb(255, 0, 191, 255);
    public static Color DimGray => Color.FromArgb(255, 105, 105, 105);
    public static Color DodgerBlue => Color.FromArgb(255, 30, 144, 255);
    public static Color Firebrick => Color.FromArgb(255, 178, 34, 34);
    public static Color FloralWhite => Color.FromArgb(255, 255, 250, 240);
    public static Color ForestGreen => Color.FromArgb(255, 34, 139, 34);
    public static Color Fuchsia => Color.FromArgb(255, 255, 0, 255);
    public static Color Gainsboro => Color.FromArgb(255, 220, 220, 220);
    public static Color GhostWhite => Color.FromArgb(255, 248, 248, 255);
    public static Color Gold => Color.FromArgb(255, 255, 215, 0);
    public static Color Goldenrod => Color.FromArgb(255, 218, 165, 32);
    public static Color Gray => Color.FromArgb(255, 128, 128, 128);
    public static Color Green => Color.FromArgb(255, 0, 128, 0);
    public static Color GreenYellow => Color.FromArgb(255, 173, 255, 47);
    public static Color Honeydew => Color.FromArgb(255, 240, 255, 240);
    public static Color HotPink => Color.FromArgb(255, 255, 105, 180);
    public static Color IndianRed => Color.FromArgb(255, 205, 92, 92);
    public static Color Indigo => Color.FromArgb(255, 75, 0, 130);
    public static Color Ivory => Color.FromArgb(255, 255, 255, 240);
    public static Color Khaki => Color.FromArgb(255, 240, 230, 140);
    public static Color Lavender => Color.FromArgb(255, 230, 230, 250);
    public static Color LavenderBlush => Color.FromArgb(255, 255, 240, 245);
    public static Color LawnGreen => Color.FromArgb(255, 124, 252, 0);
    public static Color LemonChiffon => Color.FromArgb(255, 255, 250, 205);
    public static Color LightBlue => Color.FromArgb(255, 173, 216, 230);
    public static Color LightCoral => Color.FromArgb(255, 240, 128, 128);
    public static Color LightCyan => Color.FromArgb(255, 224, 255, 255);
    public static Color LightGoldenrodYellow => Color.FromArgb(255, 250, 250, 210);
    public static Color LightGray => Color.FromArgb(255, 211, 211, 211);
    public static Color LightGreen => Color.FromArgb(255, 144, 238, 144);
    public static Color LightPink => Color.FromArgb(255, 255, 182, 193);
    public static Color LightSalmon => Color.FromArgb(255, 255, 160, 122);
    public static Color LightSeaGreen => Color.FromArgb(255, 32, 178, 170);
    public static Color LightSkyBlue => Color.FromArgb(255, 135, 206, 250);
    public static Color LightSlateGray => Color.FromArgb(255, 119, 136, 153);
    public static Color LightSteelBlue => Color.FromArgb(255, 176, 196, 222);
    public static Color LightYellow => Color.FromArgb(255, 255, 255, 224);
    public static Color Lime => Color.FromArgb(255, 0, 255, 0);
    public static Color LimeGreen => Color.FromArgb(255, 50, 205, 50);
    public static Color Linen => Color.FromArgb(255, 250, 240, 230);
    public static Color Magenta => Color.FromArgb(255, 255, 0, 255);
    public static Color Maroon => Color.FromArgb(255, 128, 0, 0);
    public static Color MediumAquamarine => Color.FromArgb(255, 102, 205, 170);
    public static Color MediumBlue => Color.FromArgb(255, 0, 0, 205);
    public static Color MediumOrchid => Color.FromArgb(255, 186, 85, 211);
    public static Color MediumPurple => Color.FromArgb(255, 147, 112, 219);
    public static Color MediumSeaGreen => Color.FromArgb(255, 60, 179, 113);
    public static Color MediumSlateBlue => Color.FromArgb(255, 123, 104, 238);
    public static Color MediumSpringGreen => Color.FromArgb(255, 0, 250, 154);
    public static Color MediumTurquoise => Color.FromArgb(255, 72, 209, 204);
    public static Color MediumVioletRed => Color.FromArgb(255, 199, 21, 133);
    public static Color MidnightBlue => Color.FromArgb(255, 25, 25, 112);
    public static Color MintCream => Color.FromArgb(255, 245, 255, 250);
    public static Color MistyRose => Color.FromArgb(255, 255, 228, 225);
    public static Color Moccasin => Color.FromArgb(255, 255, 228, 181);
    public static Color NavajoWhite => Color.FromArgb(255, 255, 222, 173);
    public static Color Navy => Color.FromArgb(255, 0, 0, 128);
    public static Color OldLace => Color.FromArgb(255, 253, 245, 230);
    public static Color Olive => Color.FromArgb(255, 128, 128, 0);
    public static Color OliveDrab => Color.FromArgb(255, 107, 142, 35);
    public static Color Orange => Color.FromArgb(255, 255, 165, 0);
    public static Color OrangeRed => Color.FromArgb(255, 255, 69, 0);
    public static Color Orchid => Color.FromArgb(255, 218, 112, 214);
    public static Color PaleGoldenrod => Color.FromArgb(255, 238, 232, 170);
    public static Color PaleGreen => Color.FromArgb(255, 152, 251, 152);
    public static Color PaleTurquoise => Color.FromArgb(255, 175, 238, 238);
    public static Color PaleVioletRed => Color.FromArgb(255, 219, 112, 147);
    public static Color PapayaWhip => Color.FromArgb(255, 255, 239, 213);
    public static Color PeachPuff => Color.FromArgb(255, 255, 218, 185);
    public static Color Peru => Color.FromArgb(255, 205, 133, 63);
    public static Color Pink => Color.FromArgb(255, 255, 192, 203);
    public static Color Plum => Color.FromArgb(255, 221, 160, 221);
    public static Color PowderBlue => Color.FromArgb(255, 176, 224, 230);
    public static Color Purple => Color.FromArgb(255, 128, 0, 128);
    public static Color Red => Color.FromArgb(255, 255, 0, 0);
    public static Color RosyBrown => Color.FromArgb(255, 188, 143, 143);
    public static Color RoyalBlue => Color.FromArgb(255, 65, 105, 225);
    public static Color SaddleBrown => Color.FromArgb(255, 139, 69, 19);
    public static Color Salmon => Color.FromArgb(255, 250, 128, 114);
    public static Color SandyBrown => Color.FromArgb(255, 244, 164, 96);
    public static Color SeaGreen => Color.FromArgb(255, 46, 139, 87);
    public static Color SeaShell => Color.FromArgb(255, 255, 245, 238);
    public static Color Sienna => Color.FromArgb(255, 160, 82, 45);
    public static Color Silver => Color.FromArgb(255, 192, 192, 192);
    public static Color SkyBlue => Color.FromArgb(255, 135, 206, 235);
    public static Color SlateBlue => Color.FromArgb(255, 106, 90, 205);
    public static Color SlateGray => Color.FromArgb(255, 112, 128, 144);
    public static Color Snow => Color.FromArgb(255, 255, 250, 250);
    public static Color SpringGreen => Color.FromArgb(255, 0, 255, 127);
    public static Color SteelBlue => Color.FromArgb(255, 70, 130, 180);
    public static Color Tan => Color.FromArgb(255, 210, 180, 140);
    public static Color Teal => Color.FromArgb(255, 0, 128, 128);
    public static Color Thistle => Color.FromArgb(255, 216, 191, 216);
    public static Color Tomato => Color.FromArgb(255, 255, 99, 71);
    public static Color Transparent => Color.FromArgb(0, 255, 255, 255);
    public static Color Turquoise => Color.FromArgb(255, 64, 224, 208);
    public static Color Violet => Color.FromArgb(255, 238, 130, 238);
    public static Color Wheat => Color.FromArgb(255, 245, 222, 179);
    public static Color White => Color.FromArgb(255, 255, 255, 255);
    public static Color WhiteSmoke => Color.FromArgb(255, 245, 245, 245);
    public static Color Yellow => Color.FromArgb(255, 255, 255, 0);
    public static Color YellowGreen => Color.FromArgb(255, 154, 205, 50);

    #endregion

    #region  Methods

    public static bool TryGetColor(string colorName, out Color color)
    {
      return ColorsDictionary.TryGetValue(colorName, out color);
    }

    public static bool TryGetColorName(Color color, out string colorName)
    {
      return ColorNamesDictionary.TryGetValue(color, out colorName);
    }

    #endregion
  }
}