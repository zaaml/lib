// <copyright file="AssemblyInfo.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;
using System.Windows.Markup;
using Zaaml.PresentationCore.Theming;
using System.Windows;
using Zaaml.Core;
using Zaaml.CodeThemeImplementation;

[assembly: XmlnsPrefix("http://schemas.zaaml.com/xaml", "zm")]

[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]

// Code UI Theme
[assembly: ThemeAssembly(typeof(CodeTheme), "")]

[assembly: InternalsVisibleTo("Zaaml.Themes.Docking.Code,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.Themes.Ribbon.Code,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.Themes.Default,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.DevThemes.Default,PublicKey=" +
                              ZaamlInfo.PublicKey)]