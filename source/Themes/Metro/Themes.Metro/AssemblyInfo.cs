// <copyright file="AssemblyInfo.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using System.Windows.Markup;
using Zaaml.PresentationCore.Theming;
using System.Windows;
using Zaaml.Core;
using Zaaml.MetroThemeImplementation;

[assembly: XmlnsPrefix("http://schemas.zaaml.com/xaml", "zm")]

[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]

// Metro UI Theme
[assembly: ThemeAssembly(typeof(MetroTheme), "")]

[assembly: InternalsVisibleTo("Zaaml.Themes.Docking.Metro,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.Themes.Ribbon.Metro,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.Themes.Default,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.DevThemes.Default,PublicKey=" +
                              ZaamlInfo.PublicKey)]