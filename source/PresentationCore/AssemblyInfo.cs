// <copyright file="AssemblyInfo.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>


using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.Core.Converters;

[assembly: System.Windows.ThemeInfo(System.Windows.ResourceDictionaryLocation.None, System.Windows.ResourceDictionaryLocation.SourceAssembly)]






[assembly: XmlnsPrefix("http://schemas.zaaml.com/xaml", "zm")]

[assembly: ConvertersAssembly]


[assembly: InternalsVisibleTo("Zaaml.UI,PublicKey=" +
															ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.UI.Artboard,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.UI.Docking,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.UI.Ribbon,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.UI.PropertyView,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.Themes.Metro,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.Themes.Docking.Metro,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.Themes.Ribbon.Metro,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.Themes.Default,PublicKey=" +
                              ZaamlInfo.PublicKey)]

[assembly: InternalsVisibleTo("Zaaml.DevThemes.Default,PublicKey=" +
                              ZaamlInfo.PublicKey)]